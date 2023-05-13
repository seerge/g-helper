﻿using GHelper;
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

    public static void ReScheduleAdmin()
    {
        if (Program.IsUserAdministrator() && IsScheduled())
        {
            UnSchedule();
            Schedule();
        }
    }

    public static void Schedule()
    {

        string strExeFilePath = Application.ExecutablePath;

        if (strExeFilePath is null) return;

        var userId = WindowsIdentity.GetCurrent().Name;

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {
            td.RegistrationInfo.Description = "G-Helper Auto Start";
            td.Triggers.Add(new LogonTrigger { UserId = userId });
            td.Actions.Add(strExeFilePath);

            if (Program.IsUserAdministrator()) 
                td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            Debug.WriteLine(strExeFilePath);
            Debug.WriteLine(userId);

            try
            {
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
            }
            catch (Exception e)
            {
                if (Program.IsUserAdministrator())
                    MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else 
                    Program.RunAsAdmin();
            }
        }

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
                if (Program.IsUserAdministrator())
                    MessageBox.Show("Can't remove task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);
                else
                    Program.RunAsAdmin();
            }
        }
    }
}
