using GHelper.Helpers;
using GHelper.Input;
using GHelper.USB;

namespace GHelper.Input
{
    public class AutoBacklightController
    {
        private static AutoBacklightController? _instance;
        private bool _isEnabled = false;
        private int _lastBacklightLevel = -1;
        private DateTime _lastUserInput = DateTime.Now;
        private System.Timers.Timer? _userInputTimer;
        private int _lastLuxReading = -1;

        // Thresholds for backlight control
        private int _darkThreshold = AppConfig.Get("als_dark_threshold", 50);   // Lux below which to turn on backlight
        private int _brightThreshold = AppConfig.Get("als_bright_threshold", 100); // Lux above which to turn off backlight
        private int _hysteresis = AppConfig.Get("als_hysteresis", 10);          // Prevent rapid switching

        public static AutoBacklightController Instance
        {
            get
            {
                _instance ??= new AutoBacklightController();
                return _instance;
            }
        }

        private AutoBacklightController()
        {
            // Set default configuration values if they don't exist
            if (!AppConfig.Exists("als_dark_threshold"))
                AppConfig.Set("als_dark_threshold", 50);
            if (!AppConfig.Exists("als_bright_threshold"))
                AppConfig.Set("als_bright_threshold", 100);
            if (!AppConfig.Exists("als_hysteresis"))
                AppConfig.Set("als_hysteresis", 10);
            if (!AppConfig.Exists("als_min_backlight"))
                AppConfig.Set("als_min_backlight", 1);
            if (!AppConfig.Exists("als_max_backlight"))
                AppConfig.Set("als_max_backlight", 3);
            if (!AppConfig.Exists("als_input_delay"))
                AppConfig.Set("als_input_delay", 10000);
            
            // Simple setting for initial low brightness when turning on in dark
            if (!AppConfig.Exists("als_initial_low_brightness"))
                AppConfig.Set("als_initial_low_brightness", 1);    // Enable low initial brightness

            // Load settings
            _darkThreshold = AppConfig.Get("als_dark_threshold", 50);
            _brightThreshold = AppConfig.Get("als_bright_threshold", 100);
            _hysteresis = AppConfig.Get("als_hysteresis", 10);

            // Timer to track user input activity
            _userInputTimer = new System.Timers.Timer(30000); // 30 seconds
            _userInputTimer.Elapsed += OnUserInputTimerElapsed;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;

                _isEnabled = value;
                AppConfig.Set("auto_backlight_enabled", value ? 1 : 0);

                if (_isEnabled)
                {
                    StartAutoBacklight();
                }
                else
                {
                    StopAutoBacklight();
                }

                Logger.WriteLine($"Auto backlight controller {(value ? "enabled" : "disabled")}");
            }
        }

