using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY

namespace Maths
{
    public static class ImmutableArray
    {
        internal static ImmutableArray<T> Create<T>(Span<T> values) => new(values.ToArray());
        internal static ImmutableArray<T> Create<T>(ReadOnlySpan<T> values) => new(values.ToArray());
        internal static ImmutableArray<T> Create<T>(params T[] values) => new(values);
    }

    public readonly struct ImmutableArray<T> : IEnumerable<T>
    {
        readonly T[] _array;

        public T this[int index] => _array[index];
        public int Length => _array.Length;

        public ImmutableArray(T[] array) => _array = array;

        public void CopyTo(byte[] perm, int v) => throw new NotImplementedException();
        public ReadOnlySpan<T> AsSpan() => _array.AsSpan();
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}

#endif
