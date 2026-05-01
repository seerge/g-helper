using GHelper.USB;
using HidSharp;

namespace GHelper.Input
{
    public class AuraListener
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        HidStream? input;
        Action<bool>? _winLockHandler;

        public AuraListener(Action<bool>? winLockHandler = null)
        {
            _winLockHandler = winLockHandler;
            Task.Run(Listen);
        }

        private void Listen()
        {
            input = AsusHid.FindHidStream(AsusHid.AURA_ID);
            if (input == null)
            {
                Logger.WriteLine("Aura input device not found");
                return;
            }

            Logger.WriteLine($"Aura: {input.Device.DevicePath}");
            input.ReadTimeout = int.MaxValue;

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var data = input.Read();
                    if (cts.Token.IsCancellationRequested) break;
                    if (data.Length >= 5 && data[0] == AsusHid.AURA_ID && data[1] == 0xBF && data[2] == 0x01 && data[4] == 0x00)
                    {
                        bool locked = data[3] == 0x01;
                        Logger.WriteLine($"WinLock: {(locked ? "On" : "Off")}");
                        _winLockHandler?.Invoke(locked);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Aura listener exited: {ex.Message}");
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            input?.Dispose();
        }
    }
}
