using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Timers;

namespace GHelper.AnimeMatrix
{

    public class AniMatrixControl : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient
    {

        SettingsForm settings;

        System.Timers.Timer matrixTimer = default!;
        System.Timers.Timer slashTimer = default!;

        public AnimeMatrixDevice? deviceMatrix;
        public SlashDevice? deviceSlash;

        public static bool lidClose = false;
        private static bool _wakeUp = false;

        double[]? AudioValues;
        WasapiCapture? AudioDevice;
        string? AudioDeviceId;
        private MMDeviceEnumerator? AudioDeviceEnum;

        public bool IsValid => deviceMatrix != null || deviceSlash != null;
        public bool IsSlash => deviceSlash != null;

        private long lastPresent;
        private List<double> maxes = new List<double>();

        public AniMatrixControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
            if (!AppConfig.IsSlash() && !AppConfig.IsAnimeMatrix()) return;
            
            try
            {
                if (AppConfig.IsSlash())
                {
                    if (AppConfig.IsSlashAura())
                        deviceSlash = new SlashDeviceAura();
                    else
                        deviceSlash = new SlashDevice();
                }
                else
                {
                    deviceMatrix = new AnimeMatrixDevice();
                }

                matrixTimer = new System.Timers.Timer(100);
                matrixTimer.Elapsed += MatrixTimer_Elapsed;

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

        }

        public void SetDevice(bool wakeUp = false)
        {
            if (deviceMatrix is not null) SetMatrix(wakeUp);
            if (deviceSlash is not null) SetSlash(wakeUp);
        }

        public void SetSlash(bool wakeUp = false)
        {
            if (deviceSlash is null) return;

            int brightness = AppConfig.Get("matrix_brightness", 0);
            int running = AppConfig.Get("matrix_running", 0);
            int inteval = AppConfig.Get("matrix_interval", 0);

            bool auto = AppConfig.Is("matrix_auto");
            bool lid = AppConfig.Is("matrix_lid");

            Task.Run(() =>
            {
                try
                {
                    deviceSlash.SetProvider();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                    return;
                }

                if (wakeUp) _wakeUp = true;

                if (brightness == 0 || (auto && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online) || (lid && lidClose))
                {
                    deviceSlash.SetSleepActive(false);
                    deviceSlash.SetEnabled(false);
                    //deviceSlash.Init();
                    //deviceSlash.SetOptions(false, 0, 0);
                }
                else
                {
                    if (_wakeUp)
                    {
                        deviceSlash.WakeUp();
                        _wakeUp = false;
                    }

                    deviceSlash.SetEnabled(true);
                    deviceSlash.Init();

                    switch ((SlashMode)running)
                    {
                        case SlashMode.Static:
                            var custom = AppConfig.GetString("slash_custom");
                            if (custom is not null && custom.Length > 0)
                            {
                                Logger.WriteLine("Slash: Static");
                                deviceSlash.SetCustom(AppConfig.StringToBytes(custom));
                            }
                            else
                            {
                                deviceSlash.SetMode((SlashMode)running);
                                deviceSlash.SetOptions(true, brightness, inteval);
                                deviceSlash.Save();
                            }
                            break;
                        case SlashMode.BatteryLevel:
                            // call tick to immediately update the pattern
                            Logger.WriteLine("Slash: Battery Level");
                            SlashTimer_start();
                            SlashTimer_tick();
                            break;
                        default:
                            deviceSlash.SetMode((SlashMode)running);
                            deviceSlash.SetOptions(true, brightness, inteval);
                            deviceSlash.Save();
                            break;
                    }

                    // kill the timer if we are not displaying battery pattern

                    deviceSlash.SetSleepActive(AppConfig.IsNotFalse("slash_sleep"));
                }
            });
        }

        public void SetLidMode(bool force = false)
        {
            bool matrixLid = AppConfig.Is("matrix_lid");

            if (deviceSlash is not null)
            {
                deviceSlash.SetLidCloseAnimation(!matrixLid && !AppConfig.Is("slash_sleep"));
            }

            if (matrixLid || force)
            {
                Logger.WriteLine($"Matrix LidClosed: {lidClose}");
                SetDevice(true);
            }
        }

        public void SetBatteryAuto()
        {
            if (deviceSlash is not null)
            {
                bool auto = AppConfig.Is("matrix_auto");
                deviceSlash.SetBatterySaver(auto);
                if (!auto) SetSlash();
            }

            if (deviceMatrix is not null) SetMatrix();
        }

