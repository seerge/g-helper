using System;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.General;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains system and general static functions
    /// </summary>
    public static class GeneralApi
    {
        /// <summary>
        ///     This function returns information about the system's chipset.
        /// </summary>
        /// <returns>Information about the system's chipset</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid argument</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IChipsetInfo GetChipsetInfo()
        {
            var getChipSetInfo = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetChipSetInfo>();

            foreach (var acceptType in getChipSetInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IChipsetInfo>();

                using (var chipsetInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getChipSetInfo(chipsetInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return chipsetInfoReference.ToValueType<IChipsetInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }


        /// <summary>
        ///     This API returns display driver version and driver-branch string.
        /// </summary>
        /// <param name="branchVersion">Contains the driver-branch string after successful return.</param>
        /// <returns>Returns driver version</returns>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        public static uint GetDriverAndBranchVersion(out string branchVersion)
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetDriverAndBranchVersion>()(
                out var driverVersion, out var branchVersionShortString);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            branchVersion = branchVersionShortString.Value;

            return driverVersion;
        }

        /// <summary>
        ///     This function converts an NvAPI error code into a null terminated string.
        /// </summary>
        /// <param name="statusCode">The error code to convert</param>
        /// <returns>The string corresponding to the error code</returns>
        // ReSharper disable once FlagArgument
        public static string GetErrorMessage(Status statusCode)
        {
            statusCode =
                DelegateFactory.GetDelegate<Delegates.General.NvAPI_GetErrorMessage>()(statusCode, out var message);

            if (statusCode != Status.Ok)
            {
                return null;
            }

            return message.Value;
        }

        /// <summary>
        ///     This function returns a string describing the version of the NvAPI library. The contents of the string are human
        ///     readable. Do not assume a fixed format.
        /// </summary>
        /// <returns>User readable string giving NvAPI version information</returns>
        /// <exception cref="NVIDIAApiException">See NVIDIAApiException.Status for the reason of the exception.</exception>
        public static string GetInterfaceVersionString()
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.General.NvAPI_GetInterfaceVersionString>()(out var version);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return version.Value;
        }

        /// <summary>
        ///     This function returns the current lid and dock information.
        /// </summary>
        /// <returns>Current lid and dock information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Requested feature not supported</exception>
        /// <exception cref="NVIDIAApiException">Status.HandleInvalidated: Handle is no longer valid</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NvAPI_Initialize() has not been called</exception>
        public static LidDockParameters GetLidAndDockInfo()
        {
            var dockInfo = typeof(LidDockParameters).Instantiate<LidDockParameters>();
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetLidAndDockInfo>()(ref dockInfo);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return dockInfo;
        }

        /// <summary>
        ///     This function initializes the NvAPI library (if not already initialized) but always increments the ref-counter.
        ///     This must be called before calling other NvAPI_ functions.
        /// </summary>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">Status.LibraryNotFound: nvapi.dll can not be loaded</exception>
        public static void Initialize()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_Initialize>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     PRIVATE - Requests to restart the display driver
        /// </summary>
        public static void RestartDisplayDriver()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_RestartDisplayDriver>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Decrements the ref-counter and when it reaches ZERO, unloads NVAPI library.
        ///     This must be called in pairs with NvAPI_Initialize.
        ///     Note: By design, it is not mandatory to call NvAPI_Initialize before calling any NvAPI.
        ///     When any NvAPI is called without first calling NvAPI_Initialize, the internal ref-counter will be implicitly
        ///     incremented. In such cases, calling NvAPI_Initialize from a different thread will result in incrementing the
        ///     ref-count again and the user has to call NvAPI_Unload twice to unload the library. However, note that the implicit
        ///     increment of the ref-counter happens only once.
        ///     If the client wants unload functionality, it is recommended to always call NvAPI_Initialize and NvAPI_Unload in
        ///     pairs.
        ///     Unloading NvAPI library is not supported when the library is in a resource locked state.
        ///     Some functions in the NvAPI library initiates an operation or allocates certain resources and there are
        ///     corresponding functions available, to complete the operation or free the allocated resources. All such function
        ///     pairs are designed to prevent unloading NvAPI library.
        ///     For example, if NvAPI_Unload is called after NvAPI_XXX which locks a resource, it fails with NVAPI_ERROR.
        ///     Developers need to call the corresponding NvAPI_YYY to unlock the resources, before calling NvAPI_Unload again.
        /// </summary>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.ApiInUse: At least an API is still being called hence cannot unload NVAPI
        ///     library from process
        /// </exception>
        public static void Unload()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_Unload>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }
    }
}