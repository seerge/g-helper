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
                "ASUSXGMobileService",
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

        private static bool IsRunning(string name)
        {
            var procs = Process.GetProcessesByName(name);
            try { return procs.Length > 0; }
            finally { foreach (var p in procs) p.Dispose(); }
        }

        public static bool IsAsusOptimizationRunning() => IsRunning("AsusOptimization");

        public static bool IsArmouryRunning()
        {
            var acService = IsRunning("ArmouryCrate.Service");
            var lightingService = IsRunning("LightingService");
            Logger.WriteLine($"AC Service: {acService}, Lighting Service: {lightingService}");
            return acService || lightingService;
        }

        public static void RunArmouryUninstaller()
        {
            Process.Start(new ProcessStartInfo("https://dlcdnets.asus.com/pub/ASUS/mb/14Utilities/Armoury_Crate_Uninstall_Tool.zip") { UseShellExecute = true });
        }

        public static bool IsOSDRunning() => IsRunning("AsusOSD");


        public static int GetRunningCount()
        {
            int count = 0;
            foreach (string service in services)
            {
                if (IsRunning(service)) count++;
            }

            if (AppConfig.IsStopAC())
                foreach (string service in processesAC)
                {
                    if (IsRunning(service))
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