using GHelper.Mode;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GHelper.Helpers
{
    // Command line control of running app: "GHelper.exe mode turbo", "GHelper.exe modes", "GHelper.exe cycle"
    public static class PipeServer
    {
        public const string PipeName = "GHelperCtl";

        public static bool IsCommand(string action)
        {
            return action == "mode" || action == "modes" || action == "cycle";
        }

        public static string? Send(string command)
        {
            try
            {
                using var pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
                pipe.Connect(1000);
                using var writer = new StreamWriter(pipe, leaveOpen: true) { AutoFlush = true };
                using var reader = new StreamReader(pipe, leaveOpen: true);
                writer.WriteLine(command);
                return reader.ReadLine();
            }
            catch
            {
                return null;
            }
        }

        public static void Start()
        {
            new Thread(Listen) { IsBackground = true }.Start();
        }

        private static void Listen()
        {
            // App can run elevated, allow regular user to connect
            var security = new PipeSecurity();
            security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            security.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User!, PipeAccessRights.FullControl, AccessControlType.Allow));

            while (true)
            {
                try
                {
                    using var pipe = NamedPipeServerStreamAcl.Create(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, security);
                    pipe.WaitForConnection();

                    using var reader = new StreamReader(pipe, leaveOpen: true);
                    using var writer = new StreamWriter(pipe, leaveOpen: true) { AutoFlush = true };
                    writer.WriteLine(Execute(reader.ReadLine() ?? ""));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Pipe: " + ex.Message);
                    Thread.Sleep(500);
                }
            }
        }

        private static string Execute(string command)
        {
            string[] args = command.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return "ERR empty command";

            Logger.WriteLine("Pipe: " + command);

            switch (args[0].ToLower())
            {
                case "modes":
                    return "OK " + string.Join(";", Modes.GetDictonary().Select(m => m.Key + "=" + m.Value));
                case "mode":
                    if (args.Length > 1)
                    {
                        int mode = Modes.Find(args[1]);
                        if (mode < 0) return "ERR unknown mode: " + args[1];
                        Program.settingsForm.Invoke(delegate
                        {
                            Program.modeControl.SetPerformanceMode(mode, true);
                        });
                    }
                    return "OK " + Modes.GetCurrent() + "=" + Modes.GetCurrentName();
                case "cycle":
                    Program.settingsForm.Invoke(delegate
                    {
                        Program.modeControl.CyclePerformanceMode();
                    });
                    return "OK " + Modes.GetCurrent() + "=" + Modes.GetCurrentName();
                case "routines":
                    return "OK " + Program.routineControl.Status;
                default:
                    return "ERR unknown command: " + args[0];
            }
        }
    }
}
