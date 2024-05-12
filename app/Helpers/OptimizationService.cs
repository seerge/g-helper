using System.Diagnostics;

namespace GHelper.Helpers
{
    public static class OptimizationService
    {

        static List<string> services = new() {
                "ArmouryCrateControlInterface",
                "AsHidService",
                "ASUSOptimization",
                "AsusAppService",
                "ASUSLinkNear",
                "ASUSLinkRemote",
                "ASUSSoftwareManager",
                "ASUSLiveUpdateAgent",
                "ASUSSwitch",
                "ASUSSystemAnalysis",
                "ASUSSystemDiagnosis",
                "AsusCertService"
        };

        static List<string> processesAC = new() {
                "ArmouryCrateSE.Service",
                "ArmouryCrate.Service",
                "LightingService",
        };

        static List<string> servicesAC = new() {
                "ArmouryCrateSEService",
                "ArmouryCrateService",
                "LightingService",
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

            if (AppConfig.IsStopAC())
                foreach (string service in processesAC)
                {
                    if (Process.GetProcessesByName(service).Count() > 0)
                    {
                        count++;
                        Logger.WriteLine(service);
                    }
                }

            return count;
        }


        public static void StopAsusServices()
        {
            foreach (string service in services)
            {
                ProcessHelper.StopDisableService(service);
            }

            if (AppConfig.IsStopAC())
            {
                foreach (string service in servicesAC)
                {
                    ProcessHelper.StopDisableService(service, "Manual");
                }
                Thread.Sleep(1000);
            }

        }

        public static void StartAsusServices()
        {
            foreach (string service in services)
            {
                ProcessHelper.StartEnableService(service);
            }

            if (AppConfig.IsStopAC())
            {
                foreach (string service in servicesAC)
                {
                    ProcessHelper.StartEnableService(service);
                }
                Thread.Sleep(1000);
            }

        }

    }

}