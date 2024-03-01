using System.Text.Json;
using GHelper.AutoTDP.FramerateSource;
using GHelper.AutoTDP.PowerLimiter;

namespace GHelper.AutoTDP
{
    internal class AutoTDPService : IDisposable
    {

        private static readonly int INTERVAL_APP_CHECK = 5_000;
        private static readonly int INTERVAL_FPS_CHECK = 2_500;

        string GameProfileFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper\\AutoTDP.json";

        IFramerateSource? framerateSouce;
        IPowerLimiter? powerLimiter;

        public List<GameProfile> GameProfiles = new List<GameProfile>();

        private bool Running = false;
        private Thread? checkerThread;
        private Thread? tdpThread;

        private double GameFPSPrevious = double.NaN;
        private double GameFPS;

        private int FramerateTargetReachedCounter;
        private int FramerateDipCounter;

        private static readonly int FPSDipHistorySize = 8;

        private List<double> FramerateLog = new List<double>();

        private double CurrentTDP;

        private GameInstance? currentGame;

        public AutoTDPService()
        {
            LoadGameProfiles();

            Start();
        }

        /// <summary>
        /// Whether the system is enabled and currently running.
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return Running;
        }

        /// <summary>
        /// Whether a supported game is actively monitored and TDP is adjusted
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return currentGame is not null;
        }

        public void Start()
        {
            if (!IsEnabled() || IsRunning())
            {
                return;
            }

            Running = true;

            InitFramerateSource();

            InitLimiter();


            checkerThread = new Thread(() =>
            {
                while (Running)
                {
                    CheckForGame();
                    try
                    {
                        Thread.Sleep(INTERVAL_APP_CHECK);
                    }
                    catch (ThreadInterruptedException)
                    {
                        continue;
                    }
                }
            });
            checkerThread.Start();
        }

        public bool IsEnabled()
        {
            return AppConfig.Get("auto_tdp_enabled", 0) == 1;
        }

        public void InitFramerateSource()
        {
            string? source = AppConfig.GetString("auto_tdp_fps_source");

            if ((source is null || source.Equals("rtss")) && RTSSFramerateSource.IsAvailable())
            {
                RTSSFramerateSource rtss = new RTSSFramerateSource();
                RTSSFramerateSource.Start();
                framerateSouce = rtss;
                return;
            }
        }

        public void InitLimiter()
        {
            string? limiter = AppConfig.GetString("auto_tdp_limiter");

            if (limiter is null || limiter.Equals("asus_acpi"))
            {
                powerLimiter = new ASUSACPIPowerLimiter();
                return;
            }

            if (limiter is not null && limiter.Equals("intel_msr"))
            {
                powerLimiter = new IntelMSRPowerLimiter();
                return;
            }
        }

        public void SaveGameProfiles()
        {
            string json = JsonSerializer.Serialize(GameProfiles);

            File.WriteAllText(GameProfileFile, json);
        }


        public void LoadGameProfiles()
        {
            if (!File.Exists(GameProfileFile))
            {
                if (GameProfiles is null) GameProfiles = new List<GameProfile>();
                return;
            }

            string? json = File.ReadAllText(GameProfileFile);

            if (json == null)
            {
                return;
            }

            try
            {
                GameProfiles = JsonSerializer.Deserialize<List<GameProfile>>(json);
            }
            catch (Exception e)
            {
                Logger.WriteLine("[AutoTDPService] Deserialization failed. Creating empty list. Message: " + e.Message);
                GameProfiles = new List<GameProfile>();
            }

        }

        public void CheckForGame()
        {
            if (currentGame is not null)
            {
                //Already handling a running game. No need to check for other games
                return;
            }
            List<GameInstance> runningGames = framerateSouce.GetRunningGames();

            if (runningGames.Count == 0)
            {
                Logger.WriteLine("[AutoTDPService] No games detected");
                return;
            }

            foreach (GameInstance gi in runningGames)
            {
                Logger.WriteLine("[AutoTDPService] Detected App: " + gi.ProcessName + "  PID: " + gi.ProcessID);

                if (IsGameInList(gi.ProcessName))
                {
                    Logger.WriteLine("[AutoTDPService] Detected Supported Game: " + gi.ProcessName + "  PID: " + gi.ProcessID);
                    HandleGame(gi);
                    return;
                }
            }
        }

        public GameProfile? ProfileForGame(String? processName)
        {
            if (processName is null)
            {
                return null;
            }

            foreach (GameProfile gp in GameProfiles)
            {
                if (gp.ProcessName is not null && processName.EndsWith(gp.ProcessName))
                {
                    return gp;
                }
            }

            return null;
        }

        public bool IsGameInList(String? processName)
        {
            return ProfileForGame(processName) is not null;
        }


        public void HandleGame(GameInstance instance)
        {
            if (currentGame is not null)
            {
                Logger.WriteLine("[AutoTDPService] Already handling a game");
                return;
            }

            if (tdpThread is not null)
            {
                tdpThread.Join();
                tdpThread = null;
            }
            currentGame = instance;

            StartGameHandler(instance);
        }

        public void Reset()
        {
            currentGame = null;
            GameFPSPrevious = double.NaN;
            GameFPS = 0;

            if (powerLimiter is not null)
            {
                powerLimiter.ResetPowerLimits();
                CurrentTDP = powerLimiter.GetCPUPowerLimit();
            }
        }

        public void StartGameHandler(GameInstance instance)
        {
            GameProfile? profile = ProfileForGame(instance.ProcessName);
            if (profile is null || powerLimiter is null || framerateSouce is null)
            {
                return;
            }

            tdpThread = new Thread(() =>
            {
                CurrentTDP = powerLimiter.GetCPUPowerLimit();
                while (currentGame is not null && Running)
                {
                    GameFPS = framerateSouce.GetFramerate(instance);


                    Logger.WriteLine("[AutoTDPService] (" + instance.ProcessName + ") Framerate " + GameFPS);

                    if (GameFPS < 0.0d)
                    {
                        //Game is not running anymore or RTSS lost its hook
                        Reset();
                        return;
                    }
                    AdjustPowerLimit(profile);

                    try
                    {
                        Thread.Sleep(INTERVAL_FPS_CHECK);
                    }
                    catch (ThreadInterruptedException)
                    {
                        continue;
                    }

                }
            });
            tdpThread.Start();
        }

        private double FPSDipCorrection(double currentFramerate, double targetFPS)
        {
            double correction = 0.0d;


            FramerateLog.Insert(0, currentFramerate);

            //Remove last entry when exceeding the desired size.
            if (FramerateLog.Count > FPSDipHistorySize)
            {
                FramerateLog.RemoveAt(FramerateLog.Count - 1);
            }

            if (targetFPS - 1 <= currentFramerate && currentFramerate <= targetFPS + 1)
            {
                FramerateTargetReachedCounter++;

                if (FramerateTargetReachedCounter >= 4
                    && FramerateTargetReachedCounter < FPSDipHistorySize
                    && targetFPS - 0.75 <= FramerateLog.Take(4).Average()
                    && FramerateLog.Take(3).Average() <= targetFPS + 0.15)
                {
                    //short dip
                    FramerateDipCounter++;
                    correction = targetFPS + 0.75 - currentFramerate;
                }
                else if (FramerateDipCounter >= 5
                    && targetFPS - 0.75 <= FramerateLog.Average()
                         && FramerateLog.Average() <= targetFPS + 0.15)
                {
                    //long dip
                    correction = targetFPS + 1.5 - currentFramerate;
                    FramerateTargetReachedCounter = FPSDipHistorySize;
                }
            }
            else
            {
                //No dip, no correction
                correction = 0.0;
                FramerateTargetReachedCounter = 0;
                FramerateDipCounter = 0;
            }

            return correction;
        }


        private double TDPDamper(double currentFramerate)
        {
            if (double.IsNaN(GameFPSPrevious)) GameFPSPrevious = currentFramerate;
            double dF = -0.1d;

            // Calculation
            double deltaError = currentFramerate - GameFPSPrevious;
            double dT = deltaError / (1010.0 / 1000.0);
            double damping = CurrentTDP / currentFramerate * dF * dT;

            GameFPSPrevious = currentFramerate;

            return damping;
        }

        public void AdjustPowerLimit(GameProfile profile)
        {

            if (powerLimiter is null)
            {
                //Should not happen... but we also don't want it to crash
                return;
            }

            double newPL = CurrentTDP;


            Logger.WriteLine("[AutoTDPService] Current: " + (int)GameFPS + "FPS");


            double delta = profile.TargetFPS - GameFPS - FPSDipCorrection(GameFPS, profile.TargetFPS);
            delta = Math.Clamp(delta, -15, 15);


            double adjustment = (delta * CurrentTDP / GameFPS) * 0.85;
            //Dampen the changes to not change TDP too aggressively which would cause performance issues
            adjustment += TDPDamper(GameFPS);


            newPL += adjustment;

            //Respect the limits that the user chose
            newPL = Math.Clamp(newPL, profile.MinTdp, profile.MaxTdp);

            Logger.WriteLine("[AutoTDPService] Setting Power Limit from " + CurrentTDP + "W to " + newPL + "W, Delta:" + adjustment);

            //We only limit to full watts, no fractions. In this case, we will cut off the fractional part
            powerLimiter.SetCPUPowerLimit((int)newPL);
            CurrentTDP = newPL;
        }

        public void StopGameHandler()
        {
            if (tdpThread is not null)
            {
                currentGame = null;
                tdpThread.Join();
                tdpThread = null;
            }

        }

        public void Shutdown()
        {
            Running = false;

            if (checkerThread is not null)
            {
                checkerThread.Interrupt();
                checkerThread.Join();
            }


            if (tdpThread is not null)
            {
                tdpThread.Interrupt();
                tdpThread.Join();
            }

            if (powerLimiter is not null)
            {
                powerLimiter.ResetPowerLimits();
                powerLimiter.Dispose();
                powerLimiter = null;
            }

            if (framerateSouce is not null)
            {
                framerateSouce = null;
            }

            //Kill RTSS instance if we started one
            RTSSFramerateSource.Stop();

        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
