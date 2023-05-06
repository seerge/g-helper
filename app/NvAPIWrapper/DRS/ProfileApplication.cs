using System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.Interfaces.DRS;

namespace NvAPIWrapper.DRS
{
    /// <summary>
    ///     Represents an application rule registered in a profile
    /// </summary>
    public class ProfileApplication
    {
        private IDRSApplication _application;

        internal ProfileApplication(IDRSApplication application, DriverSettingsProfile profile)
        {
            Profile = profile;
            _application = application;
        }

        /// <summary>
        ///     Gets the application name
        /// </summary>
        public string ApplicationName
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                return _application.ApplicationName;
            }
        }

        /// <summary>
        ///     Gets the application command line
        /// </summary>
        public string CommandLine
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                if (_application is DRSApplicationV4 applicationV4)
                {
                    return applicationV4.ApplicationCommandLine;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets a list of files that are necessary to be present inside the application parent directory
        /// </summary>
        public string[] FilesInFolder
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                if (_application is DRSApplicationV2 applicationV2)
                {
                    return applicationV2.FilesInFolder;
                }

                if (_application is DRSApplicationV3 applicationV3)
                {
                    return applicationV3.FilesInFolder;
                }

                if (_application is DRSApplicationV4 applicationV4)
                {
                    return applicationV4.FilesInFolder;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the application friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                return _application.FriendlyName;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this application rule needs a specific command line; or <see langword="null" />
        ///     if this information is not available.
        /// </summary>
        public bool? HasCommandLine
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                if (_application is DRSApplicationV3 applicationV3)
                {
                    return applicationV3.HasCommandLine;
                }

                if (_application is DRSApplicationV4 applicationV4)
                {
                    return applicationV4.HasCommandLine;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this application is a metro application; or <see langword="null" /> if this
        ///     information is not available.
        /// </summary>
        public bool? IsMetroApplication
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                if (_application is DRSApplicationV3 applicationV3)
                {
                    return applicationV3.IsMetroApplication;
                }

                if (_application is DRSApplicationV4 applicationV4)
                {
                    return applicationV4.IsMetroApplication;
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this application is predefined by the NVIDIA driver
        /// </summary>
        public bool IsPredefined
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                return _application.IsPredefined;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if this instance of <see cref="ProfileApplication" /> is a valid instance
        ///     representing an application in a profile
        /// </summary>
        public bool IsValid
        {
            get => _application != null && Profile.IsValid;
        }

        /// <summary>
        ///     Gets the application launcher name
        /// </summary>
        public string LauncherName
        {
            get
            {
                if (!IsValid)
                {
                    throw new InvalidOperationException(
                        "Can not perform this operation with an invalid application instance."
                    );
                }

                return _application.LauncherName;
            }
        }

        /// <summary>
        ///     Gets the parent profile instance
        /// </summary>
        public DriverSettingsProfile Profile { get; }

        /// <summary>
        ///     Creates a new application
        /// </summary>
        /// <param name="profile">The profile to create the new application in.</param>
        /// <param name="applicationName">The application name (with extension).</param>
        /// <param name="friendlyName">The application friendly name.</param>
        /// <param name="launcherName">The application launcher name.</param>
        /// <param name="fileInFolders">An array of files necessary to be present inside the application parent directory.</param>
        /// <param name="isMetro">A boolean value indicating if this application is a metro application.</param>
        /// <param name="commandLine">The application command line string.</param>
        /// <returns>A new instance of <see cref="ProfileApplication" /> representing the newly created application.</returns>
        // ReSharper disable once TooManyArguments
        // ReSharper disable once FunctionComplexityOverflow
        public static ProfileApplication CreateApplication(
            DriverSettingsProfile profile,
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            var createDelegates = new Func<string, string, string, string[], bool, string, IDRSApplication>[]
            {
                CreateApplicationInstanceV4,
                CreateApplicationInstanceV3,
                CreateApplicationInstanceV2,
                CreateApplicationInstanceV1
            };

            Exception lastException = null;
            IDRSApplication application = null;

            foreach (var func in createDelegates)
            {
                try
                {
                    // ReSharper disable once EventExceptionNotDocumented
                    application = func(
                        applicationName,
                        friendlyName,
                        launcherName,
                        fileInFolders,
                        isMetro,
                        commandLine
                    );

                    break;
                }
                catch (NVIDIANotSupportedException e)
                {
                    // ignore
                    lastException = e;
                }
            }

            if (application == null)
            {
                // ReSharper disable once ThrowingSystemException
                throw lastException;
            }

            application = DRSApi.CreateApplication(profile.Session.Handle, profile.Handle, application);

            return new ProfileApplication(application, profile);
        }

        // ReSharper disable once TooManyArguments
        private static IDRSApplication CreateApplicationInstanceV1(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            if (!string.IsNullOrWhiteSpace(commandLine))
            {
                throw new NotSupportedException(
                    "CommandLine is not supported with the current execution environment."
                );
            }

            if (fileInFolders?.Length > 0)
            {
                throw new NotSupportedException(
                    "Same folder file presence check is not supported with the current execution environment."
                );
            }

            return new DRSApplicationV1(
                applicationName,
                friendlyName,
                launcherName
            );
        }

        // ReSharper disable once TooManyArguments
        private static IDRSApplication CreateApplicationInstanceV2(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            if (!string.IsNullOrWhiteSpace(commandLine))
            {
                throw new NotSupportedException(
                    "CommandLine is not supported with the current execution environment."
                );
            }

            return new DRSApplicationV2(
                applicationName,
                friendlyName,
                launcherName,
                fileInFolders
            );
        }

        // ReSharper disable once TooManyArguments
        private static IDRSApplication CreateApplicationInstanceV3(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            if (!string.IsNullOrWhiteSpace(commandLine))
            {
                throw new NotSupportedException(
                    "CommandLine is not supported with the current execution environment."
                );
            }

            return new DRSApplicationV3(
                applicationName,
                friendlyName,
                launcherName,
                fileInFolders,
                isMetro
            );
        }

        // ReSharper disable once TooManyArguments
        private static IDRSApplication CreateApplicationInstanceV4(
            string applicationName,
            string friendlyName = null,
            string launcherName = null,
            string[] fileInFolders = null,
            bool isMetro = false,
            string commandLine = null
        )
        {
            return new DRSApplicationV4(
                applicationName,
                friendlyName,
                launcherName,
                fileInFolders,
                isMetro,
                commandLine
            );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (!IsValid)
            {
                return "[Invalid]";
            }

            if (IsPredefined)
            {
                return $"{ApplicationName} (Predefined)";
            }

            return ApplicationName;
        }

        /// <summary>
        ///     Deletes this application and makes this instance invalid
        /// </summary>
        public void Delete()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException(
                    "Can not perform this operation with an invalid application instance."
                );
            }

            DRSApi.DeleteApplication(Profile.Session.Handle, Profile.Handle, _application);
            _application = null;
        }
    }
}