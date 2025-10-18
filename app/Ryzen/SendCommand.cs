//
// This is a optimised/simplified version of Ryzen System Management Unit from https://github.com/JamesCJ60/Universal-x86-Tuning-Utility
// I do not take credit for the full functionality of the code (c)
//


namespace Ryzen
{
    internal class SendCommand
    {

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
        //HAWKPOINT - 12
        //STRIXPOINT - 13
        //STRIXHALO - 14
        //FIRERANGE - 15

        public static Smu RyzenAccess = new Smu(false);
        public static int FAMID = RyzenControl.FAMID;


        //STAMP Limit
        public static Smu.Status? set_stapm_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;
            Smu.Status? result = null;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    result = RyzenAccess.SendMp1(0x1a, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                    result = RyzenAccess.SendMp1(0x14, ref Args);
                    result = RyzenAccess.SendPsmu(0x31, ref Args);
                    break;
                default:
                    break;
            }

            RyzenAccess.Deinitialize();
            return result;
            
        }

        //Fast Limit
        public static Smu.Status? set_fast_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;
            Smu.Status? result = null;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    result = RyzenAccess.SendMp1(0x1b, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                    result = RyzenAccess.SendMp1(0x15, ref Args);
                    result = RyzenAccess.SendPsmu(0x32, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
            return result;
        }

        //Slow Limit
        public static Smu.Status? set_slow_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;
            Smu.Status? result = null;
            
            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    result = RyzenAccess.SendMp1(0x1c, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                    result = RyzenAccess.SendMp1(0x16, ref Args);
                    result = RyzenAccess.SendPsmu(0x33, ref Args);
                    result = RyzenAccess.SendPsmu(0x34, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
            return result;
        }


        //TCTL Temp Limit
        public static Smu.Status? set_tctl_temp(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            Smu.Status? result = null;

            switch (FAMID)
            {
                case -1:
                    result = RyzenAccess.SendPsmu(0x68, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    result = RyzenAccess.SendMp1(0x1f, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                case 14:
                    result = RyzenAccess.SendMp1(0x19, ref Args);
                    break;
                case 4:
                case 6:
                    result = RyzenAccess.SendMp1(0x23, ref Args);
                    result = RyzenAccess.SendPsmu(0x56, ref Args);
                    break;
                case 10:
                case 15:
                    result = RyzenAccess.SendMp1(0x3f, ref Args);
                    result = RyzenAccess.SendPsmu(0x59, ref Args);
                    break;
                default:
                    break;
            }

            RyzenAccess.Deinitialize();
            return result;
        }

        //Set All Core Curve Optimiser
        public static Smu.Status? set_coall(int value)
        {

            uint uvalue = Convert.ToUInt32(0x100000 - (uint)(-1 * value));

            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = uvalue;

            Smu.Status? result = null;

            switch (FAMID)
            {
                case 3:
                case 7:
                    result = RyzenAccess.SendMp1(0x55, ref Args);
                    result = RyzenAccess.SendPsmu(0xB1, ref Args);
                    break;
                case 4:
                case 6:
                    result = RyzenAccess.SendMp1(0x36, ref Args);
                    result = RyzenAccess.SendPsmu(0xB, ref Args);
                    break;
                case 5:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                    result = RyzenAccess.SendPsmu(0x5D, ref Args);
                    break;
                case 14:
                    result = RyzenAccess.SendMp1(0x4C, ref Args);
                    result = RyzenAccess.SendPsmu(0x5D, ref Args);
                    break;
                case 10:
                case 15:
                    result = RyzenAccess.SendPsmu(0x7, ref Args);
                    break;
                default:
                    break;
            }

            RyzenAccess.Deinitialize();
            return result;

        }


        //Set iGPU Curve Optimiser
        public static Smu.Status? set_cogfx(int value)
        {

            uint uvalue = Convert.ToUInt32(0x100000 - (uint)(-1 * value));

            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = uvalue;

            Smu.Status? result = null;

            switch (FAMID)
            {
                case 3:
                case 7:
                    result = RyzenAccess.SendMp1(0x64, ref Args);
                    result = RyzenAccess.SendPsmu(0x57, ref Args);
                    break;
                case 5:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                case 14:
                    result = RyzenAccess.SendPsmu(0xb7, ref Args);
                    break;
                default:
                    break;
            }

            RyzenAccess.Deinitialize();
            return result;
        }


    }
}
