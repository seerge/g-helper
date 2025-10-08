using System;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace GHelper.Helpers
{
    public static class AmbientLightSensor
    {
        private static LightSensor? _lightSensor;
        private static uint _currentLuxValue = 0;
        private static bool _isInitialized = false;
        private static bool _sensorAvailable = false;
        private static bool _isMonitoring = false;
        private static DateTime _lastReadingTime = DateTime.MinValue;

        // Exponential smoothing parameters
        private static double _smoothedLux = -1;
        private static double _smoothingFactor; // Alpha
        private static int _maxChangePerSecond;

        public static event EventHandler<LightLevelChangedEventArgs>? LightLevelChanged;

        public static int GetCurrentLux()
        {
            return (int)Math.Round(_smoothedLux >= 0 ? _smoothedLux : _currentLuxValue);
        }

        public static bool IsAvailable()
        {
            if (_isInitialized) return _sensorAvailable;

            try
            {
                _lightSensor = LightSensor.GetDefault();
                _sensorAvailable = _lightSensor != null;

                if (_sensorAvailable)
                {
                    Logger.WriteLine("Windows Runtime LightSensor found and available");
                    
                    // Set minimum report interval to reduce battery usage
                    var minInterval = _lightSensor.MinimumReportInterval;
                    var desiredInterval = Math.Max(minInterval, 1000); // At least 1 second
                    _lightSensor.ReportInterval = desiredInterval;
                    
                    Logger.WriteLine($"Sensor intervals - Min: {minInterval}ms, Set: {desiredInterval}ms");
                }
                else
                {
                    Logger.WriteLine("No Windows Runtime LightSensor available");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to detect ambient light sensor: {ex.Message}");
                _sensorAvailable = false;
            }

            _isInitialized = true;
            Logger.WriteLine($"Ambient light sensor availability: {_sensorAvailable}");
            return _sensorAvailable;
        }

        public static void Initialize()
        {
            if (_isInitialized) return;

            // Load smoothing settings
            if (!AppConfig.Exists("als_smoothing_factor")) AppConfig.Set("als_smoothing_factor", 40); // 0.4
            if (!AppConfig.Exists("als_max_change_per_sec")) AppConfig.Set("als_max_change_per_sec", 50);
            
            _smoothingFactor = AppConfig.Get("als_smoothing_factor", 40) / 100.0;
            _maxChangePerSecond = AppConfig.Get("als_max_change_per_sec", 50);

            _sensorAvailable = IsAvailable();

            if (!_sensorAvailable)
            {
                Logger.WriteLine("Ambient light sensor not available");
            }

            _isInitialized = true;
            Logger.WriteLine("Ambient light sensor initialized");
        }

        public static void Start()
        {
            if (!_isInitialized) return;

            if (_sensorAvailable && _lightSensor != null && !_isMonitoring)
            {
                try
                {
                    // Reset smoothed value on start
                    _smoothedLux = -1;

                    // Subscribe to sensor reading changes
                    _lightSensor.ReadingChanged += OnSensorReadingChanged;
                    _isMonitoring = true;
                    
                    Logger.WriteLine("Ambient light sensor monitoring started (event-driven)");

                    // Get initial reading
                    Task.Run(async () =>
                    {
                        await Task.Delay(500); // Small delay to let sensor initialize
                        TriggerInitialReading();
                    });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Failed to start sensor monitoring: {ex.Message}");
                }
            }
            else
            {
                Logger.WriteLine("Ambient light sensor not available - no automatic backlight control");
            }
        }

        public static void Stop()
        {
            if (_lightSensor != null && _isMonitoring)
            {
                try
                {
                    _lightSensor.ReadingChanged -= OnSensorReadingChanged;
                    _isMonitoring = false;
                    Logger.WriteLine("Ambient light sensor monitoring stopped");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Error stopping sensor: {ex.Message}");
                }
            }
        }

        private static void OnSensorReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
        {
            try
            {
                var reading = args.Reading;
                var rawLuxValue = reading?.IlluminanceInLux;
                if (rawLuxValue.HasValue)
                {
                    var newRawLux = (uint)Math.Max(0, Math.Round(rawLuxValue.Value));
                    var previousSmoothedLux = _smoothedLux;
                    var now = DateTime.Now;
                    
                    // Initialize smoothed value on first reading
                    if (_smoothedLux < 0)
                    {
                        _smoothedLux = newRawLux;
                    }

                    // Apply exponential smoothing
                    var newSmoothedLux = _smoothingFactor * newRawLux + (1 - _smoothingFactor) * _smoothedLux;

                    // Apply rate-of-change limiting
                    if (previousSmoothedLux >= 0)
                    {
                        var timeDelta = (now - _lastReadingTime).TotalSeconds;
                        if (timeDelta > 0)
                        {
                            var maxChange = _maxChangePerSecond * timeDelta;
                            var change = newSmoothedLux - previousSmoothedLux;
                            
                            if (Math.Abs(change) > maxChange)
                            {
                                newSmoothedLux = previousSmoothedLux + Math.Sign(change) * maxChange;
                                Logger.WriteLine($"Rate limit applied: Raw={newRawLux}, Change={change:F1}, Limited to={maxChange:F1}");
                            }
                        }
                    }

                    // Only trigger events if the smoothed value has changed meaningfully
                    if (Math.Abs(newSmoothedLux - _smoothedLux) >= 1)
                    {
                        var previousValue = (int)Math.Round(_smoothedLux);
                        _smoothedLux = newSmoothedLux;
                        _currentLuxValue = newRawLux; // Keep track of raw value for logging
                        _lastReadingTime = now;

                        var finalLux = (int)Math.Round(_smoothedLux);

                        Logger.WriteLine($"Light level changed: Raw={newRawLux}, Smoothed={finalLux} lux");

                        LightLevelChanged?.Invoke(null, new LightLevelChangedEventArgs
                        {
                            CurrentLux = finalLux,
                            PreviousLux = previousValue,
                            Timestamp = _lastReadingTime
                        });
                    }
                }
                else
                {
                    Logger.WriteLine("Sensor reading returned null lux value");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error processing sensor reading: {ex.Message}");
            }
        }

        private static void TriggerInitialReading()
        {
            try
            {
                if (_lightSensor != null)
                {
                    var reading = _lightSensor.GetCurrentReading();
                    var luxValue = reading?.IlluminanceInLux;
                    if (luxValue.HasValue)
                    {
                        _currentLuxValue = (uint)Math.Max(0, Math.Round(luxValue.Value));
                        _smoothedLux = _currentLuxValue; // Initialize smoothed value
                        _lastReadingTime = DateTime.Now;
                        
                        Logger.WriteLine($"Initial sensor reading: {_currentLuxValue} lux");

                        LightLevelChanged?.Invoke(null, new LightLevelChangedEventArgs
                        {
                            CurrentLux = (int)_currentLuxValue,
                            PreviousLux = -1,
                            Timestamp = _lastReadingTime
                        });
                    }
                    else
                    {
                        Logger.WriteLine("Initial sensor reading returned null");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error getting initial reading: {ex.Message}");
            }
        }

        public static DateTime GetLastReadingTime()
        {
            return _lastReadingTime;
        }

        public static bool IsSensorActive()
        {
            return _sensorAvailable && _isMonitoring;
        }
    }

    public class LightLevelChangedEventArgs : EventArgs
    {
        public int CurrentLux { get; set; }
        public int PreviousLux { get; set; }
        public DateTime Timestamp { get; set; }
    }
}