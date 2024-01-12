using System.Diagnostics;

namespace GHelper.Helpers
{
    public static class OptimizationService
    {

        static List<string> services = new() {
                "ArmouryCrateControlInterface",
                "ASUSOptimization",
                "AsusAppService",
                "ASUSLinkNear",
                "ASUSLinkRemote",
                "ASUSSoftwareManager",
                "ASUSSwitch",
                "ASUSSystemAnalysis",
                "ASUSSystemDiagnosis",
                "AsusCertService"
        };

        public static bool IsRunning()
        {
            return Process.GetProcessesByName("AsusOptimization").Count() > 0;
        }

        public static bool IsOSDRunning()
        {
            return Process.GetProcessesByName("AsusOSD").Count() > 0;
        }


        public static int GetRunningCount()
        {
            int count = 0;
            foreach (string service in services)
            {
                if (Process.GetProcessesByName(service).Count() > 0) count++;
            }
            return count;
        }


        public static void StopAsusServices()
        {
            foreach (string service in services)
            {
                ProcessHelper.StopDisableService(service);
            }
        }

        public static void StartAsusServices()
        {
            foreach (string service in services)
            {
                ProcessHelper.StartEnableService(service);
            }
        }

    }

}