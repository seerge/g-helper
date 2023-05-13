using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.DRS;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <inheritdoc cref="IDRSApplication" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DRSApplicationV1 : IInitializable, IDRSApplication
    {
        internal StructureVersion _Version;
        internal uint _IsPredefined;
        internal UnicodeString _ApplicationName;
        internal UnicodeString _FriendlyName;
        internal UnicodeString _LauncherName;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSApplicationV1" />
        /// </summary>
        /// <param name="applicationName">The application file name.</param>
        /// <param name="friendlyName">The application friendly name.</param>
        /// <param name="launcherName">The application launcher name.</param>
        public DRSApplicationV1(
            string applicationName,
            string friendlyName = null,
            string launcherName = null
        )
        {
            this = typeof(DRSApplicationV1).Instantiate<DRSApplicationV1>();
            IsPredefined = false;
            ApplicationName = applicationName;
            FriendlyName = friendlyName ?? string.Empty;
            LauncherName = launcherName ?? string.Empty;
        }

        /// <inheritdoc />
        public bool IsPredefined
        {
            get => _IsPredefined > 0;
            private set => _IsPredefined = value ? 1u : 0u;
        }

        /// <inheritdoc />
        public string ApplicationName
        {
            get => _ApplicationName.Value;
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Name can not be empty or null.");
                }

                _ApplicationName = new UnicodeString(value);
            }
        }

        /// <inheritdoc />
        public string FriendlyName
        {
            get => _FriendlyName.Value;
            private set => _FriendlyName = new UnicodeString(value);
        }

        /// <inheritdoc />
        public string LauncherName
        {
            get => _LauncherName.Value;
            private set => _LauncherName = new UnicodeString(value);
        }
    }
}