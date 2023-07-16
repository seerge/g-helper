//
// This is a optimised/simplified version of Ryzen System Management Unit from https://github.com/JamesCJ60/Universal-x86-Tuning-Utility
// I do not take credit for the full functionality of the code (c)
//


using System.Management;

namespace Ryzen
{
    internal class RyzenControl
    {

        public const int MinCPUUV = -30;
        public const int MaxCPUUV = 0;

        public const int MinIGPUUV = -20;
        public const int MaxIGPUUV = 0;

        public const int MinTemp = 75;
        public const int MaxTemp = 98;

        public static string[] FAM = { "RAVEN", "PICASSO", "DALI", "RENOIR/LUCIENNE", "MATISSE", "VANGOGH", "VERMEER", "CEZANNE/BARCELO", "REMBRANDT", "PHOENIX", "RAPHAEL/DRAGON RANGE" };
        public static int FAMID { get; protected set; }

        public static string CPUModel = "";
        public static string CPUName = "";

        //Zen1/+ - -1
        //RAVEN - 0
        //PICASSO - 1
        //DALI - 2
        //RENOIR/LUCIENNE - 3
        //MATISSE - 4
        //VANGOGH - 5
        //VERMEER - 6
        //CEZANNE/BARCELO - 7
        //REMBRANDT - 8
        //PHEONIX - 9
        //RAPHAEL/DRAGON RANGE - 10
        //MENDOCINO - 11

        public static void Init()
        {
            //Get CPU name

            try
            {
                ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (ManagementObject obj in myProcessorObject.Get())
                {
                    CPUName = obj["Name"].ToString();
                    CPUModel = obj["Caption"].ToString();
                }
            } catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

            FAMID = 99999;

            if (CPUModel.Contains("Model " + Convert.ToString(1)) || CPUModel.Contains("Model " + Convert.ToString(8)))
            {
                FAMID = -1; //Zen1/+ DT
            }

            if (CPUModel.Contains("Model " + Convert.ToString(17)))
            {
                FAMID = 0; //RAVEN
            }

            if (CPUModel.Contains("Model " + Convert.ToString(24)))
            {
                FAMID = 1; //PICASSO
            }

            if (CPUModel.Contains("Model " + Convert.ToString(32)))
            {
                FAMID = 2; //DALI
            }

            if (CPUModel.Contains("Model " + Convert.ToString(33)))
            {
                FAMID = 6; //VERMEER
            }

            if (CPUModel.Contains("Model " + Convert.ToString(96)) || CPUModel.Contains("Model " + Convert.ToString(104)))
            {
                FAMID = 3; //RENOIR/LUCIENNE
            }

            if (CPUModel.Contains("Model " + Convert.ToString(144)))
            {
                FAMID = 5; //VANGOGH
            }

            if (CPUModel.Contains("Model " + Convert.ToString(80)))
            {
                FAMID = 7; //CEZANNE/BARCELO
            }

            if (CPUModel.Contains("Model " + Convert.ToString(64)) || CPUModel.Contains("Model " + Convert.ToString(68)))
            {
                FAMID = 8; //REMBRANDT
            }

            if (CPUModel.Contains("Model " + Convert.ToString(116)))
            {
                FAMID = 9; //PHEONIX 
            }

            if (CPUModel.Contains("Model " + Convert.ToString(97)))
            {
                FAMID = 10; //RAPHAEL/DRAGON RANGE
            }

            if (CPUModel.Contains("Model " + Convert.ToString(160)))
            {
                FAMID = 11; //MENDOCINO 
            }

            Logger.WriteLine($"CPU: {FAMID} - {CPUName} - {CPUModel}");

            SetAddresses();
        }

        public static bool IsAMD()
        {
            if (CPUName.Length == 0) Init();
            return CPUName.Contains("AMD") || CPUName.Contains("Ryzen") || CPUName.Contains("Athlon") || CPUName.Contains("Radeon") || CPUName.Contains("AMD Custom APU 0405");
        }

        public static bool IsSupportedUV()
        {
            if (CPUName.Length == 0) Init();
            return CPUName.Contains("Ryzen 9") || CPUName.Contains("4900H") || CPUName.Contains("4800H") || CPUName.Contains("4600H");
        }

        public static bool IsSupportedUViGPU()
        {
            if (CPUName.Length == 0) Init();
            return CPUName.Contains("6900H") || CPUName.Contains("7945H") || CPUName.Contains("7845H");
        }

        public static void SetAddresses()
        {

            Smu.SMU_PCI_ADDR = 0x00000000;
            Smu.SMU_OFFSET_ADDR = 0xB8;
            Smu.SMU_OFFSET_DATA = 0xBC;

            if (FAMID == -1)
            {
                Smu.MP1_ADDR_MSG = 0X3B10528;
                Smu.MP1_ADDR_RSP = 0X3B10564;
                Smu.MP1_ADDR_ARG = 0X3B10598;

                Smu.PSMU_ADDR_MSG = 0x3B1051C;
                Smu.PSMU_ADDR_RSP = 0X3B10568;
                Smu.PSMU_ADDR_ARG = 0X3B10590;
            }


            if (FAMID == 0 || FAMID == 1 || FAMID == 2 || FAMID == 3 || FAMID == 7)
            {
                Smu.MP1_ADDR_MSG = 0x3B10528;
                Smu.MP1_ADDR_RSP = 0x3B10564;
                Smu.MP1_ADDR_ARG = 0x3B10998;

                Smu.PSMU_ADDR_MSG = 0x3B10A20;
                Smu.PSMU_ADDR_RSP = 0x3B10A80;
                Smu.PSMU_ADDR_ARG = 0x3B10A88;
            }
            else if (FAMID == 5 || FAMID == 8 || FAMID == 9 || FAMID == 11)
            {
                Smu.MP1_ADDR_MSG = 0x3B10528;
                Smu.MP1_ADDR_RSP = 0x3B10578;
                Smu.MP1_ADDR_ARG = 0x3B10998;

                Smu.PSMU_ADDR_MSG = 0x3B10a20;
                Smu.PSMU_ADDR_RSP = 0x3B10a80;
                Smu.PSMU_ADDR_ARG = 0x3B10a88;
            }
            else if (FAMID == 4 || FAMID == 6)
            {
                Smu.MP1_ADDR_MSG = 0x3B10530;
                Smu.MP1_ADDR_RSP = 0x3B1057C;
                Smu.MP1_ADDR_ARG = 0x3B109C4;

                Smu.PSMU_ADDR_MSG = 0x3B10524;
                Smu.PSMU_ADDR_RSP = 0x3B10570;
                Smu.PSMU_ADDR_ARG = 0x3B10A40;
            }
            else if (FAMID == 10)
            {
                Smu.MP1_ADDR_MSG = 0x3010508;
                Smu.MP1_ADDR_RSP = 0x3010988;
                Smu.MP1_ADDR_ARG = 0x3010984;

                Smu.PSMU_ADDR_MSG = 0x3B10524;
                Smu.PSMU_ADDR_RSP = 0x3B10570;
                Smu.PSMU_ADDR_ARG = 0x3B10A40;
            }
            else
            {
                Smu.MP1_ADDR_MSG = 0;
                Smu.MP1_ADDR_RSP = 0;
                Smu.MP1_ADDR_ARG = 0;

                Smu.PSMU_ADDR_MSG = 0;
                Smu.PSMU_ADDR_RSP = 0;
                Smu.PSMU_ADDR_ARG = 0;
            }


        }
    }
}
