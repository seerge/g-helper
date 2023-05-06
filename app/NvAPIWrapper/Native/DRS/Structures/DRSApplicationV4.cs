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
    [StructureVersion(4)]
    public struct DRSApplicationV4 : IInitializable, IDRSApplication
    {
        internal const char FileInFolderSeparator = DRSApplicationV3.FileInFolderSeparator;
        internal StructureVersion _Version;
        internal uint _IsPredefined;
        internal UnicodeString _ApplicationName;
        internal UnicodeString _FriendlyName;
        internal UnicodeString _LauncherName;
        internal UnicodeString _FileInFolder;
        internal uint _Flags;
        internal UnicodeString _CommandLine;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSApplicationV4" />
        /// </summary>
        /// <param name="applicationName">The application file name.</param>
        /// <param name="friendlyName">The application friendly name.</param>
        /// <param name="launcherName">The application launcher name.</param>
        /// <param name="fileInFolders">The list of files that are necessary to be present in the application parent directory.</param>
        /// <param name="isMetro">A boolean value indicating if this application is a metro application.</param>
        /// <param name="commandLine">The application's command line arguments.</param>
        // ReSharper disable once TooManyDependencies
        public DRSApplicationV4(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            this = typeof(DRSApplicationV4).Instantiate<DRSApplicationV4>();
            IsPredefined = false;
            ApplicationName = applicationName;
            FriendlyName = friendlyName ?? string.Empty;
            LauncherName = launcherName ?? string.Empty;
            FilesInFolder = fileInFolders ?? new string[0];
            IsMetroApplication = isMetro;
            ApplicationCommandLine = commandLine ?? string.Empty;
        }

        /// <inheritdoc />
        public bool IsPredefined
        {
            get => _IsPredefined > 0;
            private set => _IsPredefined = value ? 1u : 0u;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this application is a metro application
        /// </summary>
        public bool IsMetroApplication
        {
            get => _Flags.GetBit(0);
            private set => _Flags = _Flags.SetBit(0, value);
        }

        /// <summary>
        ///     Gets a boolean value indicating if this application has command line arguments
        /// </summary>
        public bool HasCommandLine
        {
            get => _Flags.GetBit(1);
            private set => _Flags = _Flags.SetBit(1, value);
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

        /// <summary>
        ///     Gets the application command line arguments
        /// </summary>
        public string ApplicationCommandLine
        {
            get => (HasCommandLine ? _CommandLine.Value : null) ?? string.Empty;
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _CommandLine = new UnicodeString(null);

                    if (HasCommandLine)
                    {
                        HasCommandLine = false;
                    }
                }
                else
                {
                    _CommandLine = new UnicodeString(value);

                    if (!HasCommandLine)
                    {
                        HasCommandLine = true;
                    }
                }
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