using GHelper.Display;
using GHelper.Helpers;
using GHelper.Mode;

namespace GHelper.Battery
{
    public static class BatteryOptimizationService
    {
        private static long _lastCheckTimestamp;

        public static List<string> GetUnoptimizedSettings()
        {
            var issues = new List<string>();

            // 1. Performance mode is Turbo
            try
            {
                if (Modes.GetCurrentBase() == AsusACPI.PerformanceTurbo)
                    issues.Add("Performance: Turbo");
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Turbo: {ex.Message}"); }

            // 2. Discrete GPU active without auto-switching
            try
            {
                if (Program.acpi.DeviceGet(AsusACPI.GPUEco) >= 0
                    && !AppConfig.Is("gpu_auto")
                    && AppConfig.Get("gpu_mode") != AsusACPI.GPUModeEco)
                {
                    issues.Add("Discrete GPU active");
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck GPU: {ex.Message}"); }

            // 3. High refresh rate without auto-switching
            try
            {
                if (!AppConfig.Is("screen_auto"))
                {
                    var laptopScreen = ScreenNative.FindLaptopScreen();
                    if (laptopScreen != null)
                    {
                        int rate = ScreenNative.GetRefreshRate(laptopScreen);
                        if (rate > 60)
                            issues.Add($"Display: {rate}Hz");
                    }
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Screen: {ex.Message}"); }

            // 4. CPU boost enabled for current mode
            try
            {
                if (AppConfig.GetMode("auto_boost") > 0)
                    issues.Add("CPU boost enabled");
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Boost: {ex.Message}"); }

            // 5. Anime Matrix/Slash running without auto
            try
            {
                if ((AppConfig.IsAnimeMatrix() || AppConfig.IsSlash())
                    && !AppConfig.Is("matrix_auto")
                    && AppConfig.Get("matrix_running") > 0)
                {
                    issues.Add("Anime Matrix active");
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Matrix: {ex.Message}"); }

            // 6. Overdrive enabled
            try
            {
                if (Program.acpi.IsOverdriveSupported()
                    && !AppConfig.IsNoOverdrive()
                    && Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive) == 1)
                {
                    issues.Add("Screen overdrive on");
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Overdrive: {ex.Message}"); }

            // 7. Keyboard backlight has no timeout
            try
            {
                if (AppConfig.Get("keyboard_timeout") <= 0)
                    issues.Add("Keyboard backlight always on");
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOptCheck Keyboard: {ex.Message}"); }

            return issues;
        }

        public static void ApplyBatteryOptimizations()
        {
            try
            {
                // 1. Switch to Silent mode (mode 2) or user's configured battery mode
                int batteryMode = AppConfig.Get("performance_0", 2);
                if (batteryMode < 0) batteryMode = 2;
                Program.modeControl.SetPerformanceMode(batteryMode);
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Mode: {ex.Message}"); }

            try
            {
                // 2. Enable GPU auto-switching and switch to Eco
                if (Program.acpi.DeviceGet(AsusACPI.GPUEco) >= 0)
                {
                    AppConfig.Set("gpu_auto", 1);
                    Program.gpuControl.AutoGPUMode();
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt GPU: {ex.Message}"); }

            try
            {
                // 3. Enable screen auto-switching (sets 60Hz on battery)
                AppConfig.Set("screen_auto", 1);
                ScreenControl.AutoScreen();
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Screen: {ex.Message}"); }

            try
            {
                // 4. Disable CPU boost
                AppConfig.SetMode("auto_boost", 0);
                PowerNative.SetCPUBoost(0);
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Boost: {ex.Message}"); }

            try
            {
                // 5. Enable matrix auto (if device has Anime Matrix/Slash)
                if (AppConfig.IsAnimeMatrix() || AppConfig.IsSlash())
                    AppConfig.Set("matrix_auto", 1);
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Matrix: {ex.Message}"); }

            try
            {
                // 6. Enable no_overdrive (if supported)
                if (Program.acpi.IsOverdriveSupported())
                {
                    AppConfig.Set("no_overdrive", 1);
                    ScreenControl.AutoScreen(true);
                }
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Overdrive: {ex.Message}"); }

            try
            {
                // 7. Set keyboard timeout to 60s if currently 0
                if (AppConfig.Get("keyboard_timeout") <= 0)
                    AppConfig.Set("keyboard_timeout", 60);
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt Keyboard: {ex.Message}"); }

            // Refresh settings UI
            try
            {
                Program.settingsForm.Invoke(delegate
                {
                    Program.settingsForm.VisualiseGPUMode();
                    Program.settingsForm.ShowMode(Modes.GetCurrent());
                });
            }
            catch (Exception ex) { Logger.WriteLine($"BatteryOpt UI Refresh: {ex.Message}"); }
        }

        public static void CheckAndNotify(bool isStartup = false)
        {
            try
            {
                // Guard: feature disabled
                if (!AppConfig.IsNotFalse("battery_remind")) return;

                // Guard: plugged in
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online) return;

                // Guard: power events disabled
                if (!isStartup && AppConfig.Is("disable_power_event")) return;

                // Debounce: 10 seconds
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (Math.Abs(now - _lastCheckTimestamp) < 10000) return;
                _lastCheckTimestamp = now;

                var issues = GetUnoptimizedSettings();
                if (issues.Count == 0) return;

                Logger.WriteLine($"Battery optimization: {issues.Count} issues found");

                if (AppConfig.Is("battery_auto_optimize"))
                {
                    ApplyBatteryOptimizations();
                    Program.toast.RunToast("Switched to battery-optimized mode");
                    return;
                }

                // Show reminder on UI thread
                if (isStartup)
                    Thread.Sleep(2000);

                Program.settingsForm.Invoke(delegate
                {
                    BatteryReminderForm.ShowReminder(issues);
                });
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"BatteryOptCheck: {ex.Message}");
            }
        }
    }
}
