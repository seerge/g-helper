using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.General.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct ShortString : IInitializable
    {
        public const int ShortStringLength = 64;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ShortStringLength)]
        private readonly string _Value;

        public string Value
        {
            get => _Value;
        }

        public ShortString(string value)
        {
            _Value = value ?? string.Empty;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}