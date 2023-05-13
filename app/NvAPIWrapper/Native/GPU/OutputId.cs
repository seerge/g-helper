using System;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     GPU output IDs are identifiers for the GPU outputs that drive display devices. The GPU output might or might not be
    ///     connected to a display, or be active. Each output is identified by a bit setting within a 32-bit unsigned integer.
    ///     A GPU output mask consists of a 32-bit integer with several bits set, identifying more than one output from the
    ///     same physical GPU.
    /// </summary>
    [Flags]
    public enum OutputId : uint
    {
        /// <summary>
        ///     Invalid output if
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Represents Output 1
        /// </summary>
        Output1 = 1U,

        /// <summary>
        ///     Represents Output 2
        /// </summary>
        Output2 = 1u << 1,

        /// <summary>
        ///     Represents Output 3
        /// </summary>
        Output3 = 1u << 2,

        /// <summary>
        ///     Represents Output 4
        /// </summary>
        Output4 = 1u << 3,

        /// <summary>
        ///     Represents Output 5
        /// </summary>
        Output5 = 1u << 4,

        /// <summary>
        ///     Represents Output 6
        /// </summary>
        Output6 = 1u << 5,

        /// <summary>
        ///     Represents Output 7
        /// </summary>
        Output7 = 1u << 6,

        /// <summary>
        ///     Represents Output 8
        /// </summary>
        Output8 = 1u << 7,

        /// <summary>
        ///     Represents Output 9
        /// </summary>
        Output9 = 1u << 8,

        /// <summary>
        ///     Represents Output 10
        /// </summary>
        Output10 = 1u << 9,

        /// <summary>
        ///     Represents Output 11
        /// </summary>
        Output11 = 1u << 10,

        /// <summary>
        ///     Represents Output 12
        /// </summary>
        Output12 = 1u << 11,

        /// <summary>
        ///     Represents Output 13
        /// </summary>
        Output13 = 1u << 12,

        /// <summary>
        ///     Represents Output 14
        /// </summary>
        Output14 = 1u << 13,

        /// <summary>
        ///     Represents Output 15
        /// </summary>
        Output15 = 1u << 14,

        /// <summary>
        ///     Represents Output 16
        /// </summary>
        Output16 = 1u << 15,

        /// <summary>
        ///     Represents Output 17
        /// </summary>
        Output17 = 1u << 16,

        /// <summary>
        ///     Represents Output 18
        /// </summary>
        Output18 = 1u << 17,

        /// <summary>
        ///     Represents Output 19
        /// </summary>
        Output19 = 1u << 18,

        /// <summary>
        ///     Represents Output 20
        /// </summary>
        Output20 = 1u << 19,

        /// <summary>
        ///     Represents Output 21
        /// </summary>
        Output21 = 1u << 20,

        /// <summary>
        ///     Represents Output 22
        /// </summary>
        Output22 = 1u << 21,

        /// <summary>
        ///     Represents Output 23
        /// </summary>
        Output23 = 1u << 22,

        /// <summary>
        ///     Represents Output 24
        /// </summary>
        Output24 = 1u << 23,

        /// <summary>
        ///     Represents Output 25
        /// </summary>
        Output25 = 1u << 24,

        /// <summary>
        ///     Represents Output 26
        /// </summary>
        Output26 = 1u << 25,

        /// <summary>
        ///     Represents Output 27
        /// </summary>
        Output27 = 1u << 26,

        /// <summary>
        ///     Represents Output 28
        /// </summary>
        Output28 = 1u << 27,

        /// <summary>
        ///     Represents Output 29
        /// </summary>
        Output29 = 1u << 28,

        /// <summary>
        ///     Represents Output 30
        /// </summary>
        Output30 = 1u << 29,

        /// <summary>
        ///     Represents Output 31
        /// </summary>
        Output31 = 1u << 30,

        /// <summary>
        ///     Represents Output 32
        /// </summary>
        Output32 = 1u << 31
    }
}