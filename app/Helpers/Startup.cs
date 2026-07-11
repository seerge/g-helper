using GHelper.Helpers;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

public class Startup
{

    static string taskName = "GHelper";
    static string chargeTaskName = taskName + "Charge";
    static string strExeFilePath = Application.ExecutablePath.Trim();

    private static string CurrentSid => WindowsIdentity.GetCurrent().User!.Value;
    private static string PerUserTaskName => $"{taskName}_{CurrentSid}";

    private static bool Equal(string a, string b) =>
        !string.IsNullOrEmpty(a) && string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static bool IsTaskOwnedByCurrentUser(Microsoft.Win32.TaskScheduler.Task task)
    {
        string name = WindowsIdentity.GetCurrent().Name;
        string sam = name.Contains('\\') ? name.Split('\\').Last() : name;
        var principal = task.Definition.Principal.UserId ?? "";
        return Equal(principal, CurrentSid) || Equal(principal, name) || Equal(principal, sam);
    }

    private static Microsoft.Win32.TaskScheduler.Task? GetCurrentUserTask(TaskService taskService)
    {
        var userTask = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == PerUserTaskName);
        if (userTask != null) return userTask;

        var legacyTask = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
        if (legacyTask != null && IsTaskOwnedByCurrentUser(legacyTask)) return legacyTask;

        return null;
    }

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

                if (taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == chargeTaskName) == null) ScheduleCharge();

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

    public static void Schedule()
    {

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {

            td.RegistrationInfo.Description = "G-Helper Auto Start";
            td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name, Delay = TimeSpan.FromSeconds(1) });
            // ConsoleConnect = fast user switch back; no SessionUnlock, it fires on every unlock
            td.Triggers.Add(new SessionStateChangeTrigger { StateChange = TaskSessionStateChangeType.ConsoleConnect, UserId = WindowsIdentity.GetCurrent().Name, Delay = TimeSpan.FromSeconds(1) });
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
                TaskService.Instance.RootFolder.RegisterTaskDefinition(PerUserTaskName, td);
                Logger.WriteLine($"Startup task scheduled ({PerUserTaskName}): " + strExeFilePath);

                try
                {
                    var legacy = TaskService.Instance.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
                    if (legacy != null && IsTaskOwnedByCurrentUser(legacy))
                        TaskService.Instance.RootFolder.DeleteTask(taskName);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Can't remove legacy startup task: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Can't create startup task: " + ex.Message);
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }
        }

        ScheduleCharge();

    }

    public static void UnSchedule()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                if (taskService.RootFolder.AllTasks.Any(t => t.Name == PerUserTaskName))
                    taskService.RootFolder.DeleteTask(PerUserTaskName);

                var legacyTask = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
                if (legacyTask != null && IsTaskOwnedByCurrentUser(legacyTask))
                    taskService.RootFolder.DeleteTask(taskName);
            }
            catch (Exception)
            {
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't remove task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }
        }

        UnscheduleCharge();
    }
}
