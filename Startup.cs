using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Security.Principal;

public class Startup
{

    static string taskName = "GHelper";

    public static bool IsScheduled()
    {
        TaskService taskService = new TaskService();
        return (taskService.RootFolder.AllTasks.Any(t => t.Name == taskName));
    }

    public static void Schedule()
    {

        string strExeFilePath = Application.ExecutablePath;

        if (strExeFilePath is null) return;

        var userId = WindowsIdentity.GetCurrent().Name;

        TaskDefinition td = TaskService.Instance.NewTask();
        td.RegistrationInfo.Description = "GHelper Auto Start";
        td.Triggers.Add(new LogonTrigger { UserId = userId });
        td.Actions.Add(strExeFilePath);

        td.Settings.StopIfGoingOnBatteries = false;
        td.Settings.DisallowStartIfOnBatteries = false;

        Debug.WriteLine(strExeFilePath);
        Debug.WriteLine(userId);

        try
        {
            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
        } catch
        {
            MessageBox.Show("Can't schedule task", "Scheduler Error", MessageBoxButtons.OK);
        }

    }

    public static void UnSchedule()
    {
        TaskService taskService = new TaskService();
        taskService.RootFolder.DeleteTask(taskName);
    }
}
