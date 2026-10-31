using GHelper.Mode;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using ExecAction = Microsoft.Win32.TaskScheduler.ExecAction;
using TaskService = Microsoft.Win32.TaskScheduler.TaskService;

namespace GHelper.Helpers;

public enum AgentCommand : byte
{
    RyzenPower = 1,
    Ryzen,
    AutoRyzen,
    ResetRyzen,
    CpuTemp,
    GpuClocks,
    GpuClocksReset,
    StopServices,
    StartServices,
}

/// Admin-only hardware calls for a standard (non-admin) account.
/// The unelevated UI starts a SYSTEM copy of G-Helper from the agent task and sends it one command byte per call.
public static class PrivilegedAgent
{
    const int StartTimeout = 10000;

    static readonly object sendLock = new();
    static Lazy<bool> available = new(CheckAvailable);
    static System.Threading.Timer? watchdog;

    static string PipeName(string sid) => "GHelperAgent_" + sid;

    public static bool IsAvailable => available.Value;

    public static void Invalidate() => available = new Lazy<bool>(CheckAvailable);

    static bool CheckAvailable()
    {
        if (!ProcessHelper.IsStandardUser()) return false;

        string? exe;
        try
        {
            using TaskService taskService = new TaskService();
            var task = taskService.GetTask(Startup.agentTaskName);
            if (task is null) return false;
            exe = (task.Definition.Actions.FirstOrDefault() as ExecAction)?.Path;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Can't read agent task: " + ex.Message);
            return false;
        }

        if (exe is null) return false;

        // Folder permissions may have changed since the admin set this up
        if (!Startup.IsProtectedLocation(exe, ProcessHelper.GetSessionUser().Sid))
        {
            Logger.WriteLine("Agent exe can be modified by the current user, not using it: " + exe);
            MessageBox.Show($"The G-Helper folder used for admin-only features can now be modified by your account, so those features are disabled.\n\n{exe}\n\nAsk an administrator to restore the folder permissions, or move GHelper.exe to a protected folder and turn Run on Startup off and on again.", "Run on Startup", MessageBoxButtons.OK);
            return false;
        }

        return true;
    }

    static void StartAgent()
    {
        try
        {
            using TaskService taskService = new TaskService();
            taskService.GetTask(Startup.agentTaskName)?.Run();
            Logger.WriteLine("Agent starting");
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Can't start agent: " + ex.Message);
        }
    }

    public static void Post(AgentCommand command) => Task.Run(() => Send(command));

    // Blocks until the agent ran the command, call it off the UI thread
    public static bool Send(AgentCommand command)
    {
        if (!IsAvailable) return false;

        lock (sendLock)
        {
            AppConfig.Flush();
            string pipe = PipeName(ProcessHelper.GetSessionUser().Sid.Value);

            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", pipe, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Identification);
                    client.Connect(attempt == 0 ? 200 : StartTimeout);
                    client.WriteByte((byte)command);
                    client.Flush();
                    bool result = client.ReadByte() == 1;
                    Logger.WriteLine($"Agent {command}: {result}");
                    return result;
                }
                catch (TimeoutException)
                {
                    if (attempt == 0) StartAgent();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Agent {command} failed: {ex.Message}");
                    return false;
                }
            }

            Logger.WriteLine($"Agent {command}: not running");
            return false;
        }
    }

    // SYSTEM side, started by the agent task with the user's SID
    public static void Run(string sid)
    {
        SecurityIdentifier user;
        try
        {
            user = new SecurityIdentifier(sid);
        }
        catch (Exception)
        {
            Logger.WriteLine("Agent: invalid user " + sid);
            return;
        }

        if (!ProcessHelper.IsRunningAsSystem() || AppConfig.GetUserConfigFile(user.Value) is null)
        {
            Logger.WriteLine("Agent: refusing to start for " + user.Value);
            return;
        }

        // Same check as at registration, in case the folder permissions were loosened since
        if (!Startup.IsProtectedLocation(Application.ExecutablePath, user))
        {
            Logger.WriteLine("Agent: " + Application.ExecutablePath + " can be modified by " + user.Value + ", refusing to start");
            return;
        }

        var security = new PipeSecurity();
        security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), PipeAccessRights.FullControl, AccessControlType.Allow));
        security.AddAccessRule(new PipeAccessRule(user, PipeAccessRights.ReadWrite | PipeAccessRights.Synchronize, AccessControlType.Allow));

        NamedPipeServerStream server;
        try
        {
            server = NamedPipeServerStreamAcl.Create(PipeName(user.Value), PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.FirstPipeInstance, 0, 0, security);
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Agent: can't create pipe: " + ex.Message);
            return;
        }

        AppConfig.LoadForUser(user.Value);
        Program.acpi = new AsusACPI();
        ModeControl modeControl = new ModeControl();
        Program.modeControl = modeControl;

        Logger.WriteLine("Agent started for " + user.Value);

        // Leave once G-Helper is closed, so updates can replace the exe
        int idle = 0;
        watchdog = new System.Threading.Timer(_ =>
        {
            idle = IsAppRunning() ? 0 : idle + 1;
            if (idle < 2) return;
            Logger.WriteLine("Agent: G-Helper closed, exiting");
            Environment.Exit(0);
        }, null, 5000, 5000);

        while (true)
        {
            try
            {
                server.WaitForConnection();
                int read = server.ReadByte();
                bool result = read > 0 && Enum.IsDefined((AgentCommand)read) && Execute(modeControl, (AgentCommand)read);
                server.WriteByte(result ? (byte)1 : (byte)0);
                server.WaitForPipeDrain();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Agent: " + ex.Message);
            }
            finally
            {
                try { server.Disconnect(); } catch { }
            }
        }
    }

    static bool IsAppRunning()
    {
        using Process current = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(current.ProcessName);
        try
        {
            return processes.Any(p => p.SessionId != 0);
        }
        finally
        {
            foreach (Process p in processes) p.Dispose();
        }
    }

    static bool Execute(ModeControl modeControl, AgentCommand command)
    {
        AppConfig.Reload();
        Logger.WriteLine("Agent: " + command);

        try
        {
            switch (command)
            {
                case AgentCommand.RyzenPower:
                    modeControl.SetRyzenPower(true);
                    break;
                case AgentCommand.Ryzen:
                    modeControl.SetRyzen();
                    break;
                case AgentCommand.AutoRyzen:
                    modeControl.AutoRyzen();
                    if (ModeControl.IsReapplyRyzenRequired())
                        Task.Delay(5000).ContinueWith(_ => { modeControl.AutoRyzen(); modeControl.ReadRyzenLimits(); });
                    break;
                case AgentCommand.ResetRyzen:
                    modeControl.ResetRyzen();
                    break;
                case AgentCommand.CpuTemp:
                    modeControl.AutoCPUTemp();
                    break;
                case AgentCommand.GpuClocks:
                case AgentCommand.GpuClocksReset:
                    // dGPU may have been switched off and on since the last call
                    HardwareControl.RecreateGpuControl();
                    return modeControl.ApplyGPUClocks(command == AgentCommand.GpuClocksReset) is not null;
                case AgentCommand.StopServices:
                    AsusService.StopRunningServices();
                    break;
                case AgentCommand.StartServices:
                    AsusService.StartStoppedServices();
                    break;
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Agent {command} error: {ex.Message}");
            return false;
        }
    }
}
