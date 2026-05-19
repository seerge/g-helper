using GHelper.Helpers;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

public class Startup
{

    static string taskName = "GHelper";
    static string chargeTaskName = taskName + "Charge";
    static string subFolderName = "GHelper";
    static string strExeFilePath = Application.ExecutablePath.Trim();

    private static string CurrentSid => WindowsIdentity.GetCurrent().User!.Value;
    private static string CurrentUserName => WindowsIdentity.GetCurrent().Name;
    private static string PerUserTaskName => $"{taskName}_{CurrentSid}";

    private static bool Equal(string a, string b) =>
        !string.IsNullOrEmpty(a) && string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static bool IsTaskOwnedByCurrentUser(Microsoft.Win32.TaskScheduler.Task task)
    {
        if (task == null) return false;

        string sid = CurrentSid;
        string name = CurrentUserName;
        string sam = name.Contains('\\') ? name.Split('\\').Last() : name;

        var principal = task.Definition.Principal.UserId ?? "";
        if (!string.IsNullOrEmpty(principal))
            return Equal(principal, sid) || Equal(principal, name) || Equal(principal, sam);

        // Legacy tasks may have empty Principal.UserId — fall back to security descriptor owner
        try
        {
            var sddl = task.GetSecurityDescriptorSddlForm(System.Security.AccessControl.SecurityInfos.Owner);
            if (sddl.StartsWith("O:"))
            {
                var owner = sddl[2..].TrimEnd();
                if (Equal(owner, sid)) return true;
            }
        }
        catch { }

        // Last resort — registration author is auto-populated to DOMAIN\User
        var author = task.Definition.RegistrationInfo.Author ?? "";
        if (!string.IsNullOrEmpty(author))
            return Equal(author, name) || Equal(author, sam);

        return false;
    }

    private static TaskFolder? GetSubFolder(TaskService taskService, bool create = false)
    {
        try
        {
            var folder = taskService.RootFolder.SubFolders.FirstOrDefault(f => f.Name == subFolderName);
            if (folder == null && create) folder = taskService.RootFolder.CreateFolder(subFolderName);
            return folder;
        }
        catch (Exception e)
        {
            Logger.WriteLine("Can't access GHelper task folder: " + e.Message);
            return null;
        }
    }

    private static Microsoft.Win32.TaskScheduler.Task? GetCurrentUserTask(TaskService taskService)
    {
        var folder = GetSubFolder(taskService);
        if (folder != null)
        {
            var userTask = folder.AllTasks.FirstOrDefault(t => t.Name == PerUserTaskName);
            if (userTask != null) return userTask;
        }

        var rootTask = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
        if (rootTask != null && IsTaskOwnedByCurrentUser(rootTask)) return rootTask;

        return null;
    }

    private static bool IsLegacyRootTask(Microsoft.Win32.TaskScheduler.Task task) =>
        task != null && task.Path.TrimStart('\\').Equals(taskName, StringComparison.OrdinalIgnoreCase);

    public static bool IsScheduled()
    {
        try
        {
            using (TaskService taskService = new TaskService())
                return GetCurrentUserTask(taskService) != null;
        }
        catch (Exception e)
        {
            Logger.WriteLine("Can't check startup task status: " + e.Message);
            return false;
        }
    }

    public static void ReScheduleAdmin()
    {
        if (ProcessHelper.IsUserAdministrator() && IsScheduled())
        {
            UnSchedule();
            Schedule();
        }
    }

    public static void StartupCheck()
    {
        using (TaskService taskService = new TaskService())
        {
            var task = GetCurrentUserTask(taskService);
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

                    if (!needsReschedule)
                    {
                        bool hasLogon = task.Definition.Triggers.OfType<LogonTrigger>().Any();
                        bool hasConsoleConnect = task.Definition.Triggers
                            .OfType<SessionStateChangeTrigger>()
                            .Any(t => t.StateChange == TaskSessionStateChangeType.ConsoleConnect);

                        if (!hasLogon || !hasConsoleConnect || IsLegacyRootTask(task))
                        {
                            Logger.WriteLine("Migrating startup task layout/triggers");
                            needsReschedule = true;
                        }
                    }

                    if (needsReschedule)
                    {
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

                var subFolder = GetSubFolder(taskService);
                bool chargeInSubFolder = subFolder != null && subFolder.AllTasks.Any(t => t.Name == chargeTaskName);
                bool chargeAtRoot = taskService.RootFolder.AllTasks.Any(t => t.Name == chargeTaskName);
                if (!chargeInSubFolder || chargeAtRoot) ScheduleCharge();

            }
        }
    }

