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

    public static bool IsScheduled()
    {
        try
        {
            using (TaskService taskService = new TaskService())
                return (taskService.RootFolder.AllTasks.Any(t => t.Name == taskName));
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
            var task = taskService.RootFolder.AllTasks.FirstOrDefault(t => t.Name == taskName);
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
            td.Actions.Add(strExeFilePath, "charge");

            td.Principal.RunLevel = TaskRunLevel.LUA;
            td.Principal.LogonType = TaskLogonType.S4U;
            td.Principal.UserId = WindowsIdentity.GetCurrent().Name;

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
            td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name, Delay = TimeSpan.FromSeconds(2) });
            td.Actions.Add(strExeFilePath);

            if (ProcessHelper.IsUserAdministrator())
                td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            try
            {
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
            }
            catch (Exception e)
            {
                if (ProcessHelper.IsUserAdministrator())
                    MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else
                    ProcessHelper.RunAsAdmin();
            }

            Logger.WriteLine("Startup task scheduled: " + strExeFilePath);
        }

        ScheduleCharge();

    }

    public static void UnSchedule()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                taskService.RootFolder.DeleteTask(taskName);
            }
            catch (Exception e)
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
