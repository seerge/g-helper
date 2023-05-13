using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains the information required for calculating timing for a particular display
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct TimingInput : IInitializable
    {
        [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 12)]
        internal struct TimingFlag
        {
            [FieldOffset(0)] internal ushort _Flags;
            [FieldOffset(4)] internal byte _TVFormatCEAIdPSFormatId;
            [FieldOffset(8)] internal byte _Scaling;

            public bool IsInterlaced
            {
                get => _Flags.GetBit(0);
                set => _Flags = _Flags.SetBit(0, value);
            }

            public TVFormat TVFormat
            {
                get => (TVFormat) _TVFormatCEAIdPSFormatId;
                set => _TVFormatCEAIdPSFormatId = (byte) value;
            }

            public byte CEAId
            {
                get => _TVFormatCEAIdPSFormatId;
                set => _TVFormatCEAIdPSFormatId = value;
            }

            public byte PredefinedPSFormatId
            {
                get => _TVFormatCEAIdPSFormatId;
                set => _TVFormatCEAIdPSFormatId = value;
            }

            public byte Scaling
            {
                get => _Scaling;
                set => _Scaling = value;
            }

            public TimingFlag(bool isInterlaced, byte scaling) : this()
            {
                IsInterlaced = isInterlaced;
                Scaling = scaling;
            }

            public TimingFlag(bool isInterlaced, byte scaling, TVFormat tvFormat) : this(isInterlaced, scaling)
            {
                TVFormat = tvFormat;
            }

            public TimingFlag(bool isInterlaced, byte scaling, byte ceaIdOrPredefinedPSFormatId) : this(isInterlaced,
                scaling)
            {
                _TVFormatCEAIdPSFormatId = ceaIdOrPredefinedPSFormatId;
            }
        }

        internal StructureVersion _Version;
        internal uint _Width;
        internal uint _Height;
        internal float _RefreshRate;
        internal TimingFlag _Flags;
        internal TimingOverride _TimingType;

        /// <summary>
        ///     Gets the visible horizontal size
        /// </summary>
        public uint Width
        {
            get => _Width;
        }

        /// <summary>
        ///     Gets the visible vertical size
        /// </summary>
        public uint Height
        {
            get => _Height;
        }

        /// <summary>
        ///     Gets the timing refresh rate
        /// </summary>
        public float RefreshRate
        {
            get => _RefreshRate;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the requested timing is an interlaced timing
        /// </summary>
        public bool IsInterlaced
        {
            get => _Flags.IsInterlaced;
        }

        /// <summary>
        ///     Gets the preferred scaling
        /// </summary>
        public byte Scaling
        {
            get => _Flags.Scaling;
        }

        /// <summary>
        ///     Gets timing type (formula) to use for calculating the timing
        /// </summary>
        public TimingOverride TimingType
        {
            get => _TimingType;
        }

        /// <summary>
        ///     Creates an instance of the TimingInput
        /// </summary>
        /// <param name="width">The preferred visible horizontal size</param>
        /// <param name="height">The preferred visible vertical size</param>
        /// <param name="refreshRate">The preferred timing refresh rate</param>
        /// <param name="timingType">The preferred formula to be used for timing calculation</param>
        /// <param name="isInterlaced">A boolean value indicating if the preferred timing is interlaced</param>
        /// <param name="scaling">The preferred scaling factor</param>
        public TimingInput(
            uint width,
            uint height,
            float refreshRate,
            TimingOverride timingType,
            bool isInterlaced = false,
            byte scaling = 0
        )
        {
            this = typeof(TimingInput).Instantiate<TimingInput>();
            _Width = width;
            _Height = height;
            _RefreshRate = refreshRate;
            _TimingType = timingType;
            _Flags = new TimingFlag(isInterlaced, scaling);
        }

        /// <summary>
        ///     Creates an instance of the TimingInput
        /// </summary>
        /// <param name="tvFormat">The preferred analog TV format</param>
        /// <param name="isInterlaced">A boolean value indicating if the preferred timing is interlaced</param>
        /// <param name="scaling">The preferred scaling factor</param>
        public TimingInput(TVFormat tvFormat, bool isInterlaced = false, byte scaling = 0)
            : this(0, 0, 0, TimingOverride.AnalogTV, isInterlaced, scaling)
        {
            _Flags = new TimingFlag(isInterlaced, scaling, tvFormat);
        }

        /// <summary>
        ///     Creates an instance of the TimingInput
        /// </summary>
        /// <param name="ceaIdOrPredefinedPSFormatId">
        ///     The CEA id or the predefined PsF format id depending on the value of other
        ///     arguments
        /// </param>
        /// <param name="timingType">
        ///     The preferred formula to be used for timing calculation, valid values for this overload are
        ///     <see cref="TimingOverride.EIA861" /> and <see cref="TimingOverride.Predefined" />.
        /// </param>
        /// <param name="isInterlaced">A boolean value indicating if the preferred timing is interlaced</param>
        /// <param name="scaling">The preferred scaling factor</param>
        public TimingInput(
            byte ceaIdOrPredefinedPSFormatId,
            TimingOverride timingType,
            bool isInterlaced = false,
            byte scaling = 0
        )
            : this(0, 0, 0, timingType, isInterlaced, scaling)
        {
            if (timingType != TimingOverride.EIA861 && timingType != TimingOverride.Predefined)
            {
                throw new ArgumentException("Invalid timing type passed.", nameof(timingType));
            }

            _Flags = new TimingFlag(isInterlaced, scaling, ceaIdOrPredefinedPSFormatId);
        }

        /// <summary>
        ///     Creates an instance of the TimingInput
        /// </summary>
        /// <param name="timingType">
        ///     The preferred formula to be used for timing calculation.
        /// </param>
        public TimingInput(TimingOverride timingType)
            : this(0, 0, 0, timingType)
        {
        }

        /// <summary>
        ///     Gets the analog TV actual HD/SDTV format
        /// </summary>
        public TVFormat? TVFormat
        {
            get
            {
                if (Width == 0 && Height == 0 && Math.Abs(RefreshRate) < 0.01 && TimingType == TimingOverride.AnalogTV)
                {
                    return _Flags.TVFormat;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the EIA/CEA 861B/D predefined short timing descriptor id
        /// </summary>
        public byte? CEAId
        {
            get
            {
                if (Width == 0 && Height == 0 && Math.Abs(RefreshRate) < 0.01 && TimingType == TimingOverride.EIA861)
                {
                    return _Flags.CEAId;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the Nvidia predefined PsF format id
        /// </summary>
        public byte? PredefinedPSFormatId
        {
            get
            {
                if (TimingType == TimingOverride.Predefined)
                {
                    return _Flags.PredefinedPSFormatId;
                }

                return null;
            }
        }
    }
}