    public static void UnscheduleCharge()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                var folder = GetSubFolder(taskService);
                if (folder != null && folder.AllTasks.Any(t => t.Name == chargeTaskName))
                    folder.DeleteTask(chargeTaskName);

                if (taskService.RootFolder.AllTasks.Any(t => t.Name == chargeTaskName))
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

        using (TaskService taskService = new TaskService())
        using (TaskDefinition td = taskService.NewTask())
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
                var folder = GetSubFolder(taskService, create: true);
                if (folder == null)
                {
                    Logger.WriteLine("Can't access GHelper task folder for charge task");
                    return;
                }

                folder.RegisterTaskDefinition(chargeTaskName, td);
                Logger.WriteLine("Charge limit task scheduled: " + strExeFilePath);

                var legacy = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == chargeTaskName);
                if (legacy != null)
                {
                    try
                    {
                        taskService.RootFolder.DeleteTask(chargeTaskName);
                        Logger.WriteLine("Removed legacy root GHelperCharge task");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Can't remove legacy root charge task: " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine("Can't create a charge limit task: " + e.Message);
            }
        }
    }

    public static void Schedule()
    {

        using (TaskService taskService = new TaskService())
        using (TaskDefinition td = taskService.NewTask())
        {

            td.RegistrationInfo.Description = "G-Helper Auto Start";

            // LogonTrigger fires on initial boot logon; ConsoleConnect fires on fast-user-switch back.
            // No SessionUnlock — would relaunch on every sleep/wake for the same user.
            td.Triggers.Add(new LogonTrigger
            {
                UserId = CurrentUserName,
                Delay = TimeSpan.FromSeconds(1)
            });
            td.Triggers.Add(new SessionStateChangeTrigger
            {
                StateChange = TaskSessionStateChangeType.ConsoleConnect,
                UserId = CurrentUserName,
                Delay = TimeSpan.FromSeconds(1)
            });

            td.Actions.Add(strExeFilePath);

            td.Principal.UserId = CurrentSid;
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            if (ProcessHelper.IsUserAdministrator())
                td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            try
            {
                var folder = GetSubFolder(taskService, create: true);
                if (folder == null)
                {
                    if (ProcessHelper.IsUserAdministrator())
                        MessageBox.Show("Can't create GHelper task folder in Task Scheduler. Try opening Task Scheduler manually and check permissions on the Task Scheduler Library root.", "Scheduler Error", MessageBoxButtons.OK);
                    else
                        ProcessHelper.RunAsAdmin();
                }
                else
                {
                    folder.RegisterTaskDefinition(PerUserTaskName, td);
                    Logger.WriteLine($"Startup task scheduled ({PerUserTaskName}): " + strExeFilePath);

                    var legacy = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
                    if (legacy != null && IsTaskOwnedByCurrentUser(legacy))
                    {
                        try
                        {
                            taskService.RootFolder.DeleteTask(taskName);
                            Logger.WriteLine("Removed legacy root GHelper task");
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine("Can't remove legacy root task: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.\n\n" + e.Message, "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }
        }

        try
        {
            if (!IsScheduled())
                Logger.WriteLine("Warning: Schedule() returned but IsScheduled() is false");
        }
        catch { }

        ScheduleCharge();

    }

    public static void UnSchedule()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                var folder = GetSubFolder(taskService);
                if (folder != null && folder.AllTasks.Any(t => t.Name == PerUserTaskName))
                {
                    folder.DeleteTask(PerUserTaskName);
                    try
                    {
                        if (!folder.AllTasks.Any() && !folder.SubFolders.Any())
                            taskService.RootFolder.DeleteFolder(subFolderName, false);
                    }
                    catch { }
                }

                var rootTask = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
                if (rootTask != null && IsTaskOwnedByCurrentUser(rootTask))
                    taskService.RootFolder.DeleteTask(taskName);
            }
            catch (Exception e)
            {
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't remove task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.\n\n" + e.Message, "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }
        }

        UnscheduleCharge();
    }
}
