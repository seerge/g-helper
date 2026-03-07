using System.Diagnostics;

namespace GHelper.Helpers
{
    public static class AsusService
    {

        static List<string> services = new() {
                "ArmouryCrateControlInterface",
                "ArmouryCrateProArtService",
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

        //"AsusPTPService",

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

        public static bool IsAsusOptimizationRunning()
        {
            return Process.GetProcessesByName("AsusOptimization").Length > 0;
        }

        public static bool IsArmouryRunning()
        {
            var acService = Process.GetProcessesByName("ArmouryCrate.Service").Length > 0;
            var lightingService = Process.GetProcessesByName("LightingService").Length > 0;   
            Logger.WriteLine($"AC Service: {acService}, Lighting Service: {lightingService}");
            return acService || lightingService;
        }

        public static void RunArmouryUninstaller()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\ASUS\Armoury Crate Service\UninstallTool\Armoury Crate Uninstall Tool.exe",
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Logger.WriteLine("Failed to launch Armoury Crate uninstaller.");
            }
        }

        public static bool IsOSDRunning()
        {
            return Process.GetProcessesByName("AsusOSD").Length > 0;
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