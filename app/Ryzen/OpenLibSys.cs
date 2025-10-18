//-----------------------------------------------------------------------------
//     Author : hiyohiyo
//       Mail : hiyohiyo@crystalmark.info
//        Web : http://openlibsys.org/
//    License : The modified BSD license
//
//                     Copyright 2007-2009 OpenLibSys.org. All rights reserved.
//-----------------------------------------------------------------------------
// This is support library for WinRing0 1.3.x.

using System.Runtime.InteropServices;

namespace GHelper.Ryzen
{
    public class Ols : IDisposable
    {
        const string dllNameX64 = "WinRing0x64.dll";
        const string dllName = "WinRing0.dll";

        // for this support library
        public enum Status
        {
            NO_ERROR = 0,
            DLL_NOT_FOUND = 1,
            DLL_INCORRECT_VERSION = 2,
            DLL_INITIALIZE_ERROR = 3,
        }

        // for WinRing0
        public enum OlsDllStatus
        {
            OLS_DLL_NO_ERROR = 0,
            OLS_DLL_UNSUPPORTED_PLATFORM = 1,
            OLS_DLL_DRIVER_NOT_LOADED = 2,
            OLS_DLL_DRIVER_NOT_FOUND = 3,
            OLS_DLL_DRIVER_UNLOADED = 4,
            OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK = 5,
            OLS_DLL_UNKNOWN_ERROR = 9
        }

        // for WinRing0
        public enum OlsDriverType
        {
            OLS_DRIVER_TYPE_UNKNOWN = 0,
            OLS_DRIVER_TYPE_WIN_9X = 1,
            OLS_DRIVER_TYPE_WIN_NT = 2,
            OLS_DRIVER_TYPE_WIN_NT4 = 3,    // Obsolete
            OLS_DRIVER_TYPE_WIN_NT_X64 = 4,
            OLS_DRIVER_TYPE_WIN_NT_IA64 = 5
        }

        // for WinRing0
        public enum OlsErrorPci : uint
        {
            OLS_ERROR_PCI_BUS_NOT_EXIST = 0xE0000001,
            OLS_ERROR_PCI_NO_DEVICE = 0xE0000002,
            OLS_ERROR_PCI_WRITE_CONFIG = 0xE0000003,
            OLS_ERROR_PCI_READ_CONFIG = 0xE0000004
        }

        // Bus Number, Device Number and Function Number to PCI Device Address
        public uint PciBusDevFunc(uint bus, uint dev, uint func)
        {
            return (bus & 0xFF) << 8 | (dev & 0x1F) << 3 | func & 7;
        }

        // PCI Device Address to Bus Number
        public uint PciGetBus(uint address)
        {
            return address >> 8 & 0xFF;
        }

        // PCI Device Address to Device Number
        public uint PciGetDev(uint address)
        {
            return address >> 3 & 0x1F;
        }

        // PCI Device Address to Function Number
        public uint PciGetFunc(uint address)
        {
            return address & 7;
        }

