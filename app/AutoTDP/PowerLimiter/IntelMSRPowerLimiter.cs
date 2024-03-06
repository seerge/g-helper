using Ryzen;

namespace GHelper.AutoTDP.PowerLimiter
{

    internal class IntelMSRPowerLimiter : IPowerLimiter
    {
        public static readonly uint MSR_PKG_POWER_LIMIT = 0x610;
        public static readonly uint MSR_RAPL_POWER_UNIT = 0x606;

        private Ols ols;

        private static bool DRIVER_LOADED = false;

        private uint DefaultEax = 0; // Set on first reading
        private uint DefaultEdx = 0;

        //Lower 14 bits are the power limits
        private uint PL1_MASK = 0x3FFF;
        private uint PL2_MASK = 0x3FFF;

        //The power unit factor (Default is 0.125 for most Intel CPUs).
        private double PowerUnit = 0x0;

        public IntelMSRPowerLimiter()
        {
            ols = new Ols();
            ols.InitializeOls();
            DRIVER_LOADED = true;
            ReadPowerUnit();
            Logger.WriteLine("[AutoTDPService] Read MSR_RAPL_POWER_UNIT: " + PowerUnit);
        }

        public int GetMinInterval()
        {
            return 250;
        }

        public static bool IsAvailable()
        {
            if (RyzenControl.IsAMD())
            {
                return false;
            }

            if (DRIVER_LOADED)
            {
                return true;
            }

            Ols o = new Ols();

            o.InitializeOls();

            uint err = o.GetDllStatus();
            o.DeinitializeOls();
            o.Dispose();

            if (err == (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR)
            {
                return true;
            }

            LogOLSState(err);

            return false;
        }

        private static void LogOLSState(uint err)
        {
            switch (err)
            {
                case (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR:
                    return;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DRIVER_NOT_LOADED");
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNSUPPORTED_PLATFORM:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DLL_UNSUPPORTED_PLATFORM");
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_FOUND:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DLL_DRIVER_NOT_FOUND");
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_UNLOADED:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DLL_DRIVER_UNLOADED");
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK");
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNKNOWN_ERROR:
                    Logger.WriteLine("[AutoTDPService] Intel MSR Error: OLS_DLL_UNKNOWN_ERROR");
                    break;
            }
        }

        public void SavePowerLimits()
        {
            DefaultEax = 0;
            DefaultEdx = 0;

            if (ols.Rdmsr(MSR_PKG_POWER_LIMIT, ref DefaultEax, ref DefaultEdx) == 0)
            {
                Logger.WriteLine("[AutoTDPService] Failed to Read MSR_PKG_POWER_LIMIT");
                LogOLSState(ols.GetDllStatus());
            }
        }

        public void ReadPowerUnit()
        {
            uint eax = 0;
            uint edx = 0;

            if (ols.Rdmsr(MSR_RAPL_POWER_UNIT, ref eax, ref edx) == 0)
            {
                Logger.WriteLine("[AutoTDPService] Failed to Read MSR_RAPL_POWER_UNIT");
                LogOLSState(ols.GetDllStatus());
            }


            uint pwr = eax & 0x03;

            PowerUnit = 1 / Math.Pow(2, pwr);
        }

        public void SetCPUPowerLimit(double watts)
        {
            uint eax = 0;
            uint edx = 0;


            if (ols.Rdmsr(MSR_PKG_POWER_LIMIT, ref eax, ref edx) == 0)
            {
                Logger.WriteLine("[AutoTDPService] Failed to Read MSR_PKG_POWER_LIMIT");
                LogOLSState(ols.GetDllStatus());
            }

            uint watsRapl = (uint)(watts / PowerUnit);

            //Set limits for both PL1 and PL2
            uint eaxFilterd = eax & ~PL1_MASK;
            uint edxFilterd = edx & ~PL2_MASK;

            eaxFilterd |= watsRapl;
            edxFilterd |= watsRapl;

            //Enable clamping
            eaxFilterd |= 0x8000;
            edxFilterd |= 0x8000;

            if (ols.Wrmsr(0x610, eaxFilterd, edxFilterd) == 0)
            {
                Logger.WriteLine("[AutoTDPService] Failed to Write MSR_PKG_POWER_LIMIT");
                LogOLSState(ols.GetDllStatus());
            }
        }


        public int GetCPUPowerLimit()
        {
            uint eax = 0;
            uint edx = 0;

            if (ols.Rdmsr(MSR_PKG_POWER_LIMIT, ref eax, ref edx) == 0)
            {
                Logger.WriteLine("[AutoTDPService] Failed to Read MSR_PKG_POWER_LIMIT");
                LogOLSState(ols.GetDllStatus());
            }

            uint pl1 = eax & PL1_MASK;
            uint pl2 = edx & PL2_MASK;


            Logger.WriteLine("[AutoTDPService] Read Power Limit - PL1: " + pl1 + "W, PL2: " + pl2 + "W");

            return (int)(pl1 * PowerUnit);
        }


        public void ResetPowerLimits()
        {
            if (DefaultEax == 0)
            {
                return;
            }
            ols.Wrmsr(MSR_PKG_POWER_LIMIT, DefaultEax, DefaultEdx);
        }

        public void Dispose()
        {
            ols.DeinitializeOls();
            ols.Dispose();
            DRIVER_LOADED = true;
        }
    }
}
