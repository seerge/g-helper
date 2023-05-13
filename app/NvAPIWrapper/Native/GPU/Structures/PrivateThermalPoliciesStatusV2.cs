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
    ///     Contains information regarding GPU thermal policies status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(2)]
    public struct PrivateThermalPoliciesStatusV2 : IInitializable
    {
        internal const int MaxNumberOfThermalPoliciesStatusEntries = 4;

        internal StructureVersion _Version;
        internal readonly uint _ThermalPoliciesStatusEntriesCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfThermalPoliciesStatusEntries)]
        internal readonly ThermalPoliciesStatusEntry[] _ThermalPoliciesStatusEntries;

        /// <summary>
        ///     Gets a list of thermal policy status entries
        /// </summary>
        public ThermalPoliciesStatusEntry[] ThermalPoliciesStatusEntries
        {
            get => _ThermalPoliciesStatusEntries.Take((int) _ThermalPoliciesStatusEntriesCount).ToArray();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateThermalPoliciesStatusV2" />
        /// </summary>
        /// <param name="policiesStatusEntries">The list of thermal policy status entries</param>
        public PrivateThermalPoliciesStatusV2(ThermalPoliciesStatusEntry[] policiesStatusEntries)
        {
            if (policiesStatusEntries?.Length > MaxNumberOfThermalPoliciesStatusEntries)
            {
                throw new ArgumentException(
                    $"Maximum of {MaxNumberOfThermalPoliciesStatusEntries} thermal policies entries are configurable.",
                    nameof(policiesStatusEntries)
                );
            }

            if (policiesStatusEntries == null || policiesStatusEntries.Length == 0)
            {
                throw new ArgumentException("Array is null or empty.", nameof(policiesStatusEntries));
            }

            this = typeof(PrivateThermalPoliciesStatusV2).Instantiate<PrivateThermalPoliciesStatusV2>();
            _ThermalPoliciesStatusEntriesCount = (uint) policiesStatusEntries.Length;
            Array.Copy(
                policiesStatusEntries,
                0,
                _ThermalPoliciesStatusEntries,
                0,
                policiesStatusEntries.Length
            );
        }

        /// <summary>
        ///     Contains information regarding a thermal policies status entry
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ThermalPoliciesStatusEntry
        {
            internal ThermalController _Controller;
            internal int _TargetTemperature;
            internal PerformanceStateId _PerformanceStateId;

            /// <summary>
            ///     Creates a new instance of <see cref="ThermalPoliciesStatusEntry" />
            /// </summary>
            /// <param name="controller">The thermal controller</param>
            /// <param name="targetTemperature">The target temperature.</param>
            public ThermalPoliciesStatusEntry(ThermalController controller, int targetTemperature) : this()
            {
                _Controller = controller;
                _TargetTemperature = targetTemperature * 256;
            }

            /// <summary>
            ///     Creates a new instance of <see cref="ThermalPoliciesStatusEntry" />
            /// </summary>
            /// <param name="performanceStateId">The performance state identification number</param>
            /// <param name="controller">The thermal controller</param>
            /// <param name="targetTemperature">The target temperature.</param>
            public ThermalPoliciesStatusEntry(
                PerformanceStateId performanceStateId,
                ThermalController controller,
                int targetTemperature) : this(controller, targetTemperature)
            {
                _PerformanceStateId = performanceStateId;
            }

            /// <summary>
            ///     Gets the thermal controller
            /// </summary>
            public ThermalController Controller
            {
                get => _Controller;
            }

            /// <summary>
            ///     Gets the performance state identification number
            /// </summary>
            public PerformanceStateId PerformanceStateId
            {
                get => _PerformanceStateId;
            }

            /// <summary>
            ///     Gets the target temperature
            /// </summary>
            public int TargetTemperature
            {
                get => _TargetTemperature >> 8;
            }
        }
    }
}