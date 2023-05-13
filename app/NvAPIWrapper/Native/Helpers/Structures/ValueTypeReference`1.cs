using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Helpers.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueTypeReference<T> : IDisposable, IHandle, IEquatable<ValueTypeReference<T>> where T : struct
    {
        private ValueTypeReference underlyingReference;

        public IntPtr MemoryAddress
        {
            get => underlyingReference.MemoryAddress;
        }

        public static ValueTypeReference<T> Null
        {
            get => new ValueTypeReference<T>();
        }

        public bool IsNull
        {
            get => underlyingReference.IsNull;
        }

        public ValueTypeReference(IntPtr memoryAddress)
        {
            underlyingReference = new ValueTypeReference(memoryAddress);
        }

        private ValueTypeReference(ValueTypeReference underlyingReference)
        {
            this.underlyingReference = underlyingReference;
        }

        public static ValueTypeReference<T> FromValueType(T valueType)
        {
            return new ValueTypeReference<T>(ValueTypeReference.FromValueType(valueType));
        }

        public static ValueTypeReference<T> FromValueType(object valueType, Type type)
        {
            return new ValueTypeReference<T>(ValueTypeReference.FromValueType(valueType, type));
        }

        public bool Equals(ValueTypeReference<T> other)
        {
            return underlyingReference.Equals(other.underlyingReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValueTypeReference<T> reference && Equals(reference);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return underlyingReference.GetHashCode();
        }

        public static bool operator ==(ValueTypeReference<T> left, ValueTypeReference<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueTypeReference<T> left, ValueTypeReference<T> right)
        {
            return !left.Equals(right);
        }

        public T ToValueType(Type type)
        {
            return underlyingReference.ToValueType<T>(type);
        }

        public T? ToValueType()
        {
            return underlyingReference.ToValueType<T>();
        }

        public void Dispose()
        {
            if (!IsNull)
            {
                underlyingReference.Dispose();
            }
        }
    }
}