using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace GHelper.Helpers
{
    public static class ProcessHelper
    {
        private static long lastAdmin;

        private static bool? _isSystem;
        public static bool IsRunningAsSystem()
        {
            if (_isSystem.HasValue)
                return _isSystem.Value;

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                _isSystem = identity != null && identity.IsSystem;
            }

            return _isSystem.Value;
        }

        private static string? _interactiveUserSid;
        private static int _interactiveUserSidSessionId = -1;

        private enum WTS_INFO_CLASS
        {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(int sessionId, out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            int sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out int pBytesReturned
        );

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        public static string? GetInteractiveUserSid()
        {
            try
            {
                int sessionId = Process.GetCurrentProcess().SessionId;
                if (_interactiveUserSid != null && _interactiveUserSidSessionId == sessionId)
                    return _interactiveUserSid;

                _interactiveUserSidSessionId = sessionId;
                _interactiveUserSid = GetUserSidForSession(sessionId);
                if (!string.IsNullOrWhiteSpace(_interactiveUserSid))
                    return _interactiveUserSid;

                uint activeSession = WTSGetActiveConsoleSessionId();
                if (activeSession == 0xFFFFFFFF) return null;

                _interactiveUserSid = GetUserSidForSession((int)activeSession);
                return _interactiveUserSid;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetUserSidForSession(int sessionId)
        {
            string? sidFromToken = GetUserSidFromToken(sessionId);
            if (!string.IsNullOrWhiteSpace(sidFromToken)) return sidFromToken;

            string? user = QueryWtsString(sessionId, WTS_INFO_CLASS.WTSUserName);
            if (string.IsNullOrWhiteSpace(user)) return null;

            string? domain = QueryWtsString(sessionId, WTS_INFO_CLASS.WTSDomainName);

            try
            {
                NTAccount account = string.IsNullOrWhiteSpace(domain) ? new NTAccount(user) : new NTAccount(domain, user);
                var sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                return sid.Value;
            }
            catch
            {
                try
                {
                    string full = string.IsNullOrWhiteSpace(domain) ? user : $"{domain}\\{user}";
                    var sid = (SecurityIdentifier)new NTAccount(full).Translate(typeof(SecurityIdentifier));
                    return sid.Value;
                }
                catch
                {
                    return null;
                }
            }
        }

        private static string? GetUserSidFromToken(int sessionId)
        {
            IntPtr token = IntPtr.Zero;
            try
            {
                if (!WTSQueryUserToken(sessionId, out token) || token == IntPtr.Zero)
                    return null;

                using var identity = new WindowsIdentity(token);
                token = IntPtr.Zero; // WindowsIdentity owns the handle
                return identity.User?.Value;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    try { CloseHandle(token); } catch { }
                }
            }
        }

        private static string? QueryWtsString(int sessionId, WTS_INFO_CLASS infoClass)
        {
            IntPtr buffer = IntPtr.Zero;
            int bytesReturned = 0;

            try
            {
                if (!WTSQuerySessionInformation(IntPtr.Zero, sessionId, infoClass, out buffer, out bytesReturned))
                    return null;

                if (buffer == IntPtr.Zero || bytesReturned <= 1)
                    return null;

                string? result = Marshal.PtrToStringUni(buffer);
                if (string.IsNullOrWhiteSpace(result)) return null;
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    try { WTSFreeMemory(buffer); } catch { }
                }
            }
        }

        public static void CheckAlreadyRunning()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);

            if (processes.Length > 1)
            {
                foreach (Process process in processes)
                    if (process.Id != currentProcess.Id)
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine(ex.ToString());
                            MessageBox.Show(Properties.Strings.AppAlreadyRunningText, Properties.Strings.AppAlreadyRunning, MessageBoxButtons.OK);
                            Application.Exit();
                            return;
                        }
            }
        }

        public static bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
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
            foreach (var process in Process.GetProcessesByName(name))
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

        public static string RunCMD(string name, string args, string? directory = null)
        {
            var cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.FileName = name;
            cmd.StartInfo.Arguments = args;
            if (directory != null) cmd.StartInfo.WorkingDirectory = directory;
            cmd.Start();


            var watch = Stopwatch.StartNew();
            string result = cmd.StandardOutput.ReadToEnd().Replace(Environment.NewLine, " ").Trim(' ');
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