        public void SetMatrix(bool wakeUp = false)
        {

            if (deviceMatrix is null) return;

            int brightness = AppConfig.Get("matrix_brightness", 0);
            int running = AppConfig.Get("matrix_running", 0);
            bool auto = AppConfig.Is("matrix_auto");
            bool lid = AppConfig.Is("matrix_lid");

            StopMatrixTimer();
            StopMatrixAudio();

            Task.Run(() =>
            {
                try
                {
                    deviceMatrix.SetProvider();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                    return;
                }

                if (wakeUp) deviceMatrix.WakeUp();

                if (brightness == 0 || (auto && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online) || (lid && lidClose))
                {
                    deviceMatrix.SetDisplayState(false);
                    deviceMatrix.SetDisplayState(false); // some devices are dumb
                    Logger.WriteLine("Matrix Off");
                }
                else
                {
                    deviceMatrix.SetDisplayState(true);
                    deviceMatrix.SetBrightness((BrightnessMode)brightness);

                    switch (running)
                    {
                        case 2:
                            SetMatrixPicture(AppConfig.GetString("matrix_picture"), false);
                            break;
                        case 3:
                            SetMatrixClock();
                            break;
                        case 4:
                            SetMatrixAudio();
                            break;
                        default:
                            SetBuiltIn(running);
                            break;
                    }

                }
            });


        }

        private void SetBuiltIn(int running)
        {
            BuiltInAnimation animation = new BuiltInAnimation(
                (BuiltInAnimation.Running)running,
                (BuiltInAnimation.Sleeping)AppConfig.Get("matrix_sleep", (int)BuiltInAnimation.Sleeping.Starfield),
                (BuiltInAnimation.Shutdown)AppConfig.Get("matrix_shutdown", (int)BuiltInAnimation.Shutdown.SeeYa),
                (BuiltInAnimation.Startup)AppConfig.Get("matrix_startup", (int)BuiltInAnimation.Startup.StaticEmergence)
            );
            deviceMatrix.SetBuiltInAnimation(true, animation);
            Logger.WriteLine("Matrix builtin: " + animation.AsByte);
        }

        private void StartMatrixTimer(int interval = 100)
        {
            matrixTimer.Interval = interval;
            matrixTimer.Start();
        }

        private void StopMatrixTimer()
        {
            matrixTimer.Stop();
        }

        private void MatrixTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {

            if (deviceMatrix is null) return;

            switch (AppConfig.Get("matrix_running"))
            {
                case 2:
                    deviceMatrix.PresentNextFrame();
                    break;
                case 3:
                    deviceMatrix.PresentClock();
                    break;
            }

        }

        public void SetMatrixClock()
        {
            deviceMatrix.SetBuiltInAnimation(false);
            StartMatrixTimer(1000);
            Logger.WriteLine("Matrix Clock");
        }


        private void SlashTimer_start(int interval = 180000)
        {
            // 100% to 0% in 1hr = 1% every 36 seconds
            // 1 bracket every 14.2857 * 36s = 514s ~ 8m 30s
            // only ~5 actually distinguishable levels, so refresh every <= 514/5 ~ 100s
            // default is 60s

            // create the timer if first call
            // this way, the timer only spawns if user tries to use battery pattern
            if (slashTimer == default(System.Timers.Timer))
            {
                slashTimer = new System.Timers.Timer(interval);
                slashTimer.Elapsed += SlashTimer_elapsed;
                slashTimer.AutoReset = true;
            }
            // only write if interval changed
            if (slashTimer.Interval != interval)
            {
                slashTimer.Interval = interval;
            }

            slashTimer.Start();
        }

        private void SlashTimer_elapsed(object? sender, ElapsedEventArgs e)
        {
            SlashTimer_tick();
        }

        private void SlashTimer_tick()
        {
            if (deviceSlash is null) return;

            //kill timer if called but not in battery pattern mode
            if ((SlashMode)AppConfig.Get("matrix_running", 0) != SlashMode.BatteryLevel)
            {
                slashTimer.Stop();
                slashTimer.Dispose();
                return;
            }

            deviceSlash.SetBatteryPattern(AppConfig.Get("matrix_brightness", 0));
        }


        public void Dispose()
        {
            StopMatrixAudio();
        }

        void StopMatrixAudio()
        {
            if (AudioDevice is not null)
            {
                try
                {
                    AudioDevice.StopRecording();
                    AudioDevice.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.ToString());
                }
            }

            AudioDeviceId = null;
            AudioDeviceEnum?.Dispose();
        }

