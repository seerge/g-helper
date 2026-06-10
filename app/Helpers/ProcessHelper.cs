using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GHelper.Helpers
{
    public static class ProcessHelper
    {
        private const string ExitEventName = "Global\\GHelperApp-Exit";
        private static EventWaitHandle? exitEvent;
        private static long lastAdmin;

        private static bool? _isSystem;
        public static bool IsRunningAsSystem()
        {
            if (_isSystem.HasValue)
                return _isSystem.Value;

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (identity == null)
                    _isSystem = false;
                else
                    _isSystem = string.Equals(identity.Name, @"NT AUTHORITY\SYSTEM", StringComparison.OrdinalIgnoreCase);
            }

            return _isSystem.Value;
        }

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
                        if (process.Id != currentProcess.Id)
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
                                Application.Exit();
                                return;
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
            cmd.StartInfo.FileName = name;
            cmd.StartInfo.Arguments = args;
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


    }
}
