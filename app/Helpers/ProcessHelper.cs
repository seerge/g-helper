using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GHelper.Helpers
{
    public static class ProcessHelper
    {
        private const string ExitEventName = "Global\\GHelperApp-Exit";
        private static EventWaitHandle? exitEvent;
        private static long lastAdmin;

        private static readonly Lazy<bool> _isSystem = new Lazy<bool>(() =>
        {
            using var identity = WindowsIdentity.GetCurrent();
            return identity.IsSystem;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public static bool IsRunningAsSystem() => _isSystem.Value;

        private static readonly Lazy<(SecurityIdentifier Sid, string Name)> _sessionUser =
            new Lazy<(SecurityIdentifier, string)>(LoadSessionUser, LazyThreadSafetyMode.ExecutionAndPublication);

        // Signed-in user of this session. Differs from the process user when UAC elevated with another (admin) account
        public static (SecurityIdentifier Sid, string Name) GetSessionUser() => _sessionUser.Value;

        private static (SecurityIdentifier, string) LoadSessionUser()
        {
            using var current = WindowsIdentity.GetCurrent();
            try
            {
                string? user = QuerySessionString(WTSUserName);
                string? domain = QuerySessionString(WTSDomainName);
                if (!string.IsNullOrEmpty(user))
                {
                    var account = string.IsNullOrEmpty(domain) ? new NTAccount(user) : new NTAccount(domain, user);
                    var sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                    if (!sid.Equals(current.User)) return (sid, account.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Can't read session user: " + ex.Message);
            }
            return (current.User!, current.Name);
        }

        private static string? QuerySessionString(int infoClass)
        {
            if (!WTSQuerySessionInformation(IntPtr.Zero, WTS_CURRENT_SESSION, infoClass, out IntPtr buffer, out _)) return null;
            try
            {
                return Marshal.PtrToStringUni(buffer);
            }
            finally
            {
                WTSFreeMemory(buffer);
            }
        }

        public static bool IsElevatedAsOtherUser()
        {
            if (!IsUserAdministrator() || IsRunningAsSystem()) return false;
            using var current = WindowsIdentity.GetCurrent();
            return !GetSessionUser().Sid.Equals(current.User);
        }

        private static readonly Lazy<bool> _isStandardUser = new Lazy<bool>(() =>
        {
            if (IsUserAdministrator()) return false;
            using var identity = WindowsIdentity.GetCurrent();
            // Admins running unelevated have a split (limited) token, standard accounts don't
            return GetTokenInformation(identity.Token, TokenElevationType, out int type, sizeof(int), out _) && type == TokenElevationTypeDefault;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        // Account that can't elevate without another account's credentials
        public static bool IsStandardUser() => _isStandardUser.Value;

        public static void CheckAlreadyRunning()
        {
            var sec = new EventWaitHandleSecurity();
            sec.AddAccessRule(new EventWaitHandleAccessRule(
                new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify,
                AccessControlType.Allow));

            bool created = false;
            try
            {
                exitEvent = EventWaitHandleAcl.Create(false, EventResetMode.ManualReset, ExitEventName, out created, sec);
            }
            catch
            {
                try { exitEvent = EventWaitHandle.OpenExisting(ExitEventName); }
                catch { }
            }

            if (!created && exitEvent != null)
            {
                try
                {
                    exitEvent.Set();
                    exitEvent.Reset();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Broadcast exit failed: " + ex.Message);
                    exitEvent = null;
                }
            }

            using Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
            try
            {
                if (processes.Length > 1)
                {
                    var failed = new List<Process>();
                    foreach (Process process in processes)
                        // Session 0 holds the SYSTEM agent and charge limit tasks, not another UI
                        if (process.Id != currentProcess.Id && process.SessionId != 0)
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine($"Can't kill PID {process.Id}: {ex.Message}");
                                failed.Add(process);
                            }
                        }

                    if (failed.Count > 0)
                    {
                        Thread.Sleep(2000);

                        foreach (var p in failed)
                        {
                            bool stillAlive;
                            try { stillAlive = !p.HasExited; }
                            catch { stillAlive = true; }

                            if (stillAlive)
                            {
                                MessageBox.Show(Properties.Strings.AppAlreadyRunningText, Properties.Strings.AppAlreadyRunning, MessageBoxButtons.OK);
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            finally
            {
                foreach (Process p in processes) p.Dispose();
            }

            if (exitEvent != null)
                ThreadPool.RegisterWaitForSingleObject(exitEvent, (_, _) => Application.Exit(), null, Timeout.Infinite, true);
        }

        public static bool IsUserAdministrator()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RunAsAdmin(string? param = null, bool force = false)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAdmin) < 2000) return;
            lastAdmin = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Check if the current user is an administrator
            if (!IsUserAdministrator() || force)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Arguments = param;
                startInfo.Verb = "runas";
                try
                {
                    Process.Start(startInfo);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
            }
        }

        // Runs a one-off elevated action and waits for it, without restarting the app
        public static bool RunElevatedAndWait(string param)
        {
            try
            {
                using Process? process = Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath,
                    Arguments = param,
                    Verb = "runas"
                });
                if (process is null) return false;
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch (Win32Exception ex)
            {
                Logger.WriteLine($"Elevated {param} cancelled: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Elevated {param} failed: {ex.Message}");
                return false;
            }
        }


        public static void KillByName(string name)
        {
            var processes = Process.GetProcessesByName(name);
            try
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        Logger.WriteLine($"Stopped: {process.ProcessName}");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine($"Failed to stop: {process.ProcessName} {ex.Message}");
                    }
                }
            }
            finally
            {
                foreach (var p in processes) p.Dispose();
            }
        }

        public static void KillSmartDisplayControl()
        {
            KillByName("ASUSSmartDisplayControl");
        }

        public static void KillByProcess(Process process)
        {
            try
            {
                process.Kill();
                Logger.WriteLine($"Stopped: {process.ProcessName}");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to stop: {process.ProcessName} {ex.Message}");
            }
        }

        public static void StopDisableService(string serviceName, string disable = "Disabled")
        {
            try
            {
                string script = $"Get-Service -Name \"{serviceName}\" | Stop-Service -Force -PassThru | Set-Service -StartupType {disable}";
                Logger.WriteLine(script);
                RunCMD("powershell", script);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }

        public static void StartEnableService(string serviceName, bool automatic = true)
        {
            try
            {
                string script = $"Set-Service -Name \"{serviceName}\" -Status running" + (automatic? " -StartupType Automatic":"");
                Logger.WriteLine(script);
                RunCMD("powershell", script);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }

        public static string RunCMD(string name, string args, string? directory = null, int timeoutMs = 0)
        {
            using var cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.FileName = (name == "powershell") ? Path.Combine(Environment.SystemDirectory, @"WindowsPowerShell\v1.0\powershell.exe") : name;
            cmd.StartInfo.Arguments = (name == "powershell") ? "-NoProfile " + args : args;
            if (directory != null) cmd.StartInfo.WorkingDirectory = directory;
            cmd.Start();

            var watch = Stopwatch.StartNew();
            string result;

            if (timeoutMs > 0)
            {
                var readTask = cmd.StandardOutput.ReadToEndAsync();
                if (!readTask.Wait(timeoutMs))
                {
                    try { cmd.Kill(entireProcessTree: true); } catch { }
                    watch.Stop();
                    Logger.WriteLine(name + " " + args);
                    Logger.WriteLine($"{watch.ElapsedMilliseconds} ms: TIMEOUT after {timeoutMs} ms");
                    return string.Empty;
                }
                result = readTask.Result.Replace(Environment.NewLine, " ").Trim(' ');
            }
            else
            {
                result = cmd.StandardOutput.ReadToEnd().Replace(Environment.NewLine, " ").Trim(' ');
            }

            watch.Stop();
            Logger.WriteLine(name + " " + args);
            Logger.WriteLine(watch.ElapsedMilliseconds + " ms: " + result);
            cmd.WaitForExit();

            return result;
        }

        public static void SetPriority(ProcessPriorityClass priorityClass = ProcessPriorityClass.Normal)
        {
            try
            {
                using (Process p = Process.GetCurrentProcess())
                    p.PriorityClass = priorityClass;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }

        const int WTS_CURRENT_SESSION = -1;
        const int WTSUserName = 5;
        const int WTSDomainName = 7;
        const int TokenElevationType = 18;
        const int TokenElevationTypeDefault = 1;

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, int wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, int TokenInformationClass, out int TokenInformation, uint TokenInformationLength, out uint ReturnLength);

    }
}
