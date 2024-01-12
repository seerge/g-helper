using NAudio.CoreAudioApi;
using NAudio.Wave;
using Starlight.AnimeMatrix;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;

namespace GHelper.AnimeMatrix
{

    public class AniMatrixControl : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient
    {

        SettingsForm settings;

        System.Timers.Timer matrixTimer = default!;
        public AnimeMatrixDevice? device;

        double[]? AudioValues;
        WasapiCapture? AudioDevice;
        string? AudioDeviceId;
        private MMDeviceEnumerator? AudioDeviceEnum;

        public bool IsValid => device != null;

        private long lastPresent;
        private List<double> maxes = new List<double>();

        public AniMatrixControl(SettingsForm settingsForm)
        {
            settings = settingsForm;

            try
            {
                device = new AnimeMatrixDevice();
                Task.Run(device.WakeUp);
                matrixTimer = new System.Timers.Timer(100);
                matrixTimer.Elapsed += MatrixTimer_Elapsed;
            }
            catch
            {
                device = null;
            }

        }

        public void SetMatrix(bool wakeUp = false)
        {

            if (!IsValid) return;

            int brightness = AppConfig.Get("matrix_brightness");
            int running = AppConfig.Get("matrix_running");

            bool auto = AppConfig.Is("matrix_auto");

            if (brightness < 0) brightness = 0;
            if (running < 0) running = 0;

            BuiltInAnimation animation = new BuiltInAnimation(
                (BuiltInAnimation.Running)running,
                BuiltInAnimation.Sleeping.Starfield,
                BuiltInAnimation.Shutdown.SeeYa,
                BuiltInAnimation.Startup.StaticEmergence
            );

            StopMatrixTimer();
            StopMatrixAudio();

            try
            {
                device.SetProvider();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                return;
            }

            if (wakeUp && AppConfig.ContainsModel("401")) device.WakeUp();

            if (brightness == 0 || (auto && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online))
            {
                device.SetDisplayState(false);
                device.SetDisplayState(false); // some devices are dumb
                Logger.WriteLine("Matrix Off");
            }
            else
            {
                device.SetDisplayState(true);
                device.SetBrightness((BrightnessMode)brightness);

                switch (running)
                {
                    case 2:
                        SetMatrixPicture(AppConfig.GetString("matrix_picture"));
                        break;
                    case 3:
                        SetMatrixClock();
                        break;
                    case 4:
                        SetMatrixAudio();
                        break;
                    default:
                        device.SetBuiltInAnimation(true, animation);
                        Logger.WriteLine("Matrix builtin " + animation.AsByte);
                        break;

                }

                //mat.SetBrightness((BrightnessMode)brightness);
            }

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
            //if (!IsValid) return;

            switch (AppConfig.Get("matrix_running"))
            {
                case 2:
                    device.PresentNextFrame();
                    break;
                case 3:
                    device.PresentClock();
                    break;
            }

        }


        public void SetMatrixClock()
        {
            device.SetBuiltInAnimation(false);
            StartMatrixTimer(1000);
            Logger.WriteLine("Matrix Clock");
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
            if (!IsValid) return;

            device.SetBuiltInAnimation(false);
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

        private void DrawBar(int pos, double h)
        {
            int dx = pos * 2;
            int dy = 20;

            byte color;

            for (int y = 0; y < h - (h % 2); y++)
                for (int x = 0; x < 2 - (y % 2); x++)
                {
                    //color = (byte)(Math.Min(1,(h - y - 2)*2) * 255);
                    device.SetLedPlanar(x + dx, dy + y, (byte)(h * 255 / 30));
                    device.SetLedPlanar(x + dx, dy - y, 255);
                }
        }

        void PresentAudio(double[] audio)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastPresent) < 70) return;
            lastPresent = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            device.Clear();

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

            for (int i = 0; i < size; i++) DrawBar(20 - i, bars[i] * 20 / maxAverage);

            device.Present();
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
                settings.SetMatrixRunning(2);

            }

        }

        public void SetMatrixPicture(string fileName, bool visualise = true)
        {

            if (!IsValid) return;
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
                    if (visualise) settings.VisualiseMatrix(fileName);
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
            device.SetBuiltInAnimation(false);
            device.ClearFrames();

            int matrixX = AppConfig.Get("matrix_x", 0);
            int matrixY = AppConfig.Get("matrix_y", 0);

            int matrixZoom = AppConfig.Get("matrix_zoom", 100);
            int matrixContrast = AppConfig.Get("matrix_contrast", 100);
            
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
                        device.GenerateFrame(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast);
                    else
                        device.GenerateFrameDiagonal(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast);
                    
                    device.AddFrame();
                }


                Logger.WriteLine("GIF Delay:" + frameDelay);
                StartMatrixTimer(Math.Max(matrixSpeed, frameDelay));

                //image.SelectActiveFrame(dimension, 0);

            }
            else
            {
                if (rotation == MatrixRotation.Planar)
                    device.GenerateFrame(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast);
                else
                    device.GenerateFrameDiagonal(image, matrixZoom, matrixX, matrixY, matrixQuality, matrixContrast);

                device.Present();
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
