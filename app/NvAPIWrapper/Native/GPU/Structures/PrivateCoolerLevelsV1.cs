using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU cooler levels
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateCoolerLevelsV1 : IInitializable
    {
        internal const int MaxNumberOfCoolersPerGPU = PrivateCoolerSettingsV1.MaxNumberOfCoolersPerGPU;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfCoolersPerGPU)]
        internal CoolerLevel[] _CoolerLevels;

        /// <summary>
        ///     Gets the list of cooler levels.
        /// </summary>
        /// <param name="count">The number of cooler levels to return.</param>
        /// <returns>An array of <see cref="CoolerLevel" /> instances.</returns>
        public CoolerLevel[] GetCoolerLevels(int count)
        {
            return _CoolerLevels.Take(count).ToArray();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateCoolerLevelsV1" />.
        /// </summary>
        /// <param name="levels">The list of cooler levels.</param>
        public PrivateCoolerLevelsV1(CoolerLevel[] levels)
        {
            if (levels?.Length > MaxNumberOfCoolersPerGPU)
            {
                throw new ArgumentException($"Maximum of {MaxNumberOfCoolersPerGPU} cooler levels are configurable.",
                    nameof(levels));
            }

            if (levels == null || levels.Length == 0)
            {
                throw new ArgumentException("Array is null or empty.", nameof(levels));
            }

            this = typeof(PrivateCoolerLevelsV1).Instantiate<PrivateCoolerLevelsV1>();
            Array.Copy(levels, 0, _CoolerLevels, 0, levels.Length);
        }

        /// <summary>
        ///     Contains information regarding a cooler level
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct CoolerLevel
        {
            internal uint _CurrentLevel;
            internal CoolerPolicy _CurrentPolicy;

            /// <summary>
            ///     Creates a new instance of <see cref="CoolerLevel" />
            /// </summary>
            /// <param name="coolerPolicy">The cooler policy.</param>
            /// <param name="level">The cooler level in percentage.</param>
            public CoolerLevel(CoolerPolicy coolerPolicy, uint level)
            {
                _CurrentPolicy = coolerPolicy;
                _CurrentLevel = level;
            }

            /// <summary>
            ///     Creates a new instance of <see cref="CoolerLevel" />
            /// </summary>
            /// <param name="coolerPolicy">The cooler policy.</param>
            public CoolerLevel(CoolerPolicy coolerPolicy) : this(coolerPolicy, 0)
            {
                if (coolerPolicy == CoolerPolicy.Manual)
                {
                    throw new ArgumentException(
                        "Manual policy is not valid when no level value is provided.",
                        nameof(coolerPolicy)
                    );
                }
            }

            /// <summary>
            ///     Creates a new instance of <see cref="CoolerLevel" />
            /// </summary>
            /// <param name="level">The cooler level in percentage.</param>
            public CoolerLevel(uint level) : this(CoolerPolicy.Manual, level)
            {
            }

            /// <summary>
            ///     Gets the cooler level in percentage.
            /// </summary>
            public uint CurrentLevel
            {
                get => _CurrentLevel;
            }

            /// <summary>
            ///     Gets the cooler policy
            /// </summary>
            public CoolerPolicy CoolerPolicy
            {
                get => _CurrentPolicy;
            }
        }
    }
}