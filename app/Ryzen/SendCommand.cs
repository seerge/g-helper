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

        public static Smu RyzenAccess = new Smu(false);
        public static int FAMID = Undervolter.FAMID;


        //STAMP Limit
        public static void set_stapm_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1a, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x14, ref Args);
                    RyzenAccess.SendPsmu(0x31, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //STAMP2 Limit
        public static void set_stapm2_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x31, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Fast Limit
        public static void set_fast_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;
            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1b, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x15, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Slow Limit
        public static void set_slow_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1c, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x16, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Slow time
        public static void set_slow_time(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1d, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x17, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //STAMP Time
        public static void set_stapm_time(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1e, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x18, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //TCTL Temp Limit
        public static void set_tctl_temp(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x68, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x1f, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x19, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x23, ref Args);
                    RyzenAccess.SendPsmu(0x56, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x59, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //cHTC Temp Limit
        public static void set_cHTC_temp(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x56, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x37, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Skin Temp limit
        public static void set_apu_skin_temp_limit(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 5:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x33, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x38, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM Current
        public static void set_vrm_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x20, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x1a, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM SoC Current
        public static void set_vrmsoc_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x21, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x1b, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM GFX Current
        public static void set_vrmgfx_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 5:
                    RyzenAccess.SendMp1(0x1c, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM CVIP Current
        public static void set_vrmcvip_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 5:
                    RyzenAccess.SendMp1(0x1d, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM Max Current
        public static void set_vrmmax_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x22, ref Args);
                    break;
                case 5:
                    RyzenAccess.SendMp1(0x1e, ref Args);
                    break;
                case 3:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x1c, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM GFX Max Current
        public static void set_vrmgfxmax_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 5:
                    RyzenAccess.SendMp1(0x1f, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VRM SoC Max Current
        public static void set_vrmsocmax_current(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x23, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x1d, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //GFX Clock Max
        public static void set_max_gfxclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x46, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //GFX Clock Min
        public static void set_min_gfxclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x47, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //SoC Clock Max
        public static void set_max_socclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x48, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //SoC Clock Min
        public static void set_min_socclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x49, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //FCLK Clock Max
        public static void set_max_fclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4a, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //FCLK Clock Min
        public static void set_min_fclk_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4b, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VCN Clock Max
        public static void set_max_vcn_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4c, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //VCN Clock Min
        public static void set_min_vcn_freq(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4d, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //LCLK Clock Max
        public static void set_max_lclk(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4e, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //LCLK Clock Min
        public static void set_min_lclk(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x4f, ref Args);
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Prochot Ramp
        public static void set_prochot_deassertion_ramp(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x26, ref Args);
                    break;
                case 5:
                    RyzenAccess.SendMp1(0x22, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x20, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x1f, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //GFX Clock
        public static void set_gfx_clk(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 3:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x89, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //dGPU Skin Temp
        public static void set_dGPU_skin(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x37, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x32, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Power Saving
        public static void set_power_saving(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x19, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x12, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Max Performance
        public static void set_max_performance(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendMp1(0x18, ref Args);
                    break;
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x11, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set All Core OC
        public static void set_oc_clk(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x6c, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x7d, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x31, ref Args);
                    RyzenAccess.SendPsmu(0x19, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x26, ref Args);
                    RyzenAccess.SendPsmu(0x5c, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x19, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x5F, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set Per Core OC
        public static void set_per_core_oc_clk(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x6d, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x7E, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x32, ref Args);
                    RyzenAccess.SendPsmu(0x1a, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x27, ref Args);
                    RyzenAccess.SendPsmu(0x5d, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x1a, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x60, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set VID
        public static void set_oc_volt(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x6e, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x7f, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x33, ref Args);
                    RyzenAccess.SendPsmu(0x1b, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x28, ref Args);
                    RyzenAccess.SendPsmu(0x61, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x61, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set All Core Curve Optimiser
        public static void set_coall(int value)
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
                    RyzenAccess.SendMp1(0x55, ref Args);
                    result = RyzenAccess.SendPsmu(0xB1, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x36, ref Args);
                    result = RyzenAccess.SendPsmu(0xB, ref Args);
                    break;
                case 5:
                case 8:
                case 9:
                case 11:
                    result = RyzenAccess.SendPsmu(0x5D, ref Args);
                    break;
                case 10:
                    result = RyzenAccess.SendPsmu(0x7, ref Args);
                    break;
                default:
                    break;
            }

            Logger.WriteLine($"UV: {value} {result}");

            RyzenAccess.Deinitialize();
        }

        //Set Per Core Curve Optimiser
        public static void set_coper(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x54, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x35, ref Args);
                    break;
                case 5:
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendMp1(0x4b, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x6, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set iGPU Curve Optimiser
        public static void set_cogfx(int value)
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
                    RyzenAccess.SendMp1(0x64, ref Args);
                    result = RyzenAccess.SendPsmu(0x57, ref Args);
                    break;
                case 5:
                case 8:
                case 9:
                case 11:
                    result = RyzenAccess.SendPsmu(0xb7, ref Args);
                    break;
                default:
                    break;
            }

            Logger.WriteLine($"iGPU UV: {value} {result}");

            RyzenAccess.Deinitialize();
        }

        //Disable OC
        public static void set_disable_oc()
        {
            uint value = 0x0;
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendMp1(0x24, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x6A, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x30, ref Args);
                    RyzenAccess.SendPsmu(0x1d, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x25, ref Args);
                    RyzenAccess.SendPsmu(0x5b, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x18, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x5E, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Enable OC
        public static void set_enable_oc()
        {
            uint value = 0x0;
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendMp1(0x23, ref Args);
                    break;
                case 0:
                case 1:
                case 2:
                    RyzenAccess.SendPsmu(0x69, ref Args);
                    break;
                case 3:
                case 7:
                    RyzenAccess.SendMp1(0x2f, ref Args);
                    RyzenAccess.SendPsmu(0x1d, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendMp1(0x24, ref Args);
                    RyzenAccess.SendPsmu(0x5a, ref Args);
                    break;
                case 8:
                case 9:
                case 11:
                    RyzenAccess.SendPsmu(0x17, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x5D, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set PBO Scaler
        public static void set_scaler(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x6a, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendPsmu(0x58, ref Args);
                    RyzenAccess.SendMp1(0x2F, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x5b, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }


        //Set PPT
        public static void set_ppt(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x64, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendPsmu(0x53, ref Args);
                    RyzenAccess.SendMp1(0x3D, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x56, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }


        //Set TDC
        public static void set_tdc(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x65, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendPsmu(0x54, ref Args);
                    RyzenAccess.SendMp1(0x3B, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x57, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }

        //Set EDC
        public static void set_edc(uint value)
        {
            RyzenAccess.Initialize();
            uint[] Args = new uint[6];
            Args[0] = value;

            switch (FAMID)
            {
                case -1:
                    RyzenAccess.SendPsmu(0x66, ref Args);
                    break;
                case 4:
                case 6:
                    RyzenAccess.SendPsmu(0x55, ref Args);
                    RyzenAccess.SendMp1(0x3c, ref Args);
                    break;
                case 10:
                    RyzenAccess.SendPsmu(0x58, ref Args);
                    break;
                default:
                    break;
            }
            RyzenAccess.Deinitialize();
        }
    }
}
