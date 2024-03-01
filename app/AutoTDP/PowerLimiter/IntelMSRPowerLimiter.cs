using HidSharp.Reports.Units;
using Ryzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHelper.AutoTDP.PowerLimiter
{
    internal class IntelMSRPowerLimiter : IPowerLimiter
    {
        private Ols ols;

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
            ReadPowerUnit();
        }

        public void ReadPowerUnit()
        {
            uint eax = 0;
            uint edx = 0;

            ols.Rdmsr(0x606, ref eax, ref edx);


            uint pwr = eax & 0x03;

            PowerUnit = 1 / Math.Pow(2, pwr);
        }

        public void SetCPUPowerLimit(int watts)
        {
            uint eax = 0;
            uint edx = 0;


            ols.Rdmsr(0x610, ref eax, ref edx);

            uint watsRapl = (uint)(watts / PowerUnit);

            //Set limits for both PL1 and PL2
            uint eaxFilterd = eax & ~PL1_MASK;
            uint edxFilterd = edx & ~PL2_MASK;

            eaxFilterd |= watsRapl;
            edxFilterd |= watsRapl;

            //Enable clamping
            eaxFilterd |= 0x8000;
            edxFilterd |= 0x8000;

            ols.Wrmsr(0x610, eaxFilterd, edxFilterd);
        }


        public int GetCPUPowerLimit()
        {
            uint eax = 0;
            uint edx = 0;

            ols.Rdmsr(0x610, ref eax, ref edx);

            if (DefaultEax == 0)
            {
                //Store default settings to reset them on exit
                DefaultEax = eax;
                DefaultEdx = edx;
            }

            uint pl1 = eax & PL1_MASK;
            uint pl2 = edx & PL2_MASK;

            return (int)(pl1 * PowerUnit);
        }


        public void ResetPowerLimits()
        {
            if (DefaultEax == 0)
            {
                return;
            }
            ols.Wrmsr(0x610, DefaultEax, DefaultEdx);
        }

        public void Dispose()
        {
            ols.DeinitializeOls();
            ols.Dispose();
        }
    }
}