        [DllImport("kernel32")]
        public extern static nint LoadLibrary(string lpFileName);


        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(nint hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        private static extern nint GetProcAddress(nint hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        private nint module = nint.Zero;
        private uint status = (uint)Status.NO_ERROR;

        public Ols()
        {
            string fileName;

            if (nint.Size == 8)
            {
                fileName = dllNameX64;
            }
            else
            {
                fileName = dllName;
            }

            module = LoadLibrary(fileName);
            if (module == nint.Zero)
            {
                status = (uint)Status.DLL_NOT_FOUND;
            }
            else
            {

                GetDllStatus = (_GetDllStatus)GetDelegate("GetDllStatus", typeof(_GetDllStatus));
                GetDllVersion = (_GetDllVersion)GetDelegate("GetDllVersion", typeof(_GetDllVersion));
                GetDriverVersion = (_GetDriverVersion)GetDelegate("GetDriverVersion", typeof(_GetDriverVersion));
                GetDriverType = (_GetDriverType)GetDelegate("GetDriverType", typeof(_GetDriverType));

                InitializeOls = (_InitializeOls)GetDelegate("InitializeOls", typeof(_InitializeOls));
                DeinitializeOls = (_DeinitializeOls)GetDelegate("DeinitializeOls", typeof(_DeinitializeOls));

                IsCpuid = (_IsCpuid)GetDelegate("IsCpuid", typeof(_IsCpuid));
                IsMsr = (_IsMsr)GetDelegate("IsMsr", typeof(_IsMsr));
                IsTsc = (_IsTsc)GetDelegate("IsTsc", typeof(_IsTsc));
                Hlt = (_Hlt)GetDelegate("Hlt", typeof(_Hlt));
                HltTx = (_HltTx)GetDelegate("HltTx", typeof(_HltTx));
                HltPx = (_HltPx)GetDelegate("HltPx", typeof(_HltPx));
                Rdmsr = (_Rdmsr)GetDelegate("Rdmsr", typeof(_Rdmsr));
                RdmsrTx = (_RdmsrTx)GetDelegate("RdmsrTx", typeof(_RdmsrTx));
                RdmsrPx = (_RdmsrPx)GetDelegate("RdmsrPx", typeof(_RdmsrPx));
                Wrmsr = (_Wrmsr)GetDelegate("Wrmsr", typeof(_Wrmsr));
                WrmsrTx = (_WrmsrTx)GetDelegate("WrmsrTx", typeof(_WrmsrTx));
                WrmsrPx = (_WrmsrPx)GetDelegate("WrmsrPx", typeof(_WrmsrPx));
                Rdpmc = (_Rdpmc)GetDelegate("Rdpmc", typeof(_Rdpmc));
                RdpmcTx = (_RdpmcTx)GetDelegate("RdpmcTx", typeof(_RdpmcTx));
                RdpmcPx = (_RdpmcPx)GetDelegate("RdpmcPx", typeof(_RdpmcPx));
                Cpuid = (_Cpuid)GetDelegate("Cpuid", typeof(_Cpuid));
                CpuidTx = (_CpuidTx)GetDelegate("CpuidTx", typeof(_CpuidTx));
                CpuidPx = (_CpuidPx)GetDelegate("CpuidPx", typeof(_CpuidPx));
                Rdtsc = (_Rdtsc)GetDelegate("Rdtsc", typeof(_Rdtsc));
                RdtscTx = (_RdtscTx)GetDelegate("RdtscTx", typeof(_RdtscTx));
                RdtscPx = (_RdtscPx)GetDelegate("RdtscPx", typeof(_RdtscPx));

                ReadIoPortByte = (_ReadIoPortByte)GetDelegate("ReadIoPortByte", typeof(_ReadIoPortByte));
                ReadIoPortWord = (_ReadIoPortWord)GetDelegate("ReadIoPortWord", typeof(_ReadIoPortWord));
                ReadIoPortDword = (_ReadIoPortDword)GetDelegate("ReadIoPortDword", typeof(_ReadIoPortDword));
                ReadIoPortByteEx = (_ReadIoPortByteEx)GetDelegate("ReadIoPortByteEx", typeof(_ReadIoPortByteEx));
                ReadIoPortWordEx = (_ReadIoPortWordEx)GetDelegate("ReadIoPortWordEx", typeof(_ReadIoPortWordEx));
                ReadIoPortDwordEx = (_ReadIoPortDwordEx)GetDelegate("ReadIoPortDwordEx", typeof(_ReadIoPortDwordEx));

                WriteIoPortByte = (_WriteIoPortByte)GetDelegate("WriteIoPortByte", typeof(_WriteIoPortByte));
                WriteIoPortWord = (_WriteIoPortWord)GetDelegate("WriteIoPortWord", typeof(_WriteIoPortWord));
                WriteIoPortDword = (_WriteIoPortDword)GetDelegate("WriteIoPortDword", typeof(_WriteIoPortDword));
                WriteIoPortByteEx = (_WriteIoPortByteEx)GetDelegate("WriteIoPortByteEx", typeof(_WriteIoPortByteEx));
                WriteIoPortWordEx = (_WriteIoPortWordEx)GetDelegate("WriteIoPortWordEx", typeof(_WriteIoPortWordEx));
                WriteIoPortDwordEx = (_WriteIoPortDwordEx)GetDelegate("WriteIoPortDwordEx", typeof(_WriteIoPortDwordEx));

                SetPciMaxBusIndex = (_SetPciMaxBusIndex)GetDelegate("SetPciMaxBusIndex", typeof(_SetPciMaxBusIndex));
                ReadPciConfigByte = (_ReadPciConfigByte)GetDelegate("ReadPciConfigByte", typeof(_ReadPciConfigByte));
                ReadPciConfigWord = (_ReadPciConfigWord)GetDelegate("ReadPciConfigWord", typeof(_ReadPciConfigWord));
                ReadPciConfigDword = (_ReadPciConfigDword)GetDelegate("ReadPciConfigDword", typeof(_ReadPciConfigDword));
                ReadPciConfigByteEx = (_ReadPciConfigByteEx)GetDelegate("ReadPciConfigByteEx", typeof(_ReadPciConfigByteEx));
                ReadPciConfigWordEx = (_ReadPciConfigWordEx)GetDelegate("ReadPciConfigWordEx", typeof(_ReadPciConfigWordEx));
                ReadPciConfigDwordEx = (_ReadPciConfigDwordEx)GetDelegate("ReadPciConfigDwordEx", typeof(_ReadPciConfigDwordEx));
                ReadPciConfigDwordEx64 = (_ReadPciConfigDwordEx64)GetDelegate("ReadPciConfigDwordEx", typeof(_ReadPciConfigDwordEx64));
                WritePciConfigByte = (_WritePciConfigByte)GetDelegate("WritePciConfigByte", typeof(_WritePciConfigByte));
                WritePciConfigWord = (_WritePciConfigWord)GetDelegate("WritePciConfigWord", typeof(_WritePciConfigWord));
                WritePciConfigDword = (_WritePciConfigDword)GetDelegate("WritePciConfigDword", typeof(_WritePciConfigDword));
                WritePciConfigByteEx = (_WritePciConfigByteEx)GetDelegate("WritePciConfigByteEx", typeof(_WritePciConfigByteEx));
                WritePciConfigWordEx = (_WritePciConfigWordEx)GetDelegate("WritePciConfigWordEx", typeof(_WritePciConfigWordEx));
                WritePciConfigDwordEx = (_WritePciConfigDwordEx)GetDelegate("WritePciConfigDwordEx", typeof(_WritePciConfigDwordEx));
                WritePciConfigDwordEx64 = (_WritePciConfigDwordEx64)GetDelegate("WritePciConfigDwordEx", typeof(_WritePciConfigDwordEx64));
                FindPciDeviceById = (_FindPciDeviceById)GetDelegate("FindPciDeviceById", typeof(_FindPciDeviceById));
                FindPciDeviceByClass = (_FindPciDeviceByClass)GetDelegate("FindPciDeviceByClass", typeof(_FindPciDeviceByClass));

                /*
                ReadDmiMemory = (_ReadDmiMemory)GetDelegate("ReadDmiMemory", typeof(_ReadDmiMemory));
                ReadPhysicalMemory = (_ReadPhysicalMemory)GetDelegate("ReadPhysicalMemory", typeof(_ReadPhysicalMemory));
                WritePhysicalMemory = (_WritePhysicalMemory)GetDelegate("WritePhysicalMemory", typeof(_WritePhysicalMemory));
				*/

                if (!(
                   GetDllStatus != null
                && GetDllVersion != null
                && GetDriverVersion != null
                && GetDriverType != null
                && InitializeOls != null
                && DeinitializeOls != null
                && IsCpuid != null
                && IsMsr != null
                && IsTsc != null
                && Hlt != null
                && HltTx != null
                && HltPx != null
                && Rdmsr != null
                && RdmsrTx != null
                && RdmsrPx != null
                && Wrmsr != null
                && WrmsrTx != null
                && WrmsrPx != null
                && Rdpmc != null
                && RdpmcTx != null
                && RdpmcPx != null
                && Cpuid != null
                && CpuidTx != null
                && CpuidPx != null
                && Rdtsc != null
                && RdtscTx != null
                && RdtscPx != null
                && ReadIoPortByte != null
                && ReadIoPortWord != null
                && ReadIoPortDword != null
                && ReadIoPortByteEx != null
                && ReadIoPortWordEx != null
                && ReadIoPortDwordEx != null
                && WriteIoPortByte != null
                && WriteIoPortWord != null
                && WriteIoPortDword != null
                && WriteIoPortByteEx != null
                && WriteIoPortWordEx != null
                && WriteIoPortDwordEx != null
                && SetPciMaxBusIndex != null
                && ReadPciConfigByte != null
                && ReadPciConfigWord != null
                && ReadPciConfigDword != null
                && ReadPciConfigByteEx != null
                && ReadPciConfigWordEx != null
                && ReadPciConfigDwordEx != null
                && ReadPciConfigDwordEx64 != null
                && WritePciConfigByte != null
                && WritePciConfigWord != null
                && WritePciConfigDword != null
                && WritePciConfigByteEx != null
                && WritePciConfigWordEx != null
                && WritePciConfigDwordEx != null
                && WritePciConfigDwordEx64 != null
                && FindPciDeviceById != null
                && FindPciDeviceByClass != null
                /*&& ReadDmiMemory != null
                && ReadPhysicalMemory != null
                && WritePhysicalMemory != null
				*/
                ))
                {
                    status = (uint)Status.DLL_INCORRECT_VERSION;
                }

                if (InitializeOls() == 0)
                {
                    status = (uint)Status.DLL_INITIALIZE_ERROR;
                }
            }
        }

        public uint GetStatus()
        {
            return status;
        }

        public void Dispose()
        {
            if (module != nint.Zero)
            {
                DeinitializeOls();
                FreeLibrary(module);
                module = nint.Zero;
            }
        }

        public Delegate GetDelegate(string procName, Type delegateType)
        {
            nint ptr = GetProcAddress(module, procName);
            if (ptr != nint.Zero)
            {
                Delegate d = Marshal.GetDelegateForFunctionPointer(ptr, delegateType);
                return d;
            }

            int result = Marshal.GetHRForLastWin32Error();
            throw Marshal.GetExceptionForHR(result);
        }

        //-----------------------------------------------------------------------------
        // DLL Information
        //-----------------------------------------------------------------------------
        public delegate uint _GetDllStatus();
        public delegate uint _GetDllVersion(ref byte major, ref byte minor, ref byte revision, ref byte release);
        public delegate uint _GetDriverVersion(ref byte major, ref byte minor, ref byte revision, ref byte release);
        public delegate uint _GetDriverType();

        public delegate int _InitializeOls();
        public delegate void _DeinitializeOls();

        public _GetDllStatus GetDllStatus = null;
        public _GetDriverType GetDriverType = null;
        public _GetDllVersion GetDllVersion = null;
        public _GetDriverVersion GetDriverVersion = null;

        public _InitializeOls InitializeOls = null;
        public _DeinitializeOls DeinitializeOls = null;

        //-----------------------------------------------------------------------------
        // CPU
        //-----------------------------------------------------------------------------
        public delegate int _IsCpuid();
        public delegate int _IsMsr();
        public delegate int _IsTsc();
        public delegate int _Hlt();
        public delegate int _HltTx(nuint threadAffinityMask);
        public delegate int _HltPx(nuint processAffinityMask);
        public delegate int _Rdmsr(uint index, ref uint eax, ref uint edx);
        public delegate int _RdmsrTx(uint index, ref uint eax, ref uint edx, nuint threadAffinityMask);
        public delegate int _RdmsrPx(uint index, ref uint eax, ref uint edx, nuint processAffinityMask);
        public delegate int _Wrmsr(uint index, uint eax, uint edx);
        public delegate int _WrmsrTx(uint index, uint eax, uint edx, nuint threadAffinityMask);
        public delegate int _WrmsrPx(uint index, uint eax, uint edx, nuint processAffinityMask);
        public delegate int _Rdpmc(uint index, ref uint eax, ref uint edx);
        public delegate int _RdpmcTx(uint index, ref uint eax, ref uint edx, nuint threadAffinityMask);
        public delegate int _RdpmcPx(uint index, ref uint eax, ref uint edx, nuint processAffinityMask);
        public delegate int _Cpuid(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx);
        public delegate int _CpuidTx(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx, nuint threadAffinityMask);
        public delegate int _CpuidPx(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx, nuint processAffinityMask);
        public delegate int _Rdtsc(ref uint eax, ref uint edx);
        public delegate int _RdtscTx(ref uint eax, ref uint edx, nuint threadAffinityMask);
        public delegate int _RdtscPx(ref uint eax, ref uint edx, nuint processAffinityMask);

        public _IsCpuid IsCpuid = null;
        public _IsMsr IsMsr = null;
        public _IsTsc IsTsc = null;
        public _Hlt Hlt = null;
        public _HltTx HltTx = null;
        public _HltPx HltPx = null;
        public _Rdmsr Rdmsr = null;
        public _RdmsrTx RdmsrTx = null;
        public _RdmsrPx RdmsrPx = null;
        public _Wrmsr Wrmsr = null;
        public _WrmsrTx WrmsrTx = null;
        public _WrmsrPx WrmsrPx = null;
        public _Rdpmc Rdpmc = null;
        public _RdpmcTx RdpmcTx = null;
        public _RdpmcPx RdpmcPx = null;
        public _Cpuid Cpuid = null;
        public _CpuidTx CpuidTx = null;
        public _CpuidPx CpuidPx = null;
        public _Rdtsc Rdtsc = null;
        public _RdtscTx RdtscTx = null;
        public _RdtscPx RdtscPx = null;

        //-----------------------------------------------------------------------------
        // I/O
        //-----------------------------------------------------------------------------
        public delegate byte _ReadIoPortByte(ushort port);
        public delegate ushort _ReadIoPortWord(ushort port);
        public delegate uint _ReadIoPortDword(ushort port);
        public _ReadIoPortByte ReadIoPortByte;
        public _ReadIoPortWord ReadIoPortWord;
        public _ReadIoPortDword ReadIoPortDword;

        public delegate int _ReadIoPortByteEx(ushort port, ref byte value);
        public delegate int _ReadIoPortWordEx(ushort port, ref ushort value);
        public delegate int _ReadIoPortDwordEx(ushort port, ref uint value);
        public _ReadIoPortByteEx ReadIoPortByteEx;
        public _ReadIoPortWordEx ReadIoPortWordEx;
        public _ReadIoPortDwordEx ReadIoPortDwordEx;

        public delegate void _WriteIoPortByte(ushort port, byte value);
        public delegate void _WriteIoPortWord(ushort port, ushort value);
        public delegate void _WriteIoPortDword(ushort port, uint value);
        public _WriteIoPortByte WriteIoPortByte;
        public _WriteIoPortWord WriteIoPortWord;
        public _WriteIoPortDword WriteIoPortDword;

        public delegate int _WriteIoPortByteEx(ushort port, byte value);
        public delegate int _WriteIoPortWordEx(ushort port, ushort value);
        public delegate int _WriteIoPortDwordEx(ushort port, uint value);
        public _WriteIoPortByteEx WriteIoPortByteEx;
        public _WriteIoPortWordEx WriteIoPortWordEx;
        public _WriteIoPortDwordEx WriteIoPortDwordEx;

        //-----------------------------------------------------------------------------
        // PCI
        //-----------------------------------------------------------------------------
        public delegate void _SetPciMaxBusIndex(byte max);
        public _SetPciMaxBusIndex SetPciMaxBusIndex;

        public delegate byte _ReadPciConfigByte(uint pciAddress, byte regAddress);
        public delegate ushort _ReadPciConfigWord(uint pciAddress, byte regAddress);
        public delegate uint _ReadPciConfigDword(uint pciAddress, byte regAddress);
        public _ReadPciConfigByte ReadPciConfigByte;
        public _ReadPciConfigWord ReadPciConfigWord;
        public _ReadPciConfigDword ReadPciConfigDword;

        public delegate int _ReadPciConfigByteEx(uint pciAddress, uint regAddress, ref byte value);
        public delegate int _ReadPciConfigWordEx(uint pciAddress, uint regAddress, ref ushort value);
        public delegate int _ReadPciConfigDwordEx(uint pciAddress, uint regAddress, ref uint value);
        public _ReadPciConfigByteEx ReadPciConfigByteEx;
        public _ReadPciConfigWordEx ReadPciConfigWordEx;
        public _ReadPciConfigDwordEx ReadPciConfigDwordEx;


        public delegate int _ReadPciConfigDwordEx64(uint pciAddress, uint regAddress, ref ulong value);
        public _ReadPciConfigDwordEx64 ReadPciConfigDwordEx64;

        public delegate void _WritePciConfigByte(uint pciAddress, byte regAddress, byte value);
        public delegate void _WritePciConfigWord(uint pciAddress, byte regAddress, ushort value);
        public delegate void _WritePciConfigDword(uint pciAddress, byte regAddress, uint value);
        public _WritePciConfigByte WritePciConfigByte;
        public _WritePciConfigWord WritePciConfigWord;
        public _WritePciConfigDword WritePciConfigDword;

        public delegate int _WritePciConfigByteEx(uint pciAddress, uint regAddress, byte value);
        public delegate int _WritePciConfigWordEx(uint pciAddress, uint regAddress, ushort value);
        public delegate int _WritePciConfigDwordEx(uint pciAddress, uint regAddress, uint value);
        public _WritePciConfigByteEx WritePciConfigByteEx;
        public _WritePciConfigWordEx WritePciConfigWordEx;
        public _WritePciConfigDwordEx WritePciConfigDwordEx;

        public delegate int _WritePciConfigDwordEx64(uint pciAddress, uint regAddress, ulong value);
        public _WritePciConfigDwordEx64 WritePciConfigDwordEx64;

        public delegate uint _FindPciDeviceById(ushort vendorId, ushort deviceId, byte index);
        public delegate uint _FindPciDeviceByClass(byte baseClass, byte subClass, byte programIf, byte index);
        public _FindPciDeviceById FindPciDeviceById;
        public _FindPciDeviceByClass FindPciDeviceByClass;

        //-----------------------------------------------------------------------------
        // Physical Memory (unsafe)
        //-----------------------------------------------------------------------------

        /*public unsafe delegate uint _ReadDmiMemory(byte* buffer, uint count, uint unitSize);
        public _ReadDmiMemory ReadDmiMemory;

        public unsafe delegate uint _ReadPhysicalMemory(UIntPtr address, byte* buffer, uint count, uint unitSize);
        public unsafe delegate uint _WritePhysicalMemory(UIntPtr address, byte* buffer, uint count, uint unitSize);

        public _ReadPhysicalMemory ReadPhysicalMemory;
        public _WritePhysicalMemory WritePhysicalMemory;
		*/
    }
}

