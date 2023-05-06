namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible audio channel allocations (speaker placements)
    /// </summary>
    public enum InfoFrameAudioChannelAllocation : uint
    {
        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        FrFl = 0,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        LfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        FcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Front Center [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        FcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        RcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Front Center [5] Low Frequency Effects [6] Front Right [7] Front
        ///     Left
        /// </summary>
        RcFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RrRlFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        RrRlLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        RrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcRrRlFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        RcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcRrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right
        ///     [7] Front Left
        /// </summary>
        RcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        RrcRlcRrRlFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        RrcRlcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front
        ///     Right [7] Front Left
        /// </summary>
        RrcRlcRrRlFcFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        RrcRlcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Empty [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        FrcFlcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Empty [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FrcFlcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcRcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Front Center [5] Empty [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrcFlcRcFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRcFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcRrRlFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FchRrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FchRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        TcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        TcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FrhFlhRrRlFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrhFlhRrRlLfeFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FrwFlwRrRlFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrwFlwRrRlLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        TcRcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        TcRcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Center High [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        FchRcRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Center High [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FchRcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        TcFcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        TcFchRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrhFlhRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrhFlhRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrwFlwRrRlFcFeFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrwFlwRrRlFcLfeFrFl,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 511
    }
}