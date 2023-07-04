using HidLibrary;

namespace GHelper.Input
{
    public class KeyboardListener
    {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public KeyboardListener(Action<int> KeyHandler)
        {
            HidDevice? input = AsusUSB.GetDevice();
            
            // Fallback
            if (input == null)
            {
                AsusUSB.Init();
                Thread.Sleep(1000);
                input = AsusUSB.GetDevice();
            }

            if (input == null)
            {
                Logger.WriteLine($"Input device not found");
                return;
            }

            Logger.WriteLine($"Input: {input.DevicePath}");

            var task = Task.Run(() =>
            {
                try
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {

                        // Emergency break
                        if (input == null || !input.IsConnected)
                        {
                            Logger.WriteLine("Listener terminated");
                            break;
                        }

                        var data = input.Read().Data;
                        if (data.Length > 1 && data[0] == AsusUSB.INPUT_HID_ID && data[1] > 0 && data[1] != 236)
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
