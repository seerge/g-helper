using System.Text.Json;
using GHelper.AutoTDP.FramerateSource;
using GHelper.AutoTDP.PowerLimiter;
using Ryzen;

namespace GHelper.AutoTDP
{
    internal class AutoTDPService : IDisposable
    {

        private static readonly bool LOG_AUTO_TDP = false;
        private static readonly int INTERVAL_MIN_CHECK = 30 * 1_000;
        private static readonly int INTERVAL_APP_CHECK = 5_000;
        private static readonly int INTERVAL_FPS_CHECK = 500;

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

        private static readonly int FPSDipHistorySize = 6;

        private List<double> FramerateLog = new List<double>();

        private double LowestTDP;
        private double LowestStableTDP;
        private long LowestStableStability = 0;
        private int LowestStableChecks = 0;
        private double CurrentTDP;
        private double LastAdjustment;
        private double LastAdjustmentTotal = 0;
        private int LastAdjustmentsWithoutImprovement = 0;

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

        public static bool IsAvailable()
        {

            if (AppConfig.IsAlly())
            {
                //Not yet supported
                return false;
            }

            int availableFS = 0;
            int availablePL = 0;

            if (RTSSFramerateSource.IsAvailable()) availableFS++;

            //Intel MSR Limiter is available on Intel only
            if (!RyzenControl.IsAMD()) availablePL++;

            //ASUS ACPI Power limiter is available
            if (AppConfig.IsASUS()) availablePL++;

            return availablePL > 0 && availableFS > 0;
        }

        public void Start()
        {
            if (!IsEnabled() || IsRunning() || !IsAvailable())
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
                if (LOG_AUTO_TDP)
                    Logger.WriteLine("[AutoTDPService] No games detected");
                return;
            }

            foreach (GameInstance gi in runningGames)
            {
                if (LOG_AUTO_TDP)
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
                if (gp.ProcessName is not null && processName.EndsWith(gp.ProcessName, StringComparison.CurrentCultureIgnoreCase))
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
                if (LOG_AUTO_TDP)
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
            LastAdjustmentsWithoutImprovement = 0;
            LastAdjustment = 0.0;
            FramerateLog = new List<double>();
            FramerateTargetReachedCounter = 0;
            FramerateDipCounter = 0;
            LowestStableStability = 0;
            LowestStableChecks = 0;

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
                powerLimiter.SavePowerLimits(); // save current power limits to restore them afterwards

                LowestStableTDP = profile.MaxTdp;
                LowestTDP = profile.MaxTdp;

                while (currentGame is not null && Running)
                {

                    double fps = framerateSouce.GetFramerate(instance);

                    if (LOG_AUTO_TDP)
                        Logger.WriteLine("[AutoTDPService] (" + instance.ProcessName + ") Framerate " + GameFPS);

                    if (fps < 0.0d)
                    {
                        //Game is not running anymore or RTSS lost its hook
                        Reset();
                        return;
                    }

                    //prevent FPS from going to 0 which causes issues with the math
                    GameFPS = Math.Max(5, fps);
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
                //Framerate is inside ideal range
                FramerateTargetReachedCounter++;

                if (FramerateTargetReachedCounter >= 3
                    && FramerateTargetReachedCounter < FPSDipHistorySize
                    && targetFPS - 0.5 <= FramerateLog.Take(3).Average()
                    && FramerateLog.Take(3).Average() - 0.05 <= targetFPS)
                {
                    //short dip
                    FramerateDipCounter++;
                    FramerateUnstable();

                    correction = targetFPS + 0.15 - currentFramerate;
                }
                else if (FramerateDipCounter >= 4
                    && targetFPS - 0.5 <= FramerateLog.Average()
                         && FramerateLog.Average() - 0.1 <= targetFPS)
                {
                    //long dip
                    correction = targetFPS + 0.35 - currentFramerate;
                    FramerateTargetReachedCounter = FPSDipHistorySize;
                    FramerateVeryUnstable();
                }
                else
                {
                    FramerateStable();
                }
            }
            else
            {
                //Framerate not in target range
                correction = 0.0;
                FramerateTargetReachedCounter = 0;
                FramerateDipCounter = 0;
                FramerateStable();
            }

            ProcessStability();
            return correction;
        }

        private void FramerateStable()
        {
            LowestStableStability++;
        }

