using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.General.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct GenericString : IInitializable
    {
        public const int GenericStringLength = 4096;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GenericStringLength)]
        private readonly string _Value;

        public string Value
        {
            get => _Value;
        }

        public GenericString(string value)
        {
            _Value = value ?? string.Empty;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}