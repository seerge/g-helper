using GHelper.Helpers;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

public class Startup
{

    static string taskName = "GHelper";
    static string chargeTaskName = taskName + "Charge";
    static string strExeFilePath = Application.ExecutablePath.Trim();

    // Tasks belong to the signed-in user, even when UAC elevated this process with an admin account
    static (SecurityIdentifier Sid, string Name) sessionUser = ProcessHelper.GetSessionUser();
    static string userTaskName = taskName + "_" + sessionUser.Sid.Value;
    public static string agentTaskName = taskName + "Agent_" + sessionUser.Sid.Value;

    static Microsoft.Win32.TaskScheduler.Task? GetUserTask(TaskService taskService)
    {
        try
        {
            var task = taskService.GetTask(userTaskName);
            if (task != null) return task;

            var legacy = taskService.GetTask(taskName);
            var owner = legacy?.Definition.Principal.UserId ?? "";
            if (owner == sessionUser.Sid.Value ||
                string.Equals(owner.Split('\\').Last(), sessionUser.Name.Split('\\').Last(), StringComparison.OrdinalIgnoreCase))
                return legacy;
        }
        catch (Exception e)
        {
            Logger.WriteLine("Can't read startup task: " + e.Message);
        }

        return null;
    }

    public static bool IsScheduled()
    {
        try
        {
            using (TaskService taskService = new TaskService())
                return GetUserTask(taskService) != null;
        }
        catch (Exception e)
        {
            Logger.WriteLine("Can't check startup task status: " + e.Message);
            return false;
        }
    }

    public static void ReScheduleAdmin()
    {
        if (ProcessHelper.IsUserAdministrator() && !ProcessHelper.IsElevatedAsOtherUser() && IsScheduled())
        {
            UnSchedule();
            Schedule();
        }
    }

    public static void StartupCheck()
    {
        using (TaskService taskService = new TaskService())
        {
            var task = GetUserTask(taskService);
            if (task != null)
            {
                try
                {
                    string action = task.Definition.Actions.FirstOrDefault()!.ToString().Trim();
                    bool needsReschedule = false;

                    if (!strExeFilePath.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!File.Exists(action))
                        {
                            Logger.WriteLine("Startup file doesn't exist: " + action);
                            needsReschedule = true;
                        }
                        else
                        {
                            try
                            {
                                var currentVer = Assembly.GetEntryAssembly().GetName().Version;
                                var fv = FileVersionInfo.GetVersionInfo(action).FileVersion.Split('.');
                                var scheduledVer = new Version(
                                    int.Parse(fv[0]),
                                    fv.Length > 1 ? int.Parse(fv[1]) : 0,
                                    fv.Length > 2 ? int.Parse(fv[2]) : 0,
                                    fv.Length > 3 ? int.Parse(fv[3]) : 0
                                );
                                if (currentVer > scheduledVer)
                                {
                                    Logger.WriteLine($"Startup file is older {scheduledVer}, current is {currentVer}");
                                    needsReschedule = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine("Can't compare assembly versions: " + ex.Message);
                            }
                        }
                    }

                    if (needsReschedule)
                    {
                        // No credential prompts at logon, only the user's own task can be updated
                        if (ProcessHelper.IsStandardUser())
                        {
                            Logger.WriteLine("Rescheduling logon task to: " + strExeFilePath + ", turn Run on Startup off and on to update system tasks");
                            RegisterUserTask();
                            return;
                        }
                        if (task.Definition.Principal.RunLevel == TaskRunLevel.Highest && !ProcessHelper.IsUserAdministrator())
                        {
                            ProcessHelper.RunAsAdmin();
                            return;
                        }
                        Logger.WriteLine("Rescheduling to: " + strExeFilePath);
                        UnSchedule();
                        Schedule();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Can't check startup task: {ex.Message}");
                }

                bool canScheduleCharge = !ProcessHelper.IsStandardUser() && (!ProcessHelper.IsElevatedAsOtherUser() || IsProtectedLocation(strExeFilePath, sessionUser.Sid));
                if (canScheduleCharge && taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == chargeTaskName) == null) ScheduleCharge();

            }
        }
    }

