using System.Runtime.InteropServices;

namespace GHelper.Mode
{
    internal class PowerNative
    {
        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerWriteDCValueIndex(IntPtr RootPowerKey,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
            int AcValueIndex);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerWriteACValueIndex(IntPtr RootPowerKey,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
            int AcValueIndex);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerReadACValueIndex(IntPtr RootPowerKey,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
            out IntPtr AcValueIndex
            );

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerReadDCValueIndex(IntPtr RootPowerKey,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SubGroupOfPowerSettingsGuid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid PowerSettingGuid,
            out IntPtr AcValueIndex
            );


        [DllImport("powrprof.dll")]
        static extern uint PowerReadACValue(
            IntPtr RootPowerKey,
            Guid SchemeGuid,
            Guid SubGroupOfPowerSettingGuid,
            Guid PowerSettingGuid,
            ref int Type,
            ref IntPtr Buffer,
            ref uint BufferSize
            );


        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerSetActiveScheme(IntPtr RootPowerKey,
            [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        static extern UInt32 PowerGetActiveScheme(IntPtr UserPowerKey, out IntPtr ActivePolicyGuid);

        static readonly Guid GUID_CPU = new Guid("54533251-82be-4824-96c1-47b60b740d00");
        static readonly Guid GUID_BOOST = new Guid("be337238-0d82-4146-a960-4f3749d470c7");

        private static Guid GUID_SLEEP_SUBGROUP = new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");
        private static Guid GUID_HIBERNATEIDLE = new Guid("9d7815a6-7ee4-497e-8888-515a05f02364");

        private static Guid GUID_SYSTEM_BUTTON_SUBGROUP = new Guid("4f971e89-eebd-4455-a8de-9e59040e7347");
        private static Guid GUID_LIDACTION = new Guid("5CA83367-6E45-459F-A27B-476B1D01C936");

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetActualOverlayScheme")]
        public static extern uint PowerGetActualOverlayScheme(out Guid ActualOverlayGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        public static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);

        [DllImportAttribute("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        public static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

        public static Dictionary<string, string> powerModes = new Dictionary<string, string>
            {
                { "961cc777-2547-4f9d-8174-7d86181b8a7a", "Best Power Efficiency" },
                { "00000000-0000-0000-0000-000000000000", "Balanced" },
                { "ded574b5-45a0-4f42-8737-46345c09c238", "Best Performance" },
            };
        static Guid GetActiveScheme()
        {
            IntPtr pActiveSchemeGuid;
            var hr = PowerGetActiveScheme(IntPtr.Zero, out pActiveSchemeGuid);
            Guid activeSchemeGuid = (Guid)Marshal.PtrToStructure(pActiveSchemeGuid, typeof(Guid));
            return activeSchemeGuid;
        }

        public static int GetCPUBoost()
        {
            IntPtr AcValueIndex;
            Guid activeSchemeGuid = GetActiveScheme();

            UInt32 value = PowerReadACValueIndex(IntPtr.Zero,
                 activeSchemeGuid,
                 GUID_CPU,
                 GUID_BOOST, out AcValueIndex);

            return AcValueIndex.ToInt32();

        }

        public static void SetCPUBoost(int boost = 0)
        {
            Guid activeSchemeGuid = GetActiveScheme();

            if (boost == GetCPUBoost()) return;

            var hrAC = PowerWriteACValueIndex(
                 IntPtr.Zero,
                 activeSchemeGuid,
                 GUID_CPU,
                 GUID_BOOST,
                 boost);

            PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);

            var hrDC = PowerWriteDCValueIndex(
                 IntPtr.Zero,
                 activeSchemeGuid,
                 GUID_CPU,
                 GUID_BOOST,
                 boost);

            PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);

            Logger.WriteLine("Boost " + boost);
        }

        public static void SetPowerScheme(string scheme)
        {
            List<string> overlays = new() {
                "00000000-0000-0000-0000-000000000000",
                "ded574b5-45a0-4f42-8737-46345c09c238",
                "961cc777-2547-4f9d-8174-7d86181b8a7a",
                "3af9B8d9-7c97-431d-ad78-34a8bfea439f"
            };

            Guid guidScheme = new Guid(scheme);

            if (overlays.Contains(scheme))
            {

                Guid activeSchemeGuid = GetActiveScheme();
                Guid balanced = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e");

                if (activeSchemeGuid != balanced && !AppConfig.Is("skip_power_plan"))
                {
                    PowerSetActiveScheme(IntPtr.Zero, balanced);
                    Logger.WriteLine("Balanced Plan: " + balanced);
                }

                uint status = PowerGetEffectiveOverlayScheme(out Guid activeScheme);
                if (status != 0 || activeScheme != guidScheme)
                {
                    PowerSetActiveOverlayScheme(guidScheme);
                    Logger.WriteLine("Power Mode: " + scheme);
                }
            }
            else
            {
                PowerSetActiveScheme(IntPtr.Zero, guidScheme);
                Logger.WriteLine("Power Plan: " + scheme);
            }


        }

        public static void SetPowerScheme(int mode)
        {
            switch (mode)
            {
                case 0: // balanced
                    SetPowerScheme("00000000-0000-0000-0000-000000000000");
                    break;
                case 1: // turbo
                    SetPowerScheme("ded574b5-45a0-4f42-8737-46345c09c238");
                    break;
                case 2: //silent
                    SetPowerScheme("961cc777-2547-4f9d-8174-7d86181b8a7a");
                    break;
            }
        }

        public static int GetLidAction(bool ac)
        {
            Guid activeSchemeGuid = GetActiveScheme();

            IntPtr activeIndex;
            if (ac)
                PowerReadACValueIndex(IntPtr.Zero,
                     activeSchemeGuid,
                     GUID_SYSTEM_BUTTON_SUBGROUP,
                     GUID_LIDACTION, out activeIndex);

            else
                PowerReadDCValueIndex(IntPtr.Zero,
                    activeSchemeGuid,
                    GUID_SYSTEM_BUTTON_SUBGROUP,
                    GUID_LIDACTION, out activeIndex);


            return activeIndex.ToInt32();
        }


        public static void SetLidAction(int action, bool acOnly = false)
        {
            /**
             * 1: Do nothing
             * 2: Seelp
             * 3: Hibernate
             * 4: Shutdown
             */

            Guid activeSchemeGuid = GetActiveScheme();

            var hrAC = PowerWriteACValueIndex(
                IntPtr.Zero,
                activeSchemeGuid,
                GUID_SYSTEM_BUTTON_SUBGROUP,
                GUID_LIDACTION,
                action);

            PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);

            if (!acOnly)
            {
                var hrDC = PowerWriteDCValueIndex(
                  IntPtr.Zero,
                  activeSchemeGuid,
                  GUID_SYSTEM_BUTTON_SUBGROUP,
                  GUID_LIDACTION,
                  action);

                PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);
            }

            Logger.WriteLine("Changed Lid Action to " + action);
        }

        public static int GetHibernateAfter()
        {
            Guid activeSchemeGuid = GetActiveScheme();
            IntPtr seconds;
            PowerReadDCValueIndex(IntPtr.Zero,
                    activeSchemeGuid,
                    GUID_SLEEP_SUBGROUP,
                    GUID_HIBERNATEIDLE, out seconds);

            Logger.WriteLine("Hibernate after " + seconds);
            return (seconds.ToInt32() / 60);
        }


        public static void SetHibernateAfter(int minutes)
        {
            int seconds = minutes * 60;

            Guid activeSchemeGuid = GetActiveScheme();
            var hrAC = PowerWriteDCValueIndex(
                IntPtr.Zero,
                activeSchemeGuid,
                GUID_SLEEP_SUBGROUP,
                GUID_HIBERNATEIDLE,
                seconds);

            PowerSetActiveScheme(IntPtr.Zero, activeSchemeGuid);

            Logger.WriteLine("Setting Hibernate after " + seconds + ": " + (hrAC == 0 ? "OK" : hrAC));
        }


    }
}
