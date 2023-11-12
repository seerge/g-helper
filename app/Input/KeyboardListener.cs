using HidSharp;
using GHelper.USB;

namespace GHelper.Input
{
    public class KeyboardListener
    {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public KeyboardListener(Action<int> KeyHandler)
        {
            HidStream? input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
            
            // Fallback
            if (input == null)
            {
                Aura.Init();
                Thread.Sleep(1000);
                input = input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
            }

            if (input == null)
            {
                Logger.WriteLine($"Input device not found");
                return;
            }

            input.ReadTimeout = int.MaxValue;

            Logger.WriteLine($"Input: {input.Device.DevicePath}");

            var task = Task.Run(() =>
            {
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


                        var data = input.Read();
                        if (data.Length > 1 && data[0] == AsusHid.INPUT_ID && data[1] > 0 && data[1] != 236)
                        {
                            Logger.WriteLine($"Key: {data[1]}");
                            KeyHandler(data[1]);
                        }
                    }

                    Logger.WriteLine("Listener stopped");

                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.ToString());
                }
            });


        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
