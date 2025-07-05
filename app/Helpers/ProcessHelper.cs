using System.Diagnostics;
using System.Security.Principal;

namespace GHelper.Helpers
{
    public static class ProcessHelper
    {
        private static long lastAdmin;

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

            Logger.WriteLine(name + " " + args);
            string result = cmd.StandardOutput.ReadToEnd().Replace(Environment.NewLine, " ").Trim(' ');
            Logger.WriteLine(result);
            
            cmd.WaitForExit();

            return result;
        }


    }
}
