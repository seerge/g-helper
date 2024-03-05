using System.Diagnostics;
using RTSSSharedMemoryNET;

namespace GHelper.AutoTDP.FramerateSource
{
    internal class RTSSFramerateSource : IFramerateSource
    {
        private static Process? rtssInstance;

        private static OSD? osd;

        public static string RTSSPath { get; set; }


        public static bool IsRunning => Process.GetProcessesByName("RTSS").Length != 0;


        static RTSSFramerateSource()
        {
            RTSSPath = @"C:\Program Files (x86)\RivaTuner Statistics Server\RTSS.exe";
        }

        public static bool IsAvailable()
        {
            return File.Exists(RTSSPath);

        }

        public static void Start()
        {
            if ((rtssInstance == null || rtssInstance.HasExited) && !IsRunning && File.Exists(RTSSPath))
            {
                try
                {
                    rtssInstance = Process.Start(RTSSPath);
                    Thread.Sleep(2000); // If it works, don't touch it
                }
                catch (Exception exc)
                {
                    Logger.WriteLine("Could not start RTSS Service" + exc.Message);
                }

                RunOSD();
            }
            else
            {
                RunOSD();
            }
        }

        public List<GameInstance> GetRunningGames()
        {
            if (!IsRunning)
            {
                Start();
            }

            List<GameInstance> giL = new List<GameInstance>();

            foreach (AppEntry appEntry in OSD.GetAppEntries())
            {
                GameInstance i = new GameInstance();
                i.ProcessID = appEntry.ProcessId;
                i.ProcessName = appEntry.Name;

                giL.Add(i);
            }

            return giL;
        }

        public double GetFramerate(GameInstance instance)
        {
            if (!IsRunning)
            {
                return -1.0d;
            }

            try
            {
                var appE = OSD.GetAppEntries().FirstOrDefault(a => a.ProcessId == instance.ProcessID);
                if (appE is null)
                    return -1.0d;

                return (double)appE.StatFrameTimeBufFramerate / 10;
            }
            catch (InvalidDataException)
            {
            }
            catch (FileNotFoundException)
            {
            }

            return -1.0d;
        }

        public static void RunOSD()
        {
            if (osd == null)
            {
                try
                {
                    osd = new OSD("GHELPER");
                }
                catch (Exception exc)
                {
                    Logger.WriteLine("Could not start OSD" + exc.Message);
                }
            }
        }

        public static void Stop()
        {
            if (rtssInstance != null && !rtssInstance.HasExited)
            {
                try
                {
                    rtssInstance.Kill();
                    rtssInstance = null;
                    var proc = Process.GetProcessesByName("RTSSHooksLoader64");
                    proc[0].Kill();
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
        }
    }
}