    public static void UnscheduleCharge()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                taskService.RootFolder.DeleteTask(chargeTaskName);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Can't remove charge limit task: " + e.Message);
            }
        }
    }

    public static void ScheduleCharge()
    {

        if (strExeFilePath is null) return;

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {
            td.RegistrationInfo.Description = "G-Helper Charge Limit";
            td.Triggers.Add(new BootTrigger());
            td.Triggers.Add(new EventTrigger
            {
                Subscription = "<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-Boot'] and EventID=27]]</Select></Query></QueryList>"
            });
            td.Actions.Add(strExeFilePath, "charge");

            td.Principal.UserId = "SYSTEM";
            td.Principal.LogonType = TaskLogonType.ServiceAccount;
            td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds(30);

            try
            {
                TaskService.Instance.RootFolder.RegisterTaskDefinition(chargeTaskName, td);
                Logger.WriteLine("Charge limit task scheduled: " + strExeFilePath);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Can't create a charge limit task: " + e.Message);
            }
        }
    }

    static void UnscheduleAgent()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                var task = taskService.GetTask(agentTaskName);
                if (task is null) return;
                task.Stop();
                taskService.RootFolder.DeleteTask(agentTaskName, false);
                Logger.WriteLine("Agent task removed");
            }
            catch (Exception e)
            {
                Logger.WriteLine("Can't remove agent task: " + e.Message);
            }
        }
    }

    // SYSTEM side of G-Helper for a standard account, started on demand by its unelevated UI
    static void ScheduleAgent()
    {
        using (TaskDefinition td = TaskService.Instance.NewTask())
        {
            td.RegistrationInfo.Description = "G-Helper Agent";
            td.Actions.Add(strExeFilePath, "agent " + sessionUser.Sid.Value);

            td.Principal.UserId = "SYSTEM";
            td.Principal.LogonType = TaskLogonType.ServiceAccount;
            td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.AllowDemandStart = true;
            td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            try
            {
                // User may read and start the agent, but not change what it runs
                TaskService.Instance.RootFolder.RegisterTaskDefinition(agentTaskName, td, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount, TaskSddl("GRGX"));
                Logger.WriteLine("Agent task scheduled: " + strExeFilePath);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Can't create agent task: " + e.Message);
            }
        }
    }

    static string TaskSddl(string userRights) => $"D:(A;;FA;;;BA)(A;;FA;;;SY)(A;;{userRights};;;{sessionUser.Sid.Value})";

    static bool RegisterUserTask()
    {
        bool forOtherUser = ProcessHelper.IsElevatedAsOtherUser();

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {

            td.RegistrationInfo.Description = "G-Helper Auto Start";
            td.Triggers.Add(new LogonTrigger { UserId = sessionUser.Name, Delay = TimeSpan.FromSeconds(1) });
            // ConsoleConnect = fast user switch back; no SessionUnlock, it fires on every unlock
            td.Triggers.Add(new SessionStateChangeTrigger { StateChange = TaskSessionStateChangeType.ConsoleConnect, UserId = sessionUser.Name, Delay = TimeSpan.FromSeconds(1) });
            td.Actions.Add(strExeFilePath);

            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            if (ProcessHelper.IsUserAdministrator() && !forOtherUser)
                td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            try
            {
                if (forOtherUser)
                    // Runs unelevated as the signed-in user, who can still read and remove it
                    TaskService.Instance.RootFolder.RegisterTaskDefinition(userTaskName, td, TaskCreation.CreateOrUpdate, sessionUser.Name, null, TaskLogonType.InteractiveToken, TaskSddl("FA"));
                else
                    TaskService.Instance.RootFolder.RegisterTaskDefinition(userTaskName, td);
                Logger.WriteLine("Startup task scheduled: " + strExeFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Can't create startup task: " + ex.Message);
                return false;
            }
        }
    }

    const FileSystemRights ReplaceRights = FileSystemRights.Delete | FileSystemRights.ChangePermissions | FileSystemRights.TakeOwnership;
    const int GenericWriteAll = 0x40000000 | 0x10000000;

    // SYSTEM tasks run this exe, so the user must not be able to swap it. Checked when registering, by the agent and by the UI
    public static bool IsProtectedLocation(string exePath, SecurityIdentifier user)
    {
        try
        {
            var file = new FileInfo(exePath);
            if (!IsProtected(file.GetAccessControl(), ReplaceRights | FileSystemRights.WriteData | FileSystemRights.AppendData, user)) return false;

            var dir = file.Directory!;
            if (!IsProtected(dir.GetAccessControl(), ReplaceRights | FileSystemRights.CreateFiles | FileSystemRights.CreateDirectories | FileSystemRights.DeleteSubdirectoriesAndFiles, user)) return false;

            // Renaming any parent folder would allow the same
            for (dir = dir.Parent; dir is not null; dir = dir.Parent)
                if (!IsProtected(dir.GetAccessControl(), ReplaceRights | FileSystemRights.DeleteSubdirectoriesAndFiles, user)) return false;

            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Can't check G-Helper folder permissions: " + ex.Message);
            return false;
        }
    }

    static bool IsProtected(FileSystemSecurity security, FileSystemRights unsafeRights, SecurityIdentifier user)
    {
        int unsafeMask = (int)unsafeRights | GenericWriteAll;

        if (user.Equals(security.GetOwner(typeof(SecurityIdentifier)))) return false;

        // What the user can really do here, group memberships included. No answer counts as unsafe
        FileSystemRights? effective = EffectiveAccess.Get(security, user);
        if (effective is null)
        {
            Logger.WriteLine("Can't compute effective access for " + user.Value);
            return false;
        }
        if (((int)effective.Value & unsafeMask) != 0) return false;

        // Authz only knows group memberships. Logon type SIDs (interactive session, or a scheduled task
        // the user registers for themselves) are added at logon, so scan the ACEs for those by hand
        SecurityIdentifier[] users =
        [
            user,
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
            new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
            new SecurityIdentifier(WellKnownSidType.InteractiveSid, null),
            new SecurityIdentifier(WellKnownSidType.BatchSid, null),
            new SecurityIdentifier(WellKnownSidType.NetworkSid, null),
            new SecurityIdentifier(WellKnownSidType.LocalSid, null),
            new SecurityIdentifier(WellKnownSidType.ThisOrganizationSid, null),
            new SecurityIdentifier("S-1-2-1"),   // CONSOLE LOGON
            new SecurityIdentifier("S-1-5-113"), // Local account
        ];

        foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
        {
            if (rule.AccessControlType != AccessControlType.Allow) continue;
            if (rule.PropagationFlags.HasFlag(PropagationFlags.InheritOnly)) continue;
            if (rule.IdentityReference is not SecurityIdentifier sid || !users.Contains(sid)) continue;
            if (((int)rule.FileSystemRights & unsafeMask) != 0) return false;
        }

        return true;
    }

    public static void Schedule()
    {

        if (ProcessHelper.IsStandardUser())
        {
            // One credential prompt lets an admin set up the logon, charge limit and agent tasks for this account
            bool elevated = ProcessHelper.RunElevatedAndWait("schedule");
            if (!elevated)
            {
                Logger.WriteLine("Elevated scheduling failed, registering logon task only");
                RegisterUserTask();
            }
            PrivilegedAgent.Invalidate();
            return;
        }

        if (!RegisterUserTask())
        {
            if (ProcessHelper.IsUserAdministrator())
                MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
            else
                ProcessHelper.RunAsAdmin();
        }

        if (!ProcessHelper.IsElevatedAsOtherUser())
        {
            ScheduleCharge();
            return;
        }

        if (!IsProtectedLocation(strExeFilePath, sessionUser.Sid))
        {
            Logger.WriteLine("G-Helper folder is writable by " + sessionUser.Name + ", skipping system tasks");
            MessageBox.Show($"G-Helper runs from a folder that {sessionUser.Name} can modify, so admin-only features (CPU undervolt and power limits, GPU clocks, charge limit on boot) can't be enabled safely.\n\nMove GHelper.exe to a folder like C:\\Program Files\\G-Helper and turn Run on Startup on again.", "Run on Startup", MessageBoxButtons.OK);
            return;
        }

        ScheduleCharge();
        ScheduleAgent();

    }

    public static void UnSchedule()
    {
        if (ProcessHelper.IsStandardUser())
        {
            if (!ProcessHelper.RunElevatedAndWait("unschedule"))
            {
                Logger.WriteLine("Elevated unscheduling failed, removing logon task only");
                using (TaskService taskService = new TaskService())
                {
                    try
                    {
                        taskService.RootFolder.DeleteTask(userTaskName, false);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine("Can't remove startup task: " + e.Message);
                    }
                }
            }
            PrivilegedAgent.Invalidate();
            return;
        }

        using (TaskService taskService = new TaskService())
        {
            try
            {
                taskService.RootFolder.DeleteTask(userTaskName, false);
                if (GetUserTask(taskService) != null) taskService.RootFolder.DeleteTask(taskName);
            }
            catch (Exception)
            {
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't remove task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }
        }

        UnscheduleAgent();
        UnscheduleCharge();
    }
}
