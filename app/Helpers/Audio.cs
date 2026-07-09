using NAudio.CoreAudioApi;

namespace GHelper.Helpers
{
    internal class Audio
    {
        static AudioEndpointVolume? endpointVolume; // keeps the COM callback alive

        public static void SubscribeMute(Action<bool> onMuteChange)
        {
            try
            {
                using var enumerator = new MMDeviceEnumerator();
                endpointVolume = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioEndpointVolume;
                endpointVolume.OnVolumeNotification += (data) => onMuteChange(data.Muted);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error subscribing to volume notifications: " + ex.Message);
            }
        }

        public static bool ToggleMicMute()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var commDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                var consoleDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                var mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

                bool status = !commDevice.AudioEndpointVolume.Mute;
                
                commDevice.AudioEndpointVolume.Mute = status;
                consoleDevice.AudioEndpointVolume.Mute = status;
                mmDevice.AudioEndpointVolume.Mute = status;

                Logger.WriteLine(commDevice.ToString() + ":" + status);
                Logger.WriteLine(consoleDevice.ToString() + ":" + status);
                Logger.WriteLine(mmDevice.ToString() + ":" + status);

                return status;
            }
        }

        public static bool IsMicMuted()
        {
            try
            {
                using (var deviceEnumerator = new MMDeviceEnumerator())
                {
                    var commDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                    return commDevice.AudioEndpointVolume.Mute;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error checking mic mute status: " + ex.Message);
                return false; // Assume not muted in case of error
            }
        }

        public static bool IsMuted()
        {
            try
            {
                using (var deviceEnumerator = new MMDeviceEnumerator())
                {
                    var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    return defaultDevice.AudioEndpointVolume.Mute;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error checking mute status: " + ex.Message);
                return false; // Assume not muted in case of error
            }
        }
    }
}
