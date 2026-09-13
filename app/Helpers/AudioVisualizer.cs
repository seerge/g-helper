using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public class AudioVisualizer : IMMNotificationClient
    {
        public static readonly AudioVisualizer Shared = new();

        // 订阅者收到的频谱为固定段数的对数分箱（40Hz ~ 20kHz）平均幅度
        public const int SPECTRUM_BANDS = 32;

        private readonly HashSet<Action<double[]>> subscribers = new();
        private volatile Action<double[]>[] subscriberSnapshot = Array.Empty<Action<double[]>>();

        private FastLoopbackCapture? capture;
        private string? captureDeviceId;
        private MMDeviceEnumerator? enumerator;

        private readonly object _lock = new();
        private volatile bool _running;
        private volatile bool _stopping;

        // 滚动频谱窗口
        private const int FFT_SIZE = 2048;          // ≈43ms @48kHz
        private const double F_MIN = 40;            // 对数分箱频率下限 Hz
        private const double F_MAX = 20000;         // 对数分箱频率上限 Hz
        private readonly double[] ring = new double[FFT_SIZE];
        private int ringCount;
        private int sampleRate;

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
                    capture = new FastLoopbackCapture(device, 20);
                    captureDeviceId = device.ID;
                    sampleRate = capture.WaveFormat.SampleRate;
                    ringCount = 0;

                    capture.DataAvailable += Capture_DataAvailable;
                    capture.StartRecording();
                }

                _running = true;
                Logger.WriteLine($"AudioVisualizer: subscribed to default output ({capture.WaveFormat.Encoding} {sampleRate}Hz)");
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
            if (capture is null) return;

            double[] samples = BytesToSamples(e.Buffer, e.BytesRecorded, capture.WaveFormat);
            if (samples.Length == 0) return;

            PushRing(samples);
            if (ringCount < FFT_SIZE) return;

            // 加 Hann 窗后做 FFT
            double[] windowed = new double[FFT_SIZE];
            Array.Copy(ring, windowed, FFT_SIZE);
            ApplyHann(windowed);

            var fft = FftSharp.FFT.Forward(windowed);
            double[] mag = FftSharp.FFT.Magnitude(fft);
            double[] bands = LogBands(mag, sampleRate);

            foreach (var sub in subscriberSnapshot)
            {
                try { sub.Invoke(bands); }
                catch (Exception ex) { Logger.WriteLine("AudioVisualizer: subscriber threw: " + ex); }
            }
        }

        private void PushRing(double[] samples)
        {
            if (samples.Length >= FFT_SIZE)
            {
                Array.Copy(samples, samples.Length - FFT_SIZE, ring, 0, FFT_SIZE);
                ringCount = FFT_SIZE;
            }
            else if (ringCount + samples.Length <= FFT_SIZE)
            {
                Array.Copy(samples, 0, ring, ringCount, samples.Length);
                ringCount += samples.Length;
            }
            else
            {
                // 溢出：挤掉头部旧样本，保留全部新样本
                int overflow = ringCount + samples.Length - FFT_SIZE;
                Array.Copy(ring, overflow, ring, 0, FFT_SIZE - samples.Length);
                Array.Copy(samples, 0, ring, FFT_SIZE - samples.Length, samples.Length);
                ringCount = FFT_SIZE;
            }
        }

        private static void ApplyHann(double[] samples)
        {
            int n = samples.Length;
            for (int i = 0; i < n; i++)
                samples[i] *= 0.5 * (1 - Math.Cos(2 * Math.PI * i / (n - 1)));
        }

        private double[] LogBands(double[] mag, int sampleRate)
        {
            double[] bands = new double[SPECTRUM_BANDS];
            int[] counts = new int[SPECTRUM_BANDS];

            double logMin = Math.Log10(F_MIN);
            double logSpan = Math.Log10(F_MAX) - logMin;
            double binFreq = (double)sampleRate / FFT_SIZE;

            for (int i = 1; i < mag.Length; i++)   // i=0 是 DC，跳过
            {
                double f = i * binFreq;
                if (f < F_MIN || f > F_MAX) continue;

                double t = (Math.Log10(f) - logMin) / logSpan;
                int s = (int)(t * SPECTRUM_BANDS);
                if (s >= SPECTRUM_BANDS) s = SPECTRUM_BANDS - 1;

                bands[s] += mag[i];
                counts[s]++;
            }

            for (int s = 0; s < SPECTRUM_BANDS; s++)
                if (counts[s] > 0) bands[s] /= counts[s];

            return bands;
        }

        private static double[] BytesToSamples(byte[] buffer, int bytesRecorded, WaveFormat fmt)
        {
            int bytesPerSamplePerChannel = fmt.BitsPerSample / 8;
            int bytesPerSample = bytesPerSamplePerChannel * fmt.Channels;
            if (bytesPerSample == 0) return Array.Empty<double>();

            // loopback 的 mix format 是 WaveFormatExtensible（Encoding=Extensible），
            // 需按 SubFormat GUID 判断 32bit 到底是 float 还是 int
            // (00000003-... = MEDIASUBTYPE_IEEE_FLOAT, 00000001-... = PCM)
            bool isFloat = fmt.Encoding == WaveFormatEncoding.IeeeFloat
                || (fmt.Encoding == WaveFormatEncoding.Extensible
                    && fmt is WaveFormatExtensible ext
                    && ext.SubFormat == new Guid("00000003-0000-0010-8000-00aa00389b71"));

            int count = bytesRecorded / bytesPerSample;
            double[] samples = new double[count];

            if (bytesPerSamplePerChannel == 2)
            {
                for (int i = 0; i < count; i++)
                    samples[i] = BitConverter.ToInt16(buffer, i * bytesPerSample);
            }
            else if (bytesPerSamplePerChannel == 3)
            {
                for (int i = 0; i < count; i++)
                {
                    int offset = i * bytesPerSample;
                    samples[i] = buffer[offset] | (buffer[offset + 1] << 8) | ((sbyte)buffer[offset + 2] << 16);
                }
            }
            else if (bytesPerSamplePerChannel == 4 && isFloat)
            {
                for (int i = 0; i < count; i++)
                    samples[i] = BitConverter.ToSingle(buffer, i * bytesPerSample);
            }
            else if (bytesPerSamplePerChannel == 4)
            {
                for (int i = 0; i < count; i++)
                    samples[i] = BitConverter.ToInt32(buffer, i * bytesPerSample);
            }

            return samples;
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

        /// <summary>
        /// 自写的 WASAPI loopback 采集器：把回调缓冲从默认 100ms 缩到可配置
        /// 毫秒数（默认 20ms），把灯效刷新率从 ~10fps 提到 ~50fps。
        /// </summary>
        private sealed class FastLoopbackCapture : IDisposable
        {
            private readonly AudioClient _audioClient;
            private readonly AudioCaptureClient _captureClient;
            private readonly int _bytesPerFrame;
            private readonly Thread _thread;
            private volatile bool _running;

            public WaveFormat WaveFormat { get; }

            public event EventHandler<WaveInEventArgs>? DataAvailable;

            public FastLoopbackCapture(MMDevice device, int bufferMilliseconds)
            {
                _audioClient = device.AudioClient;
                WaveFormat = _audioClient.MixFormat;
                _bytesPerFrame = WaveFormat.BitsPerSample / 8 * WaveFormat.Channels;

                _audioClient.Initialize(
                    AudioClientShareMode.Shared,
                    AudioClientStreamFlags.Loopback,
                    bufferMilliseconds * 10000L,     // 100ns 单位
                    0,                                // periodicity 0 = 用系统默认
                    WaveFormat,
                    Guid.Empty);

                _captureClient = _audioClient.AudioCaptureClient;
                _thread = new Thread(CaptureLoop) { IsBackground = true, Name = "GHelperAudio" };
            }

            public void StartRecording()
            {
                if (_running) return;
                _running = true;
                _audioClient.Start();
                _thread.Start();
            }

            public void StopRecording()
            {
                _running = false;
                try { _audioClient.Stop(); } catch { /* ignore */ }
            }

            private void CaptureLoop()
            {
                try
                {
                    while (_running)
                    {
                        // 一次唤醒尽量消费完积压的 packet，防止缓冲越堆越多
                        int processed = 0;
                        while (processed < 16 && _captureClient.GetNextPacketSize() > 0)
                        {
                            int frames;
                            AudioClientBufferFlags flags;
                            IntPtr bufferPtr = _captureClient.GetBuffer(out frames, out flags);

                            if (frames > 0)
                            {
                                int bytes = frames * _bytesPerFrame;
                                byte[] data = new byte[bytes];
                                Marshal.Copy(bufferPtr, data, 0, bytes);
                                DataAvailable?.Invoke(this, new WaveInEventArgs(data, bytes));
                                processed++;
                            }

                            _captureClient.ReleaseBuffer(frames);
                        }

                        Thread.Sleep(5);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("AudioVisualizer capture loop: " + ex.Message);
                }
            }

            public void Dispose()
            {
                StopRecording();
                try { _thread.Join(200); } catch { /* ignore */ }

                try { _captureClient.Dispose(); } catch { /* ignore */ }
                try { _audioClient.Dispose(); } catch { /* ignore */ }
            }
        }
    }
}
