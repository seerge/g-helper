using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Exceptions;

namespace NvAPIWrapper.Native.Helpers
{
    internal static class DelegateFactory
    {
        private static readonly Dictionary<KeyValuePair<FunctionId, Type>, object> Delegates =
            new Dictionary<KeyValuePair<FunctionId, Type>, object>();

        public static T GetDelegate<T>() where T : class
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidOperationException($"{typeof(T).Name} is not a delegate type");
            }

            var functionId = typeof(T).GetCustomAttributes(typeof(FunctionIdAttribute), true)
                .Cast<FunctionIdAttribute>()
                .FirstOrDefault();

            if (functionId == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name}'s address is unknown.");
            }

            var delegateKey = new KeyValuePair<FunctionId, Type>(functionId.FunctionId, typeof(T));

            lock (Delegates)
            {
                if (Delegates.ContainsKey(delegateKey))
                {
                    return Delegates[delegateKey] as T;
                }

                var ptr = NvAPI_QueryInterface((uint) functionId.FunctionId);

                if (ptr != IntPtr.Zero)
                {
                    var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
                    Delegates.Add(delegateKey, delegateValue);

                    return delegateValue;
                }
            }

            throw new NVIDIANotSupportedException(@"Function identification number is invalid or not supported.");
        }

        private static IntPtr NvAPI_QueryInterface(uint interfaceId)
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                throw new NVIDIANotSupportedException(
                    "32bit process running in a 64bit environment can't access NVIDIA API.");
            }

            return Environment.Is64BitProcess
                ? NvAPI64_QueryInterface(interfaceId)
                : NvAPI32_QueryInterface(interfaceId);
        }

        [DllImport(@"nvapi", EntryPoint = @"nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl,
            PreserveSig = true)]
        private static extern IntPtr NvAPI32_QueryInterface(uint interfaceId);

        [DllImport(@"nvapi64", EntryPoint = @"nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl,
            PreserveSig = true)]
        private static extern IntPtr NvAPI64_QueryInterface(uint interfaceId);
    }
}