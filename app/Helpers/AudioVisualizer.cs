using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace GHelper.Helpers
{
    public class AudioVisualizer : IMMNotificationClient
    {
        public static readonly AudioVisualizer Shared = new();

        private readonly HashSet<Action<double[]>> subscribers = new();
        private volatile Action<double[]>[] subscriberSnapshot = Array.Empty<Action<double[]>>();

        private double[]? audioValues;
        private WasapiCapture? capture;
        private string? captureDeviceId;
        private MMDeviceEnumerator? enumerator;

        private readonly object _lock = new();
        private volatile bool _running;
        private volatile bool _stopping;

        public bool IsRunning => _running;

        public bool Subscribe(Action<double[]> handler)
        {
            lock (_lock)
            {
                if (subscribers.Contains(handler)) return true;
                if (subscribers.Count == 0 && !StartCapture()) return false;
                subscribers.Add(handler);
                subscriberSnapshot = subscribers.ToArray();
                return true;
            }
        }

        public void Unsubscribe(Action<double[]> handler)
        {
            lock (_lock)
            {
                if (!subscribers.Remove(handler)) return;
                subscriberSnapshot = subscribers.ToArray();
                if (subscribers.Count == 0) StopCapture();
            }
        }

        private bool StartCapture()
        {
            if (_running) return true;
            _stopping = false;

            try
            {
                enumerator = new MMDeviceEnumerator();
                enumerator.RegisterEndpointNotificationCallback(this);

                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console))
                {
                    capture = new WasapiLoopbackCapture(device);
                    captureDeviceId = device.ID;

                    var fmt = capture.WaveFormat;
                    audioValues = new double[fmt.SampleRate / 1000];

                    capture.DataAvailable += Capture_DataAvailable;
                    capture.StartRecording();
                }

                _running = true;
                Logger.WriteLine("AudioVisualizer: subscribed to default output");
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("AudioVisualizer: " + ex);
                Cleanup();
                return false;
            }
        }

        private void StopCapture()
        {
            _stopping = true;
            _running = false;
            Cleanup();
            _stopping = false;
        }

        private void Cleanup()
        {
            if (enumerator is not null)
            {
                try { enumerator.UnregisterEndpointNotificationCallback(this); }
                catch (Exception ex) { Logger.WriteLine("AudioVisualizer: unregister failed: " + ex); }
            }

            if (capture is not null)
            {
                try
                {
                    capture.DataAvailable -= Capture_DataAvailable;
                    capture.StopRecording();
                    capture.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("AudioVisualizer: dispose failed: " + ex);
                }
                capture = null;
            }

            captureDeviceId = null;

            if (enumerator is not null)
            {
                try { enumerator.Dispose(); } catch { /* ignore */ }
                enumerator = null;
            }
        }

        private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (capture is null || audioValues is null) return;

            int bytesPerSamplePerChannel = capture.WaveFormat.BitsPerSample / 8;
            int bytesPerSample = bytesPerSamplePerChannel * capture.WaveFormat.Channels;
            int bufferSampleCount = e.Buffer.Length / bytesPerSample;
            if (bufferSampleCount > audioValues.Length) bufferSampleCount = audioValues.Length;

            if (bytesPerSamplePerChannel == 2 && capture.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                for (int i = 0; i < bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
            else if (bytesPerSamplePerChannel == 4 && capture.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                for (int i = 0; i < bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
            else if (bytesPerSamplePerChannel == 4 && capture.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                for (int i = 0; i < bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);

            double[] padded = FftSharp.Pad.ZeroPad(audioValues);
            var fft = FftSharp.FFT.Forward(padded);
            double[] mag = FftSharp.FFT.Magnitude(fft);

            foreach (var sub in subscriberSnapshot)
            {
                try { sub.Invoke(mag); }
                catch (Exception ex) { Logger.WriteLine("AudioVisualizer: subscriber threw: " + ex); }
            }
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState) { }
        public void OnDeviceAdded(string pwstrDeviceId) { }
        public void OnDeviceRemoved(string deviceId) { }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (!_running || _stopping) return;
            if (flow != DataFlow.Render || role != Role.Console) return;

            var current = captureDeviceId;
            if (!string.IsNullOrEmpty(current) && current == defaultDeviceId) return;

            Logger.WriteLine("AudioVisualizer: default output changed -> " + defaultDeviceId);
            captureDeviceId = defaultDeviceId;

            Task.Delay(50).ContinueWith(_ =>
            {
                lock (_lock)
                {
                    if (subscribers.Count == 0) return;
                    StopCapture();
                    StartCapture();
                }
            });
        }
    }

    // envelope and hue state for audio driven lighting, one instance per device
    public class AudioLevels
    {
        public const int Bands = 8;

        public double[] Bars { get; } = new double[Bands];
        public double Scale { get; private set; } = 1;
        public double Brightness { get; private set; }

        private readonly List<double> maxes = new();
        private readonly double decay = AppConfig.Get("audio_decay", 70) / 100.0;
        public double DecayMultiplier { get; init; } = 1;
        private long lastPresent;
        private double peak;
        private double envelope;
        private double smoothedHue;

        public void Reset()
        {
            maxes.Clear();
            lastPresent = 0;
            envelope = 0;
            smoothedHue = 0;
        }

        public bool Update(double[] fftMag)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (Math.Abs(now - lastPresent) < 50) return false;
            lastPresent = now;

            if (fftMag.Length < Bands) return false;

            peak = 0;
            for (int i = 0; i < Bands; i++)
            {
                Bars[i] = Math.Sqrt(fftMag[i] * 10000);
                if (Bars[i] > peak) peak = Bars[i];
            }

            maxes.Add(peak);
            if (maxes.Count > 100) maxes.RemoveAt(0);
            Scale = Math.Max(1, maxes.OrderByDescending(x => x).ElementAt(maxes.Count / 10));

            envelope = Math.Max(envelope * decay * DecayMultiplier, peak);
            double brightness = Math.Min(1.0, envelope / Scale);
            Brightness = brightness * brightness * brightness;
            return true;
        }

        public Color DominantColor(Color baseColor)
        {
            int dominant = 1;
            double dominantWeighted = Bars[1];
            for (int i = 2; i < Bands; i++)
            {
                double w = Bars[i] * (1 + (i - 1) * 0.15);
                if (w > dominantWeighted) { dominantWeighted = w; dominant = i; }
            }

            if (peak > Scale * 0.3)
            {
                double baseHue = ColorUtils.HSV.ToHSV(baseColor).Hue;
                double targetHue = (baseHue + (dominant - 1) / (double)(Bands - 2) * (2.0 / 3.0)) % 1.0;
                smoothedHue = smoothedHue * 0.6 + targetHue * 0.4;
            }

            return new ColorUtils.HSV { Hue = smoothedHue, Saturation = 1.0, Value = Brightness }.ToRGB();
        }
    }
}
