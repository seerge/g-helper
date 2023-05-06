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
    [StructureVersion(2)]
    public struct DRSApplicationV2 : IInitializable, IDRSApplication
    {
        internal const char FileInFolderSeparator = ':';
        internal StructureVersion _Version;
        internal uint _IsPredefined;
        internal UnicodeString _ApplicationName;
        internal UnicodeString _FriendlyName;
        internal UnicodeString _LauncherName;
        internal UnicodeString _FileInFolder;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSApplicationV2" />
        /// </summary>
        /// <param name="applicationName">The application file name.</param>
        /// <param name="friendlyName">The application friendly name.</param>
        /// <param name="launcherName">The application launcher name.</param>
        /// <param name="fileInFolders">The list of files that are necessary to be present in the application parent directory.</param>
        // ReSharper disable once TooManyDependencies
        public DRSApplicationV2(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null
        )
        {
            this = typeof(DRSApplicationV2).Instantiate<DRSApplicationV2>();
            IsPredefined = false;
            ApplicationName = applicationName;
            FriendlyName = friendlyName ?? string.Empty;
            LauncherName = launcherName ?? string.Empty;
            FilesInFolder = fileInFolders ?? new string[0];
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

        /// <summary>
        ///     Gets the list of files that are necessary to be present in the application parent directory.
        /// </summary>
        public string[] FilesInFolder
        {
            get => _FileInFolder.Value?.Split(new[] {FileInFolderSeparator}, StringSplitOptions.RemoveEmptyEntries) ??
                   new string[0];
            private set => _FileInFolder = new UnicodeString(string.Join(FileInFolderSeparator.ToString(), value));
        }
    }
}