        void SetMatrixAudio()
        {
            if (deviceMatrix is null) return;

            deviceMatrix.SetBuiltInAnimation(false);
            StopMatrixTimer();
            StopMatrixAudio();

            try
            {
                AudioDeviceEnum = new MMDeviceEnumerator();
                AudioDeviceEnum.RegisterEndpointNotificationCallback(this);

                using (MMDevice device = AudioDeviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console))
                {
                    AudioDevice = new WasapiLoopbackCapture(device);
                    AudioDeviceId = device.ID;
                    WaveFormat fmt = AudioDevice.WaveFormat;

                    AudioValues = new double[fmt.SampleRate / 1000];
                    AudioDevice.DataAvailable += WaveIn_DataAvailable;
                    AudioDevice.StartRecording();
                    Logger.WriteLine("Matrix Audio");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            int bytesPerSamplePerChannel = AudioDevice.WaveFormat.BitsPerSample / 8;
            int bytesPerSample = bytesPerSamplePerChannel * AudioDevice.WaveFormat.Channels;
            int bufferSampleCount = e.Buffer.Length / bytesPerSample;

            if (bufferSampleCount >= AudioValues.Length)
            {
                bufferSampleCount = AudioValues.Length;
            }

            if (bytesPerSamplePerChannel == 2 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < bufferSampleCount; i++)
                    AudioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
            }
            else if (bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < bufferSampleCount; i++)
                    AudioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
            }
            else if (bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int i = 0; i < bufferSampleCount; i++)
                    AudioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
            }

            double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
            double[] fftMag = FftSharp.Transform.FFTmagnitude(paddedAudio);

            PresentAudio(fftMag);
        }

        void PresentAudio(double[] audio)
        {

            if (deviceMatrix is null) return;

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastPresent) < 70) return;
            lastPresent = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            deviceMatrix.Clear();

            int size = 20;
            double[] bars = new double[size];
            double max = 2, maxAverage;

            for (int i = 0; i < size; i++)
            {
                bars[i] = Math.Sqrt(audio[i] * 10000);
                if (bars[i] > max) max = bars[i];
            }

            maxes.Add(max);
            if (maxes.Count > 20) maxes.RemoveAt(0);
            maxAverage = maxes.Average();

            for (int i = 0; i < size; i++) deviceMatrix.DrawBar(20 - i, bars[i] * 20 / maxAverage);

            deviceMatrix.Present();
        }

        public void OpenMatrixPicture()
        {
            string fileName = null;

            Thread t = new Thread(() =>
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png,*.gif)|*.BMP;*.JPG;*.JPEG;*.PNG;*.GIF";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    fileName = of.FileName;
                }
                return;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (fileName is not null)
            {
                AppConfig.Set("matrix_picture", fileName);
                AppConfig.Set("matrix_running", 2);

                SetMatrixPicture(fileName);
                settings.VisualiseMatrixRunning(2);

            }

        }

        public void SetMatrixPicture(string fileName, bool visualise = true)
        {

            if (deviceMatrix is null) return;

            StopMatrixTimer();

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open))
                //using (var ms = new MemoryStream())
                {
                    /*
                    ms.SetLength(0);
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    */
                    using (Image image = Image.FromStream(fs))
                    {
                        ProcessPicture(image);
                        Logger.WriteLine("Matrix " + fileName);
                    }

                    fs.Close();
                    if (visualise) settings.VisualiseMatrixPicture(fileName);
                }
            }
            catch
            {
                Debug.WriteLine("Error loading picture");
                return;
            }

        }

        protected void ProcessPicture(Image image)
        {
            deviceMatrix.SetBuiltInAnimation(false);
            deviceMatrix.ClearFrames();

            int matrixX = AppConfig.Get("matrix_x", 0);
            int matrixY = AppConfig.Get("matrix_y", 0);

            int matrixZoom = AppConfig.Get("matrix_zoom", 100);
            int matrixContrast = AppConfig.Get("matrix_contrast", 100);
            int matrixGamma = AppConfig.Get("matrix_gamma", 0);

            int matrixSpeed = AppConfig.Get("matrix_speed", 50);

            MatrixRotation rotation = (MatrixRotation)AppConfig.Get("matrix_rotation", 0);

            InterpolationMode matrixQuality = (InterpolationMode)AppConfig.Get("matrix_quality", 0);


            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(dimension);

            if (frameCount > 1)
            {
                var delayPropertyBytes = image.GetPropertyItem(0x5100).Value;
                var frameDelay = BitConverter.ToInt32(delayPropertyBytes) * 10;

                for (int i = 0; i < frameCount; i++)
                {
                    image.SelectActiveFrame(dimension, i);

                    if (rotation == MatrixRotation.Planar)
                        deviceMatrix.GenerateFrame(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast, matrixGamma);
                    else
                        deviceMatrix.GenerateFrameDiagonal(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast, matrixGamma);

                    deviceMatrix.AddFrame();
                }


                Logger.WriteLine("GIF Delay:" + frameDelay);
                StartMatrixTimer(Math.Max(matrixSpeed, frameDelay));

                //image.SelectActiveFrame(dimension, 0);

            }
            else
            {
                if (rotation == MatrixRotation.Planar)
                    deviceMatrix.GenerateFrame(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast, matrixGamma);
                else
                    deviceMatrix.GenerateFrameDiagonal(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast, matrixGamma);

                deviceMatrix.Present();
            }

        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {

        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {

        }

        public void OnDeviceRemoved(string deviceId)
        {

        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (AudioDeviceId == defaultDeviceId)
            {
                //We already caputre this device. No need to re-initialize
                return;
            }

            int running = AppConfig.Get("matrix_running");
            if (flow != DataFlow.Render || role != Role.Console || running != 4)
            {
                return;
            }

            //Restart audio if default audio changed
            Logger.WriteLine("Matrix Audio: Default Output changed to " + defaultDeviceId);

            //Already set the device here. Otherwise this will be called multiple times in a short succession and causes a crash due to dispose during initalization.
            AudioDeviceId = defaultDeviceId;

            //Delay is required or it will deadlock on dispose.
            Task.Delay(50).ContinueWith(t => SetMatrixAudio());
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {

        }
    }
}
