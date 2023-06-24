//
// This is a optimised/simplified version of Ryzen System Management Unit from https://github.com/JamesCJ60/Universal-x86-Tuning-Utility
// I do not take credit for the full functionality of the code (c)
//

[assembly: CLSCompliant(false)]


namespace Ryzen
{
    class Smu
    {

        public enum Status : int
        {
            BAD = 0x0,
            OK = 0x1,
            FAILED = 0xFF,
            UNKNOWN_CMD = 0xFE,
            CMD_REJECTED_PREREQ = 0xFD,
            CMD_REJECTED_BUSY = 0xFC
        }

        private static readonly Dictionary<Status, string> status = new Dictionary<Status, string>()
        {
            { Status.BAD, "BAD" },
            { Status.OK, "OK" },
            { Status.FAILED, "Failed" },
            { Status.UNKNOWN_CMD, "Unknown Command" },
            { Status.CMD_REJECTED_PREREQ, "CMD Rejected Prereq" },
            { Status.CMD_REJECTED_BUSY, "CMD Rejected Busy" }
        };



        Ols RyzenNbAccesss;


        public Smu(bool EnableDebug)
        {
            ShowDebug = EnableDebug;
            RyzenNbAccesss = new Ols();

            // Check WinRing0 status
            switch (RyzenNbAccesss.GetDllStatus())
            {
                case (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR:
                    if (ShowDebug)
                    {
                        //MessageBox.Show("Ols Dll is OK.", "Ols.OlsDllStatus:");
                    }
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED:
                    //MessageBox.Show("WinRing OLS_DRIVER_NOT_LOADED", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing OLS_DRIVER_NOT_LOADED");

                case (uint)Ols.OlsDllStatus.OLS_DLL_UNSUPPORTED_PLATFORM:
                    //MessageBox.Show("WinRing OLS_UNSUPPORTED_PLATFORM", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing OLS_UNSUPPORTED_PLATFORM");

                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_FOUND:
                    //MessageBox.Show("WinRing OLS_DLL_DRIVER_NOT_FOUND", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing OLS_DLL_DRIVER_NOT_FOUND");

                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_UNLOADED:
                    //MessageBox.Show("WinRing OLS_DLL_DRIVER_UNLOADED", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing OLS_DLL_DRIVER_UNLOADED");

                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK:
                    //MessageBox.Show("WinRing DRIVER_NOT_LOADED_ON_NETWORK", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing DRIVER_NOT_LOADED_ON_NETWORK");

                case (uint)Ols.OlsDllStatus.OLS_DLL_UNKNOWN_ERROR:
                    //MessageBox.Show("WinRing OLS_DLL_UNKNOWN_ERROR", "Ols.OlsDllStatus:");
                    throw new ApplicationException("WinRing OLS_DLL_UNKNOWN_ERROR");
            }

        }

        public void Initialize()
        {
            amdSmuMutex = new Mutex();
            RyzenNbAccesss.InitializeOls();

            // Check WinRing0 status
            switch (RyzenNbAccesss.GetStatus())
            {
                case (uint)Ols.Status.NO_ERROR:
                    if (ShowDebug)
                    {
                        //MessageBox.Show("Ols is OK.", "Ols.Status:");
                        ShowDebug = false;
                    }
                    break;
                case (uint)Ols.Status.DLL_NOT_FOUND:
                    //MessageBox.Show("WinRing Status: DLL_NOT_FOUND", "Ols.Status:");
                    throw new ApplicationException("WinRing DLL_NOT_FOUND");
                    break;
                case (uint)Ols.Status.DLL_INCORRECT_VERSION:
                    //MessageBox.Show("WinRing Status: DLL_INCORRECT_VERSION", "Ols.Status:");
                    throw new ApplicationException("WinRing DLL_INCORRECT_VERSION");
                    break;
                case (uint)Ols.Status.DLL_INITIALIZE_ERROR:
                    //MessageBox.Show("WinRing Status: DLL_INITIALIZE_ERROR", "Ols.Status:");
                    throw new ApplicationException("WinRing DLL_INITIALIZE_ERROR");
                    break;
                default:
                    break;
            }
        }


        public void Deinitialize()
        {
            RyzenNbAccesss.DeinitializeOls();
        }

        public static uint SMU_PCI_ADDR { get; set; }
        public static uint SMU_OFFSET_ADDR { get; set; }
        public static uint SMU_OFFSET_DATA { get; set; }

        public static uint MP1_ADDR_MSG { get; set; }
        public static uint MP1_ADDR_RSP { get; set; }
        public static uint MP1_ADDR_ARG { get; set; }

        public static uint PSMU_ADDR_MSG { get; set; }
        public static uint PSMU_ADDR_RSP { get; set; }
        public static uint PSMU_ADDR_ARG { get; set; }
        public static uint[] args { get; set; }

        public bool ShowDebug { get; set; }

        private static Mutex amdSmuMutex;
        private const ushort SMU_TIMEOUT = 8192;

        public Status SendMp1(uint message, ref uint[] arguments)
        {
            var result = SendMsg(MP1_ADDR_MSG, MP1_ADDR_RSP, MP1_ADDR_ARG, message, ref arguments);
            //Logger.WriteLine($"RyzenMP1:{message} {arguments[0]} {result}");
            return result;
        }

        public Status SendPsmu(uint message, ref uint[] arguments)
        {
            var result =  SendMsg(PSMU_ADDR_MSG, PSMU_ADDR_RSP, PSMU_ADDR_ARG, message, ref arguments);
            //Logger.WriteLine($"RyzenPSMU:{message} {arguments[0]} {result}");
            return result;
        }


        public Status SendMsg(uint SMU_ADDR_MSG, uint SMU_ADDR_RSP, uint SMU_ADDR_ARG, uint msg, ref uint[] args)
        {
            ushort timeout = SMU_TIMEOUT;
            uint[] cmdArgs = new uint[6];
            int argsLength = args.Length;
            uint status = 0;

            if (argsLength > cmdArgs.Length)
                argsLength = cmdArgs.Length;

            for (int i = 0; i < argsLength; ++i)
                cmdArgs[i] = args[i];

            if (amdSmuMutex.WaitOne(5000))
            {
                // Clear response register
                bool temp;
                do
                    temp = SmuWriteReg(SMU_ADDR_RSP, 0);
                while (!temp && --timeout > 0);

                if (timeout == 0)
                {
                    amdSmuMutex.ReleaseMutex();
                    SmuReadReg(SMU_ADDR_RSP, ref status);
                    return (Status)status;
                }

                // Write data
                for (int i = 0; i < cmdArgs.Length; ++i)
                    SmuWriteReg(SMU_ADDR_ARG + (uint)(i * 4), cmdArgs[i]);

                // Send message
                SmuWriteReg(SMU_ADDR_MSG, msg);

                // Wait done
                if (!SmuWaitDone(SMU_ADDR_RSP))
                {
                    amdSmuMutex.ReleaseMutex();
                    SmuReadReg(SMU_ADDR_RSP, ref status);
                    return (Status)status;
                }

                // Read back args
                for (int i = 0; i < args.Length; ++i)
                    SmuReadReg(SMU_ADDR_ARG + (uint)(i * 4), ref args[i]);
            }

            amdSmuMutex.ReleaseMutex();
            SmuReadReg(SMU_ADDR_RSP, ref status);

            return (Status)status;
        }


        public bool SmuWaitDone(uint SMU_ADDR_RSP)
        {
            bool res;
            ushort timeout = SMU_TIMEOUT;
            uint data = 0;

            do
                res = SmuReadReg(SMU_ADDR_RSP, ref data);
            while ((!res || data != 1) && --timeout > 0);

            if (timeout == 0 || data != 1) res = false;

            return res;
        }


        private bool SmuWriteReg(uint addr, uint data)
        {
            if (RyzenNbAccesss.WritePciConfigDwordEx(SMU_PCI_ADDR, SMU_OFFSET_ADDR, addr) == 1)
            {
                return RyzenNbAccesss.WritePciConfigDwordEx(SMU_PCI_ADDR, SMU_OFFSET_DATA, data) == 1;
            }
            return false;
        }

        private bool SmuReadReg(uint addr, ref uint data)
        {
            if (RyzenNbAccesss.WritePciConfigDwordEx(SMU_PCI_ADDR, SMU_OFFSET_ADDR, addr) == 1)
            {
                return RyzenNbAccesss.ReadPciConfigDwordEx(SMU_PCI_ADDR, SMU_OFFSET_DATA, ref data) == 1;
            }
            return false;
        }



    }
}
