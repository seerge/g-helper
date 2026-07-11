using GHelper.Ally;
using System.Diagnostics;
using System.Management;

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


        private static Dictionary<string, string> GetServiceStates()
        {
            var names = AppConfig.IsStopAC() ? services.Concat(servicesAC) : services;
            var states = new Dictionary<string, string>();
            try
            {
                string filter = string.Join(" OR ", names.Select(name => $"Name='{name}'"));
                using var searcher = new ManagementObjectSearcher($"SELECT Name, State FROM Win32_Service WHERE {filter}");
                foreach (ManagementObject mo in searcher.Get())
                    states[(string)mo["Name"]] = (string)mo["State"];
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
            return states;
        }

        private static List<string> GetRunningServices()
        {
            return GetServiceStates().Where(s => s.Value != "Stopped").Select(s => s.Key).ToList();
        }

        public static int GetRunningCount()
        {
            return GetRunningServices().Count;
        }


        public static void StopAsusServices()
        {
            foreach (string service in GetRunningServices())
            {
                ProcessHelper.StopDisableService(service, servicesAC.Contains(service) ? "Manual" : "Disabled");
            }

            if (GetRunningCount() == 0) AppConfig.Set("services_disabled", 1);

            if (AppConfig.IsAlly()) AllyControl.ApplyMode((ControllerMode)AppConfig.Get("controller_mode", (int)ControllerMode.Auto), true);
        }

        public static void StartAsusServices()
        {
            AppConfig.Set("services_disabled", 0);
            foreach (string service in GetServiceStates().Keys)
            {
                ProcessHelper.StartEnableService(service);
            }
        }

        public static void StopOnStartup()
        {
            if (AppConfig.Is("services_skip")) return;
            if (!AppConfig.Is("services_disabled") || !ProcessHelper.IsUserAdministrator()) return;
            if (GetRunningCount() == 0) return;

            Logger.WriteLine("ASUS services revived, re-stopping on startup");
            Task.Run(() => StopAsusServices());
        }

    }

}