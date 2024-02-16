namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible video signal standards
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff546632(v=vs.85).aspx
    /// </summary>
    public enum VideoSignalStandard : ushort
    {
        /// <summary>
        ///     Indicates that the variable has not yet been assigned a meaningful value.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        ///     Represents the Video Electronics Standards Association (VESA) Display Monitor Timing (DMT) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        VESA_DMT = 1,

        /// <summary>
        ///     Represents the VESA Generalized Timing Formula (GTF) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        VESA_GTF = 2,

        /// <summary>
        ///     Represents the VESA Coordinated Video Timing (CVT) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        VESA_CVT = 3,

        /// <summary>
        ///     Represents the IBM standard.
        /// </summary>
        IBM = 4,

        /// <summary>
        ///     Represents the Apple standard.
        /// </summary>
        Apple = 5,

        /// <summary>
        ///     Represents the National Television Standards Committee (NTSC) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NTSC_M = 6,

        /// <summary>
        ///     Represents the NTSC japanese standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NTSC_J = 7,

        /// <summary>
        ///     Represents the NTSC standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NTSC_443 = 8,

        /// <summary>
        ///     Represents the Phase Alteration Line (PAL) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_B = 9,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_B1 = 10,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_G = 11,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_H = 12,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_I = 13,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_D = 14,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_N = 15,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_NC = 16,

        /// <summary>
        ///     Represents the Systeme Electronic Pour Couleur Avec Memoire (SECAM) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_B = 17,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_D = 18,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_G = 19,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_H = 20,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_K = 21,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_K1 = 22,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_L = 23,

        /// <summary>
        ///     Represents the SECAM standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SECAM_L1 = 24,

        /// <summary>
        ///     Represents the Electronics Industries Association (EIA) standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        EIA_861 = 25,

        /// <summary>
        ///     Represents the EIA standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        EIA_861A = 26,

        /// <summary>
        ///     Represents the EIA standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        EIA_861B = 27,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_K = 28,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_K1 = 29,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_L = 30,

        /// <summary>
        ///     Represents the PAL standard.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PAL_M = 31,

        /// <summary>
        ///     Represents any video standard other than those represented by the previous constants in this enumeration.
        /// </summary>
        Other = 255
    }
}