        private void FramerateUnstable()
        {
            LowestStableStability -= 3;
        }
        private void FramerateVeryUnstable()
        {
            LowestStableStability -= 30;
        }

        private bool Stabilize()
        {
            return LowestStableChecks * INTERVAL_FPS_CHECK > INTERVAL_MIN_CHECK;
        }

        private void ProcessStability()
        {
            if (!Stabilize()) LowestStableChecks++;

            if (LowestStableStability < 0 && Stabilize())
            {
                //If unstable for too often increase lowest stable TDP
                LowestStableTDP += 1;
                LowestTDP += 1;
                LowestStableStability = 0;
                return;
            }

            if (CurrentTDP > LowestStableTDP - 0.1 && CurrentTDP < LowestStableTDP + 0.1 && Stabilize())
            {
                LowestStableStability++;

                if (LowestStableStability > 120)
                {
                    //if stable for long time try to reduce it a bit
                    LowestStableTDP -= 0.5;
                    LowestStableStability = 0;
                }
            }

            if (LowestTDP - 0.25 <= CurrentTDP && CurrentTDP >= LowestTDP + 0.25)
            {
                LowestStableStability++;

                if (LowestStableStability > 10 && Stabilize())
                {
                    LowestStableTDP = LowestTDP + (LowestTDP * 0.10); // Add 10% additional wattage to get a smoother framerate
                }

            }

            if (CurrentTDP < LowestTDP - 0.1 && LowestStableStability > 0)
            {
                LowestStableStability = 0;
                LowestTDP = CurrentTDP;
            }

            LowestStableStability = Math.Min(LowestStableStability, 150);
        }


        private double TDPDamper(double currentFramerate)
        {
            if (double.IsNaN(GameFPSPrevious)) GameFPSPrevious = currentFramerate;
            double dF = -0.12d;

            // PID Compute
            double deltaError = currentFramerate - GameFPSPrevious;
            double dT = deltaError / (1020.0 / 1000.0);
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
            double fpsCorrection = FPSDipCorrection(GameFPS, profile.GetTDPFPS());
            double delta = profile.GetTDPFPS() - GameFPS - fpsCorrection - 1;
            delta = Math.Clamp(delta, -15, 15);


            double adjustment = (delta * CurrentTDP / GameFPS) * 0.65;
            //Dampen the changes to not change TDP too aggressively which would cause performance issues
            adjustment += TDPDamper(GameFPS);

            adjustment = Math.Min(adjustment, (CurrentTDP * 0.1));

            if (GameFPSPrevious > profile.GetTDPFPS() && GameFPS < profile.GetTDPFPS())
            {
                if (LOG_AUTO_TDP)
                    Logger.WriteLine("[AutoTDPService] Single Dip, Ignore");
                //single dip. Ignore
                return;
            }

            if (LastAdjustment > 0 && GameFPS <= GameFPSPrevious && adjustment > 0)
            {
                LastAdjustmentsWithoutImprovement++;
                LastAdjustmentTotal += adjustment;

                //Wait for 3 consecutive power increases and at least 3W increased TDP before judging that increasing power does nothing.
                if (LastAdjustmentsWithoutImprovement >= 3 && LastAdjustmentTotal > 3)
                {
                    //Do not adjust if increasing power does not improve framerate.
                    if (LOG_AUTO_TDP)
                        Logger.WriteLine("[AutoTDPService] Not adjusting because no improvement from last increase");
                    return;
                }

            }
            else
            {
                LastAdjustmentsWithoutImprovement = 0;
                LastAdjustmentTotal = 0;
            }

            newPL += adjustment;
            LastAdjustment = adjustment;

            //Respect the limits that the user chose
            newPL = Math.Clamp(newPL, profile.MinTdp, profile.MaxTdp);

            if (newPL < LowestStableTDP && LowestStableTDP < profile.MaxTdp - 1)
            {
                newPL = LowestStableTDP;
            }

            if (LOG_AUTO_TDP)
                Logger.WriteLine("[AutoTDPService] Power Limit from " + CurrentTDP + "W to " + newPL + "W, Delta:" + adjustment
                    + " Lowest: " + LowestTDP + "W, Lowest Stable(" + LowestStableStability + "): " + LowestStableTDP + "W");

            //Apply power limits
            powerLimiter.SetCPUPowerLimit(newPL);
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

            currentGame = null;
            Reset();

            //Kill RTSS instance if we started one
            RTSSFramerateSource.Stop();

        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