        public void Initialize()
        {
            // Load settings
            _isEnabled = AppConfig.Is("auto_backlight_enabled");
            _darkThreshold = AppConfig.Get("als_dark_threshold", 50);
            _brightThreshold = AppConfig.Get("als_bright_threshold", 100);
            _hysteresis = AppConfig.Get("als_hysteresis", 10);

            Logger.WriteLine($"Auto backlight controller initializing - Enabled: {_isEnabled}");

            // Initialize ambient light sensor
            try
            {
                AmbientLightSensor.Initialize();
                
                // Subscribe to sensor events
                AmbientLightSensor.LightLevelChanged += OnLightLevelChanged;
                
                if (AmbientLightSensor.IsAvailable())
                {
                    Logger.WriteLine("Ambient light sensor available and connected");
                }
                else
                {
                    Logger.WriteLine("No hardware sensor detected, using time-based simulation");
                }
                
                if (_isEnabled)
                {
                    StartAutoBacklight();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to initialize ambient light sensor: {ex.Message}");
                // Continue without sensor support
            }
        }

        private void StartAutoBacklight()
        {
            AmbientLightSensor.Start();
            _userInputTimer?.Start();
            Logger.WriteLine("Auto backlight started");

            // Perform initial adjustment after a short delay
            Task.Run(async () =>
            {
                await Task.Delay(1000); // Wait 1 second for sensor to stabilize
                var currentLux = AmbientLightSensor.GetCurrentLux();
                if (currentLux >= 0)
                {
                    AdjustBacklightBasedOnAmbientLight(currentLux, true);
                }
                else
                {
                    Logger.WriteLine("No initial lux reading available");
                }
            });
        }

        private void StopAutoBacklight()
        {
            AmbientLightSensor.Stop();
            _userInputTimer?.Stop();
            Logger.WriteLine("Auto backlight stopped");
        }

        private void OnLightLevelChanged(object? sender, LightLevelChangedEventArgs e)
        {
            if (!_isEnabled) return;

            // Only adjust if there hasn't been recent user input
            var timeSinceInput = DateTime.Now - _lastUserInput;
            var inputDelay = AppConfig.Get("als_input_delay", 10000); // 10 seconds default

            if (timeSinceInput.TotalMilliseconds > inputDelay)
            {
                AdjustBacklightBasedOnAmbientLight(e.CurrentLux);
            }
            else
            {
                Logger.WriteLine($"Skipping auto backlight adjustment due to recent user input ({timeSinceInput.TotalSeconds:F1}s ago)");
            }
        }

        private void AdjustBacklightBasedOnAmbientLight(int luxValue, bool isInitial = false)
        {
            try
            {
                var currentBacklight = InputDispatcher.GetBacklight();
                var targetBacklight = currentBacklight;
                
                // Store last reading for hysteresis calculations
                _lastLuxReading = luxValue;

                // Determine target backlight level based on ambient light with hysteresis
                bool shouldTurnOn = luxValue <= (_darkThreshold - _hysteresis);
                bool shouldTurnOff = luxValue >= (_brightThreshold + _hysteresis);
                
                Logger.WriteLine($"ALS: {luxValue} lux, Current backlight: {currentBacklight}, Dark threshold: {_darkThreshold}, Bright threshold: {_brightThreshold}");

                if (shouldTurnOn && currentBacklight == 0)
                {
                    // Dark environment - turn on backlight
                    var minBacklight = AppConfig.Get("als_min_backlight", 1);
                    var maxBacklight = AppConfig.Get("als_max_backlight", 3);
                    var useInitialLowBrightness = AppConfig.Is("als_initial_low_brightness");
                    
                    if (useInitialLowBrightness)
                    {
                        // Start with low brightness level for comfort
                        targetBacklight = minBacklight;
                        Logger.WriteLine($"Dark detected ({luxValue} ≤ {_darkThreshold - _hysteresis}), turning backlight ON to low level {targetBacklight}");
                    }
                    else
                    {
                        // Original behavior - scale backlight based on darkness
                        var darknessRatio = Math.Max(0, Math.Min(1, (double)(_darkThreshold - luxValue) / _darkThreshold));
                        targetBacklight = Math.Max(minBacklight, (int)(minBacklight + (maxBacklight - minBacklight) * darknessRatio));
                        Logger.WriteLine($"Dark detected ({luxValue} ≤ {_darkThreshold - _hysteresis}), turning backlight ON to level {targetBacklight}");
                    }
                }
                else if (shouldTurnOff && currentBacklight > 0)
                {
                    // Bright environment - turn off backlight
                    targetBacklight = 0;
                    Logger.WriteLine($"Bright detected ({luxValue} ≥ {_brightThreshold + _hysteresis}), turning backlight OFF");
                }
                else
                {
                    // In hysteresis zone or no change needed
                    Logger.WriteLine($"No backlight change needed (hysteresis zone or already correct)");
                    return; // Early return to avoid unnecessary processing
                }

                // Apply change if different
                if (targetBacklight != currentBacklight)
                {
                    var reason = isInitial ? "Initial ALS" : $"ALS ({luxValue} lux)";
                    Logger.WriteLine($"Auto backlight: {currentBacklight} → {targetBacklight} ({reason})");

                    // Update the appropriate config based on power state
                    bool onBattery = System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus != System.Windows.Forms.PowerLineStatus.Online;
                    if (onBattery)
                        AppConfig.Set("keyboard_brightness_ac", targetBacklight);
                    else
                        AppConfig.Set("keyboard_brightness", targetBacklight);

                    // Apply brightness change through the standard Aura system
                    Aura.ApplyBrightness(targetBacklight, reason);
                    
                    // Store the level for tracking
                    _lastBacklightLevel = targetBacklight;

                    // Show toast notification if enabled
                    if (AppConfig.Is("als_show_notifications"))
                    {
                        var message = targetBacklight == 0 ? "Backlight Off (Bright)" : $"Backlight {targetBacklight} (Dark)";
                        Program.toast.RunToast(message, targetBacklight > 0 ? ToastIcon.BacklightUp : ToastIcon.BacklightDown);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error in AdjustBacklightBasedOnAmbientLight: {ex.Message}");
            }
        }

        public void NotifyUserInput()
        {
            _lastUserInput = DateTime.Now;
            _userInputTimer?.Stop();
            _userInputTimer?.Start();
            
            Logger.WriteLine("User input detected - pausing auto backlight adjustments");
        }

        private void OnUserInputTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // User input timer elapsed - can resume auto adjustments
            Logger.WriteLine("User input cooldown expired, resuming auto backlight");
            
            // Trigger immediate re-evaluation if we have a recent lux reading
            if (_lastLuxReading >= 0)
            {
                Task.Run(() => AdjustBacklightBasedOnAmbientLight(_lastLuxReading));
            }
        }

        public void UpdateSettings()
        {
            _darkThreshold = AppConfig.Get("als_dark_threshold", 50);
            _brightThreshold = AppConfig.Get("als_bright_threshold", 100);
            _hysteresis = AppConfig.Get("als_hysteresis", 10);

            Logger.WriteLine($"Auto backlight settings updated: Dark≤{_darkThreshold}, Bright≥{_brightThreshold}, Hysteresis={_hysteresis}");
            
            // Re-evaluate current conditions with new settings
            if (_isEnabled && _lastLuxReading >= 0)
            {
                Task.Run(() => AdjustBacklightBasedOnAmbientLight(_lastLuxReading));
            }
        }

        public Dictionary<string, object> GetStatus()
        {
            return new Dictionary<string, object>
            {
                ["enabled"] = _isEnabled,
                ["sensor_available"] = AmbientLightSensor.IsAvailable(),
                ["sensor_active"] = AmbientLightSensor.IsSensorActive(),
                ["current_lux"] = AmbientLightSensor.GetCurrentLux(),
                ["current_backlight"] = InputDispatcher.GetBacklight(),
                ["dark_threshold"] = _darkThreshold,
                ["bright_threshold"] = _brightThreshold,
                ["hysteresis"] = _hysteresis,
                ["initial_low_brightness"] = AppConfig.Is("als_initial_low_brightness"),
                ["last_user_input"] = _lastUserInput,
                ["last_lux_reading"] = _lastLuxReading,
                ["last_sensor_reading"] = AmbientLightSensor.GetLastReadingTime()
            };
        }

        public void Dispose()
        {
            StopAutoBacklight();
            _userInputTimer?.Dispose();
            AmbientLightSensor.LightLevelChanged -= OnLightLevelChanged;
        }
    }
}