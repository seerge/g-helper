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

        public static event EventHandler<LightLevelChangedEventArgs>? LightLevelChanged;

        public static int GetCurrentLux()
        {
            return (int)_currentLuxValue;
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
                var luxValue = reading?.IlluminanceInLux;
                if (luxValue.HasValue)
                {
                    var newLuxValue = (uint)Math.Max(0, Math.Round(luxValue.Value));
                    
                    // Only trigger events if the value changed significantly (hysteresis)
                    if (Math.Abs((int)newLuxValue - (int)_currentLuxValue) >= 5 || _currentLuxValue == 0)
                    {
                        var previousValue = _currentLuxValue;
                        _currentLuxValue = newLuxValue;
                        _lastReadingTime = DateTime.Now;

                        Logger.WriteLine($"Light level changed: {previousValue} → {newLuxValue} lux");

                        LightLevelChanged?.Invoke(null, new LightLevelChangedEventArgs
                        {
                            CurrentLux = (int)newLuxValue,
                            PreviousLux = (int)previousValue,
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