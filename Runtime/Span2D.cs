#pragma warning disable IDE0032 // Use auto property
#pragma warning disable RCS1085 // Use auto-implemented property
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Maths
{
    public readonly ref struct Span2D<T>
    {
        readonly Span<T> _span;
        readonly int _width;

        public int Width => _width;
        public int Height => _span.Length / _width;
        public bool IsEmpty => _span.IsEmpty;

        public ref T this[int i] => ref _span[i];
        public ref T this[int x, int y] => ref _span[x + (y * _width)];

        public Span2D(Span<T> span, int width)
        {
            _span = span;
            _width = width;
        }

        public Span<T> AsSpan() => _span;
        public T[] ToArray() => _span.ToArray();
        public void Clear() => _span.Clear();
        public void Fill(T value) => _span.Fill(value);
        public Span<T>.Enumerator GetEnumerator() => _span.GetEnumerator();

        public static implicit operator Span2D<T>(Array2D<T> v) => new(v.AsArray(), v.Width);

        public static bool operator ==(Span2D<T> left, Span2D<T> right) => left._span == right._span && left._width == right._width;
        public static bool operator !=(Span2D<T> left, Span2D<T> right) => !(left == right);

        [Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
        [DoesNotReturn] public override bool Equals(object? obj) => throw new NotImplementedException();
        [Obsolete("GetHashCode() on Span will always throw an exception.")]
        [DoesNotReturn] public override int GetHashCode() => throw new NotImplementedException();
    }
}
