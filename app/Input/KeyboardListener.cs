using GHelper.USB;
using HidSharp;
using System.Text;

namespace GHelper.Input
{
    public class KeyboardListener
    {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Action<int> _handler;

        static int retry = 0;

        public KeyboardListener(Action<int> KeyHandler)
        {
            _handler = KeyHandler;
            var task = Task.Run(Listen);
        }

        private void Listen()
        {

            HidStream? input = AsusHid.FindHidStream(AsusHid.INPUT_ID);

            // Fallback
            int count = 0;

            while (input == null && count++ < 10)
            {
                Thread.Sleep(1000);
                input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
            }

            if (input == null)
            {
                Logger.WriteLine($"Input device not found");
                return;
            }

            AsusHid.WriteInput(Encoding.ASCII.GetBytes("ZASUS Tech.Inc."));
            Logger.WriteLine($"Input: {input.Device.DevicePath}");

            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {

                    // Emergency break
                    if (input == null || !input.CanRead)
                    {
                        Logger.WriteLine("Listener terminated");
                        break;
                    }

                    input.ReadTimeout = int.MaxValue;

                    var data = input.Read();
                    if (cancellationTokenSource.Token.IsCancellationRequested) break;
                    if (data.Length > 1 && data[0] == AsusHid.INPUT_ID && data[1] > 0 && data[1] != 236)
                    {
                        Logger.WriteLine($"Key: {data[1]}");
                        _handler(data[1]);
                    }
                }

                Logger.WriteLine("Listener stopped");

            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Listener exited: {ex.Message}");
                if (retry++ < 2)
                {
                    Thread.Sleep(300);
                    Logger.WriteLine($"Restarting listener {retry}");
                    Listen();
                }
            }

        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
