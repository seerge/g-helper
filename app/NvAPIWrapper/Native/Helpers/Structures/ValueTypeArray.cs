using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Helpers.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueTypeArray : IDisposable, IHandle, IEquatable<ValueTypeArray>
    {
        // ReSharper disable once ConvertToAutoProperty
        public IntPtr MemoryAddress { get; }

        public static ValueTypeArray Null
        {
            get => new ValueTypeArray();
        }

        public bool IsNull
        {
            get => MemoryAddress == IntPtr.Zero;
        }

        public ValueTypeArray(IntPtr memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public static ValueTypeArray FromArray(IEnumerable<object> list)
        {
            var array = list.ToArray();

            if (array.Length > 0)
            {
                if (array[0] == null || !array[0].GetType().IsValueType)
                {
                    throw new ArgumentException("Only Value Types are acceptable.", nameof(list));
                }

                var type = array[0].GetType();

                if (array.Any(item => item.GetType() != type))
                {
                    throw new ArgumentException("Array should not hold objects of multiple types.", nameof(list));
                }

                return FromArray(array, type);
            }

            return Null;
        }


        // ReSharper disable once ExcessiveIndentation
        // ReSharper disable once MethodTooLong
        public static ValueTypeArray FromArray(IEnumerable<object> list, Type type)
        {
            var array = list.ToArray();

            if (array.Length > 0)
            {
                var typeSize = Marshal.SizeOf(type);
                var memoryAddress = Marshal.AllocHGlobal(array.Length * typeSize);

                if (memoryAddress != IntPtr.Zero)
                {
                    var result = new ValueTypeArray(memoryAddress);

                    foreach (var item in array)
                    {
                        if (type == typeof(int))
                        {
                            Marshal.WriteInt32(memoryAddress, (int) item);
                        }
                        else if (type == typeof(uint))
                        {
                            Marshal.WriteInt32(memoryAddress, (int) (uint) item);
                        }
                        else if (type == typeof(short))
                        {
                            Marshal.WriteInt16(memoryAddress, (short) item);
                        }
                        else if (type == typeof(ushort))
                        {
                            Marshal.WriteInt16(memoryAddress, (short) (ushort) item);
                        }
                        else if (type == typeof(long))
                        {
                            Marshal.WriteInt64(memoryAddress, (long) item);
                        }
                        else if (type == typeof(ulong))
                        {
                            Marshal.WriteInt64(memoryAddress, (long) (ulong) item);
                        }
                        else if (type == typeof(byte))
                        {
                            Marshal.WriteByte(memoryAddress, (byte) item);
                        }
                        else if (type == typeof(IntPtr))
                        {
                            Marshal.WriteIntPtr(memoryAddress, (IntPtr) item);
                        }
                        else
                        {
                            Marshal.StructureToPtr(item, memoryAddress, false);
                        }

                        memoryAddress += typeSize;
                    }

                    return result;
                }
            }

            return Null;
        }

        public bool Equals(ValueTypeArray other)
        {
            return MemoryAddress.Equals(other.MemoryAddress);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValueTypeArray array && Equals(array);
        }

        public override int GetHashCode()
        {
            return MemoryAddress.GetHashCode();
        }

        public static bool operator ==(ValueTypeArray left, ValueTypeArray right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueTypeArray left, ValueTypeArray right)
        {
            return !left.Equals(right);
        }

        public static ValueTypeArray FromArray<T>(T[] array) where T : struct
        {
            return FromArray(array.Cast<object>());
        }

        public T[] ToArray<T>(int count) where T : struct
        {
            return ToArray<T>(count, typeof(T));
        }

        public T[] ToArray<T>(int count, Type type)
        {
            return ToArray<T>(0, count, type);
        }

        public T[] ToArray<T>(int start, int count) where T : struct
        {
            return ToArray<T>(start, count, typeof(T)).ToArray();
        }

        public T[] ToArray<T>(int start, int count, Type type)
        {
            if (IsNull)
            {
                return null;
            }

            return AsEnumerable<T>(start, count, type).ToArray();
        }

        public IEnumerable<T> AsEnumerable<T>(int count) where T : struct
        {
            return AsEnumerable<T>(count, typeof(T));
        }


        public IEnumerable<T> AsEnumerable<T>(int count, Type type)
        {
            return AsEnumerable<T>(0, count, type);
        }

        public IEnumerable<T> AsEnumerable<T>(int start, int count) where T : struct
        {
            return AsEnumerable<T>(start, count, typeof(T));
        }

        // ReSharper disable once ExcessiveIndentation
        // ReSharper disable once MethodTooLong
        public IEnumerable<T> AsEnumerable<T>(int start, int count, Type type)
        {
            if (!IsNull)
            {
                if (!type.IsValueType)
                {
                    throw new ArgumentException("Only Value Types are acceptable.", nameof(type));
                }

                var typeSize = Marshal.SizeOf(type);
                var address = MemoryAddress + start * typeSize;

                for (var i = 0; i < count; i++)
                {
                    if (type == typeof(int))
                    {
                        yield return (T) (object) Marshal.ReadInt32(address);
                    }
                    else if (type == typeof(uint))
                    {
                        yield return (T) (object) (uint) Marshal.ReadInt32(address);
                    }
                    else if (type == typeof(short))
                    {
                        yield return (T) (object) Marshal.ReadInt16(address);
                    }
                    else if (type == typeof(ushort))
                    {
                        yield return (T) (object) (ushort) Marshal.ReadInt16(address);
                    }
                    else if (type == typeof(long))
                    {
                        yield return (T) (object) Marshal.ReadInt64(address);
                    }
                    else if (type == typeof(ulong))
                    {
                        yield return (T) (object) (ulong) Marshal.ReadInt64(address);
                    }
                    else if (type == typeof(byte))
                    {
                        yield return (T) (object) Marshal.ReadByte(address);
                    }
                    else if (type == typeof(IntPtr))
                    {
                        yield return (T) (object) Marshal.ReadIntPtr(address);
                    }
                    else
                    {
                        yield return (T) Marshal.PtrToStructure(address, type);
                    }

                    address += typeSize;
                }
            }
        }


        public void Dispose()
        {
            if (!IsNull)
            {
                Marshal.FreeHGlobal(MemoryAddress);
            }
        }
    }
}