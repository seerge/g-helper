using HidSharp;
using GHelper.USB;

namespace GHelper.Input
{
    public class KeyboardListener
    {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Action<int> _handler;

        public KeyboardListener(Action<int> KeyHandler)
        {
            _handler = KeyHandler;
            var task = Task.Run(Listen);
        }

        private void Listen () { 

            HidStream? input = AsusHid.FindHidStream(AsusHid.INPUT_ID);

            // Fallback

            int count = 0;

            while (input == null && count++ < 5)
            {
                Aura.Init();
                Thread.Sleep(2000);
                input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
            }

            if (input == null)
            {
                Logger.WriteLine($"Input device not found");
                return;
            }

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
                Logger.WriteLine(ex.ToString());
            }

        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
