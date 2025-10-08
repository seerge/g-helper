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
        
        // Pattern-based detection: track light level history
        private readonly Queue<LightLevelReading> _lightLevelHistory = new Queue<LightLevelReading>();
        private const int MAX_HISTORY_SIZE = 10; // Track last 10 readings
        private const int MIN_READINGS_FOR_PATTERN = 3; // Need at least 3 readings to detect pattern

        // Configuration for pattern detection
        private int _patternWindowSeconds = AppConfig.Get("als_pattern_window", 30); // Time window for pattern analysis
        private int _darkeningThreshold = AppConfig.Get("als_darkening_threshold", -30); // Lux decrease to trigger backlight on
        private int _brighteningThreshold = AppConfig.Get("als_brightening_threshold", 40); // Lux increase to trigger backlight off
        private double _patternConfidence = AppConfig.Get("als_pattern_confidence", 70) / 100.0; // 70% confidence required
        
        // Absolute light level thresholds - prevent backlight in well-lit conditions
        private int _absoluteDarkThreshold = AppConfig.Get("als_absolute_dark", 20); // Must be below this to turn ON
        private int _absoluteBrightThreshold = AppConfig.Get("als_absolute_bright", 80); // Must be above this to turn OFF

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
            if (!AppConfig.Exists("als_pattern_window"))
                AppConfig.Set("als_pattern_window", 30);
            if (!AppConfig.Exists("als_darkening_threshold"))
                AppConfig.Set("als_darkening_threshold", -30);
            if (!AppConfig.Exists("als_brightening_threshold"))
                AppConfig.Set("als_brightening_threshold", 40);
            if (!AppConfig.Exists("als_pattern_confidence"))
                AppConfig.Set("als_pattern_confidence", 70);
            if (!AppConfig.Exists("als_absolute_dark"))
                AppConfig.Set("als_absolute_dark", 20);
            if (!AppConfig.Exists("als_absolute_bright"))
                AppConfig.Set("als_absolute_bright", 80);
            if (!AppConfig.Exists("als_min_backlight"))
                AppConfig.Set("als_min_backlight", 1);
            if (!AppConfig.Exists("als_max_backlight"))
                AppConfig.Set("als_max_backlight", 3);
            if (!AppConfig.Exists("als_input_delay"))
                AppConfig.Set("als_input_delay", 10000);
            if (!AppConfig.Exists("als_initial_low_brightness"))
                AppConfig.Set("als_initial_low_brightness", 1);

            // Load settings
            _patternWindowSeconds = AppConfig.Get("als_pattern_window", 30);
            _darkeningThreshold = AppConfig.Get("als_darkening_threshold", -30);
            _brighteningThreshold = AppConfig.Get("als_brightening_threshold", 40);
            _patternConfidence = AppConfig.Get("als_pattern_confidence", 70) / 100.0;
            _absoluteDarkThreshold = AppConfig.Get("als_absolute_dark", 20);
            _absoluteBrightThreshold = AppConfig.Get("als_absolute_bright", 80);

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
            _patternWindowSeconds = AppConfig.Get("als_pattern_window", 30);
            _darkeningThreshold = AppConfig.Get("als_darkening_threshold", -30);
            _brighteningThreshold = AppConfig.Get("als_brightening_threshold", 40);
            _patternConfidence = AppConfig.Get("als_pattern_confidence", 70) / 100.0;
            _absoluteDarkThreshold = AppConfig.Get("als_absolute_dark", 20);
            _absoluteBrightThreshold = AppConfig.Get("als_absolute_bright", 80);

            Logger.WriteLine($"Auto backlight controller initializing - Enabled: {_isEnabled}");
            Logger.WriteLine($"Thresholds: Absolute dark ≤{_absoluteDarkThreshold} lux, Absolute bright ≥{_absoluteBrightThreshold} lux");

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
            _lightLevelHistory.Clear(); // Clear history on start
            AmbientLightSensor.Start();
            _userInputTimer?.Start();
            Logger.WriteLine("Auto backlight started with pattern-based detection");

            // Perform initial adjustment after a short delay
            Task.Run(async () =>
            {
                await Task.Delay(1000); // Wait 1 second for sensor to stabilize
                var currentLux = AmbientLightSensor.GetCurrentLux();
                if (currentLux >= 0)
                {
                    // Add initial reading to history
                    AddLightLevelReading(currentLux, DateTime.Now);
                    
                    // On initial startup, use simple threshold check
                    PerformInitialBacklightCheck(currentLux);
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
            _lightLevelHistory.Clear();
            Logger.WriteLine("Auto backlight stopped");
        }

        private void OnLightLevelChanged(object? sender, LightLevelChangedEventArgs e)
        {
            if (!_isEnabled) return;

            // Add new reading to history
            AddLightLevelReading(e.CurrentLux, e.Timestamp);

            // Only adjust if there hasn't been recent user input
            var timeSinceInput = DateTime.Now - _lastUserInput;
            var inputDelay = AppConfig.Get("als_input_delay", 10000); // 10 seconds default

            if (timeSinceInput.TotalMilliseconds > inputDelay)
            {
                AdjustBacklightBasedOnPattern();
            }
            else
            {
                Logger.WriteLine($"Skipping auto backlight adjustment due to recent user input ({timeSinceInput.TotalSeconds:F1}s ago)");
            }
        }

        private void AddLightLevelReading(int luxValue, DateTime timestamp)
        {
            // Add new reading
            _lightLevelHistory.Enqueue(new LightLevelReading
            {
                Lux = luxValue,
                Timestamp = timestamp
            });

            // Remove old readings outside the pattern window
            var cutoffTime = DateTime.Now.AddSeconds(-_patternWindowSeconds);
            while (_lightLevelHistory.Count > 0 && _lightLevelHistory.Peek().Timestamp < cutoffTime)
            {
                _lightLevelHistory.Dequeue();
            }

            // Also enforce maximum history size
            while (_lightLevelHistory.Count > MAX_HISTORY_SIZE)
            {
                _lightLevelHistory.Dequeue();
            }
        }

        private void AdjustBacklightBasedOnPattern()
        {
            try
            {
                // Need enough readings to detect a pattern
                if (_lightLevelHistory.Count < MIN_READINGS_FOR_PATTERN)
                {
                    Logger.WriteLine($"Not enough readings for pattern detection ({_lightLevelHistory.Count}/{MIN_READINGS_FOR_PATTERN})");
                    return;
                }

                var currentBacklight = InputDispatcher.GetBacklight();
                var currentLux = AmbientLightSensor.GetCurrentLux();

                // --- NEW FAILSAFE LOGIC ---
                // If the backlight is on in a room that is already bright, turn it off.
                // This handles cases where the initial brightening pattern was missed.
                if (currentBacklight > 0 && currentLux >= _absoluteBrightThreshold)
                {
                    var recentReadings = _lightLevelHistory.ToArray();
                    // Ensure at least 75% of recent readings are also bright to avoid acting on a single spike.
                    var brightReadings = recentReadings.Count(r => r.Lux >= _absoluteBrightThreshold - 10); // Use a small tolerance
                    if (brightReadings >= recentReadings.Length * 0.75)
                    {
                        Logger.WriteLine($"Failsafe: Backlight is on but environment is bright ({currentLux} >= {_absoluteBrightThreshold} lux). Turning off.");
                        Aura.ApplyBrightness(0, "Failsafe Bright");
                        // Update config
                        bool onBattery = System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus != System.Windows.Forms.PowerLineStatus.Online;
                        if (onBattery)
                            AppConfig.Set("keyboard_brightness_ac", 0);
                        else
                            AppConfig.Set("keyboard_brightness", 0);
                        
                        _lastBacklightLevel = 0;
                        return; // Exit after applying failsafe
                    }
                }
                // --- END OF FAILSAFE LOGIC ---

                var pattern = AnalyzeLightPattern();

                if (pattern == null)
                {
                    Logger.WriteLine("No clear pattern detected");
                    return;
                }

                Logger.WriteLine($"Pattern detected: Trend={pattern.Trend}, Change={pattern.TotalChange:F1} lux, " +
                                $"Confidence={pattern.Confidence:P0}, Current={currentLux} lux, Backlight={currentBacklight}");

                int targetBacklight = currentBacklight;
                string reason = "";

                // Detect darkening pattern - should turn backlight ON
                // IMPORTANT: Only turn on if BOTH pattern is detected AND absolute light level is dark enough
                if (pattern.Trend == LightTrend.Darkening && 
                    pattern.TotalChange <= _darkeningThreshold && 
                    pattern.Confidence >= _patternConfidence &&
                    currentLux <= _absoluteDarkThreshold && // NEW: Must be actually dark
                    currentBacklight == 0)
                {
                    var minBacklight = AppConfig.Get("als_min_backlight", 1);
                    var useInitialLowBrightness = AppConfig.Is("als_initial_low_brightness");
                    
                    targetBacklight = useInitialLowBrightness ? minBacklight : 
                                     CalculateBacklightFromDarkness(pattern.AverageLux);
                    
                    reason = $"Darkening pattern detected (Δ{pattern.TotalChange:F0} lux, now {currentLux} lux)";
                    Logger.WriteLine($"Environment darkening AND dark enough: turning backlight ON to level {targetBacklight}");
                }
                // Detect brightening pattern - should turn backlight OFF
                // IMPORTANT: Only turn off if BOTH pattern is detected AND absolute light level is bright enough
                else if (pattern.Trend == LightTrend.Brightening && 
                         pattern.TotalChange >= _brighteningThreshold && 
                         pattern.Confidence >= _patternConfidence &&
                         currentLux >= _absoluteBrightThreshold && // NEW: Must be actually bright
                         currentBacklight > 0)
                {
                    targetBacklight = 0;
                    reason = $"Brightening pattern detected (Δ+{pattern.TotalChange:F0} lux, now {currentLux} lux)";
                    Logger.WriteLine($"Environment brightening AND bright enough: turning backlight OFF");
                }
                else
                {
                    // Log why conditions weren't met
                    if (pattern.Trend == LightTrend.Darkening && currentBacklight == 0)
                    {
                        if (currentLux > _absoluteDarkThreshold)
                            Logger.WriteLine($"Darkening pattern detected but too bright ({currentLux} > {_absoluteDarkThreshold} lux threshold)");
                        else if (pattern.TotalChange > _darkeningThreshold)
                            Logger.WriteLine($"Not dark enough change (Δ{pattern.TotalChange} > {_darkeningThreshold} threshold)");
                        else if (pattern.Confidence < _patternConfidence)
                            Logger.WriteLine($"Pattern confidence too low ({pattern.Confidence:P0} < {_patternConfidence:P0} required)");
                    }
                    else if (pattern.Trend == LightTrend.Brightening && currentBacklight > 0)
                    {
                        if (currentLux < _absoluteBrightThreshold)
                            Logger.WriteLine($"Brightening pattern detected but not bright enough ({currentLux} < {_absoluteBrightThreshold} lux threshold)");
                        else if (pattern.TotalChange < _brighteningThreshold)
                            Logger.WriteLine($"Not bright enough change (Δ{pattern.TotalChange} < {_brighteningThreshold} threshold)");
                        else if (pattern.Confidence < _patternConfidence)
                            Logger.WriteLine($"Pattern confidence too low ({pattern.Confidence:P0} < {_patternConfidence:P0} required)");
                    }
                    else
                    {
                        Logger.WriteLine($"Pattern detected but conditions not met for adjustment");
                    }
                    return;
                }

                // Apply change if different
                if (targetBacklight != currentBacklight)
                {
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
                Logger.WriteLine($"Error in AdjustBacklightBasedOnPattern: {ex.Message}");
            }
        }

        private void PerformInitialBacklightCheck(int luxValue)
        {
            try
            {
                var currentBacklight = InputDispatcher.GetBacklight();
                
                // FIXED: Only turn on backlight if it's ACTUALLY dark (indoor lighting is typically 50-500 lux)
                // Using the absolute dark threshold instead of a fixed 50
                if (luxValue <= _absoluteDarkThreshold && currentBacklight == 0)
                {
                    var minBacklight = AppConfig.Get("als_min_backlight", 1);
                    var useInitialLowBrightness = AppConfig.Is("als_initial_low_brightness");
                    
                    var targetBacklight = useInitialLowBrightness ? minBacklight : CalculateBacklightFromDarkness(luxValue);
                    
                    Logger.WriteLine($"Initial check: Dark environment ({luxValue} ≤ {_absoluteDarkThreshold} lux), turning backlight ON to level {targetBacklight}");
                    
                    bool onBattery = System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus != System.Windows.Forms.PowerLineStatus.Online;
                    if (onBattery)
                        AppConfig.Set("keyboard_brightness_ac", targetBacklight);
                    else
                        AppConfig.Set("keyboard_brightness", targetBacklight);
                    
                    Aura.ApplyBrightness(targetBacklight, "Initial ALS");
                    _lastBacklightLevel = targetBacklight;
                }
                else
                {
                    Logger.WriteLine($"Initial check: No adjustment needed ({luxValue} lux, backlight={currentBacklight}, threshold={_absoluteDarkThreshold})");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error in PerformInitialBacklightCheck: {ex.Message}");
            }
        }

        private LightPattern? AnalyzeLightPattern()
        {
            if (_lightLevelHistory.Count < MIN_READINGS_FOR_PATTERN)
                return null;

            var readings = _lightLevelHistory.ToArray();
            var firstReading = readings[0];
            var lastReading = readings[^1];
            
            // Calculate total change
            int totalChange = lastReading.Lux - firstReading.Lux;
            
            // Calculate average lux
            double averageLux = readings.Average(r => r.Lux);
            
            // Determine trend
            LightTrend trend;
            if (totalChange < -5) // Getting darker (negative change)
                trend = LightTrend.Darkening;
            else if (totalChange > 5) // Getting brighter (positive change)
                trend = LightTrend.Brightening;
            else
                trend = LightTrend.Stable;
            
            // Calculate confidence based on consistency of the trend
            double confidence = CalculatePatternConfidence(readings, trend);
            
            return new LightPattern
            {
                Trend = trend,
                TotalChange = totalChange,
                AverageLux = averageLux,
                Confidence = confidence,
                ReadingsCount = readings.Length
            };
        }

        private double CalculatePatternConfidence(LightLevelReading[] readings, LightTrend trend)
        {
            if (readings.Length < 2)
                return 0.0;
            
            int consistentChanges = 0;
            int totalChanges = readings.Length - 1;
            
            for (int i = 1; i < readings.Length; i++)
            {
                int change = readings[i].Lux - readings[i - 1].Lux;
                
                bool isConsistent = trend switch
                {
                    LightTrend.Darkening => change <= 0, // Each reading should be darker or same
                    LightTrend.Brightening => change >= 0, // Each reading should be brighter or same
                    LightTrend.Stable => Math.Abs(change) < 5, // Small changes
                    _ => false
                };
                
                if (isConsistent)
                    consistentChanges++;
            }
            
            return (double)consistentChanges / totalChanges;
        }

        private int CalculateBacklightFromDarkness(double luxValue)
        {
            var minBacklight = AppConfig.Get("als_min_backlight", 1);
            var maxBacklight = AppConfig.Get("als_max_backlight", 3);
            
            // Very simple scaling: darker = higher backlight
            if (luxValue < 10)
                return maxBacklight;
            else if (luxValue < 30)
                return Math.Min(maxBacklight, minBacklight + 1);
            else
                return minBacklight;
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
            Logger.WriteLine("User input cooldown expired, resuming auto backlight with pattern analysis");
            
            // Trigger immediate re-evaluation with current pattern
            if (_lightLevelHistory.Count >= MIN_READINGS_FOR_PATTERN)
            {
                Task.Run(() => AdjustBacklightBasedOnPattern());
            }
        }

        public void UpdateSettings()
        {
            _patternWindowSeconds = AppConfig.Get("als_pattern_window", 30);
            _darkeningThreshold = AppConfig.Get("als_darkening_threshold", -30);
            _brighteningThreshold = AppConfig.Get("als_brightening_threshold", 40);
            _patternConfidence = AppConfig.Get("als_pattern_confidence", 70) / 100.0;
            _absoluteDarkThreshold = AppConfig.Get("als_absolute_dark", 20);
            _absoluteBrightThreshold = AppConfig.Get("als_absolute_bright", 80);

            Logger.WriteLine($"Auto backlight settings updated: Window={_patternWindowSeconds}s, " +
                           $"DarkThreshold={_darkeningThreshold}, BrightThreshold={_brighteningThreshold}, " +
                           $"Confidence={_patternConfidence:P0}, AbsoluteDark≤{_absoluteDarkThreshold}, AbsoluteBright≥{_absoluteBrightThreshold}");
            
            // Re-evaluate current conditions with new settings
            if (_isEnabled && _lightLevelHistory.Count >= MIN_READINGS_FOR_PATTERN)
            {
                Task.Run(() => AdjustBacklightBasedOnPattern());
            }
        }

        public Dictionary<string, object> GetStatus()
        {
            var pattern = _lightLevelHistory.Count >= MIN_READINGS_FOR_PATTERN ? AnalyzeLightPattern() : null;
            
            return new Dictionary<string, object>
            {
                ["enabled"] = _isEnabled,
                ["sensor_available"] = AmbientLightSensor.IsAvailable(),
                ["sensor_active"] = AmbientLightSensor.IsSensorActive(),
                ["current_lux"] = AmbientLightSensor.GetCurrentLux(),
                ["current_backlight"] = InputDispatcher.GetBacklight(),
                ["pattern_window_seconds"] = _patternWindowSeconds,
                ["darkening_threshold"] = _darkeningThreshold,
                ["brightening_threshold"] = _brighteningThreshold,
                ["pattern_confidence_required"] = _patternConfidence,
                ["absolute_dark_threshold"] = _absoluteDarkThreshold,
                ["absolute_bright_threshold"] = _absoluteBrightThreshold,
                ["readings_count"] = _lightLevelHistory.Count,
                ["pattern_trend"] = pattern?.Trend.ToString() ?? "None",
                ["pattern_change"] = pattern?.TotalChange ?? 0,
                ["pattern_confidence"] = pattern?.Confidence ?? 0.0,
                ["last_user_input"] = _lastUserInput,
                ["last_sensor_reading"] = AmbientLightSensor.GetLastReadingTime()
            };
        }

        public void Dispose()
        {
            StopAutoBacklight();
            _userInputTimer?.Dispose();
            AmbientLightSensor.LightLevelChanged -= OnLightLevelChanged;
        }

        // Helper classes for pattern analysis
        private class LightLevelReading
        {
            public int Lux { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class LightPattern
        {
            public LightTrend Trend { get; set; }
            public int TotalChange { get; set; }
            public double AverageLux { get; set; }
            public double Confidence { get; set; }
            public int ReadingsCount { get; set; }
        }

        private enum LightTrend
        {
            Stable,
            Darkening,
            Brightening
        }
    }
}