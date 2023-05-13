using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU cooler settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateCoolerSettingsV1 : IInitializable
    {
        internal const int MaxNumberOfCoolersPerGPU = 3;

        internal StructureVersion _Version;
        internal readonly uint _CoolerSettingsCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfCoolersPerGPU)]
        internal readonly CoolerSetting[] _CoolerSettings;

        /// <summary>
        ///     Gets the list of cooler settings
        /// </summary>
        public CoolerSetting[] CoolerSettings
        {
            get => _CoolerSettings.Take((int) _CoolerSettingsCount).ToArray();
        }

        /// <summary>
        ///     Contains information regarding a cooler settings
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CoolerSetting
        {
            internal CoolerType _CoolerType;
            internal CoolerController _CoolerController;
            internal uint _DefaultMinimumLevel;
            internal uint _DefaultMaximumLevel;
            internal uint _CurrentMinimumLevel;
            internal uint _CurrentMaximumLevel;
            internal uint _CurrentLevel;
            internal CoolerPolicy _DefaultPolicy;
            internal CoolerPolicy _CurrentPolicy;
            internal CoolerTarget _Target;
            internal CoolerControlMode _ControlMode;
            internal uint _IsActive;

            /// <summary>
            ///     Gets the current cooler level in percentage.
            /// </summary>
            public uint CurrentLevel
            {
                get => _CurrentLevel;
            }

            /// <summary>
            ///     Gets the default minimum cooler level in percentage.
            /// </summary>
            public uint DefaultMinimumLevel
            {
                get => _DefaultMinimumLevel;
            }

            /// <summary>
            ///     Gets the default maximum cooler level in percentage.
            /// </summary>
            public uint DefaultMaximumLevel
            {
                get => _DefaultMaximumLevel;
            }

            /// <summary>
            ///     Gets the current minimum cooler level in percentage.
            /// </summary>
            public uint CurrentMinimumLevel
            {
                get => _CurrentMinimumLevel;
            }

            /// <summary>
            ///     Gets the current maximum cooler level in percentage.
            /// </summary>
            public uint CurrentMaximumLevel
            {
                get => _CurrentMaximumLevel;
            }

            /// <summary>
            ///     Gets the cooler type.
            /// </summary>
            public CoolerType CoolerType
            {
                get => _CoolerType;
            }

            /// <summary>
            ///     Gets the cooler controller.
            /// </summary>
            public CoolerController CoolerController
            {
                get => _CoolerController;
            }

            /// <summary>
            ///     Gets the cooler default policy.
            /// </summary>
            public CoolerPolicy DefaultPolicy
            {
                get => _DefaultPolicy;
            }

            /// <summary>
            ///     Gets the cooler current policy.
            /// </summary>
            public CoolerPolicy CurrentPolicy
            {
                get => _CurrentPolicy;
            }

            /// <summary>
            ///     Gets the cooler target.
            /// </summary>
            public CoolerTarget Target
            {
                get => _Target;
            }

            /// <summary>
            ///     Gets the cooler control mode.
            /// </summary>
            public CoolerControlMode ControlMode
            {
                get => _ControlMode;
            }
        }
    }
}