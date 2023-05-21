using NAudio.CoreAudioApi;
using NAudio.Wave;
using Starlight.AnimeMatrix;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Timers;

namespace GHelper.AnimeMatrix
{

    public class AniMatrix
    {
        static System.Timers.Timer matrixTimer = default!;
        static AnimeMatrixDevice mat;

        static double[] AudioValues;
        static WasapiCapture AudioDevice;

        public static bool IsValid => mat != null;

        private static long lastPresent;
        private static List<double> maxes = new List<double>();

        public AniMatrix()
        {
            try
            {
                mat = new AnimeMatrixDevice();
                mat.WakeUp();
                matrixTimer = new System.Timers.Timer(100);
                matrixTimer.Elapsed += MatrixTimer_Elapsed;
            }
            catch
            {
                mat = null;
            }

        }

        public void SetMatrix()
        {

            if (!IsValid) return;

            int brightness = AppConfig.getConfig("matrix_brightness");
            int running = AppConfig.getConfig("matrix_running");

            bool auto = AppConfig.getConfig("matrix_auto") == 1;

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

            mat.SetProvider();

            if (brightness == 0 || (auto && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online))
            {
                mat.SetDisplayState(false);
                Logger.WriteLine("Matrix Off");
            }
            else
            {
                mat.SetDisplayState(true);
                mat.SetBrightness((BrightnessMode)brightness);

                switch (running)
                {
                    case 2:
                        SetMatrixPicture(AppConfig.getConfigString("matrix_picture"));
                        break;
                    case 3:
                        StartMatrixTimer(1000);
                        Logger.WriteLine("Matrix Clock");
                        break;
                    case 4:
                        SetMatrixAudio();
                        break;
                    default:
                        mat.SetBuiltInAnimation(true, animation);
                        Logger.WriteLine("Matrix builtin " + animation.AsByte);
                        break;

                }

                //mat.SetBrightness((BrightnessMode)brightness);
            }

        }
        private static void StartMatrixTimer(int interval = 100)
        {
            matrixTimer.Interval = interval;
            matrixTimer.Enabled = true;
        }

        private static void StopMatrixTimer()
        {
            matrixTimer.Enabled = false;
        }


        private static void MatrixTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (!IsValid) return;

            switch (AppConfig.getConfig("matrix_running"))
            {
                case 2:
                    mat.PresentNextFrame();
                    break;
                case 3:
                    mat.PresentClock();
                    break;
            }

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
        }

        void SetMatrixAudio()
        {
            if (!IsValid) return;

            mat.SetBuiltInAnimation(false);
            StopMatrixTimer();
            StopMatrixAudio();

            try
            {
                using (var enumerator = new MMDeviceEnumerator())
                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console))
                {
                    AudioDevice = new WasapiLoopbackCapture(device);
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
                    mat.SetLedPlanar(x + dx, dy + y, (byte)(h * 255 / 30));
                    mat.SetLedPlanar(x + dx, dy - y, 255);
                }
        }

        void PresentAudio(double[] audio)
        {

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastPresent) < 70) return;
            lastPresent = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            mat.Clear();

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

            for (int i = 0; i < size; i++) DrawBar(20 - i, bars[i]*20/maxAverage);

            mat.Present();
        }


        public void SetMatrixPicture(string fileName)
        {

            if (!IsValid) return;
            StopMatrixTimer();

            Image image;

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    image = Image.FromStream(ms);
                }
            }
            catch
            {
                Debug.WriteLine("Error loading picture");
                return;
            }

            mat.SetBuiltInAnimation(false);
            mat.ClearFrames();

            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            int frameCount = image.GetFrameCount(dimension);

            if (frameCount > 1)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    image.SelectActiveFrame(dimension, i);
                    mat.GenerateFrame(image);
                    mat.AddFrame();
                }

                StartMatrixTimer();
                Logger.WriteLine("Matrix GIF " + fileName);
            }
            else
            {
                mat.GenerateFrame(image);
                mat.Present();
                Logger.WriteLine("Matrix " + fileName);
            }
        }


    }
}
