using System;
using System.Diagnostics;

#nullable enable

namespace Maths
{
    public struct RangeInt : IEquatable<RangeInt>
    {
        public int A;
        public int B;

        public readonly int Length => B - A;

        public RangeInt(int a, int b)
        {
            A = a;
            B = b;
        }
        public RangeInt(int v)
        {
            A = v;
            B = v;
        }

        public readonly float SampleFloat(float v)
        {
            if (A == B) return A;
            v = Math.Clamp(v, 0, 1);
            return (A * (1f - v)) + (B * v);
        }
        public readonly int Sample(float v)
        {
            if (A == B) return A;
            v = Math.Clamp(v, 0, 1);
            return (int)MathF.Round((A * (1f - v)) + (B * v));
        }
        public readonly int Random()
        {
            if (A == B) return A;
            else return Sample(Maths.Random.Float());
        }

        public override readonly bool Equals(object? obj) =>
            obj is RangeInt range &&
            Equals(range);
        public readonly bool Equals(RangeInt other) =>
            A == other.A &&
            B == other.B;
        public override readonly int GetHashCode() => HashCode.Combine(A, B);

        public static bool operator ==(RangeInt left, RangeInt right) => left.Equals(right);
        public static bool operator !=(RangeInt left, RangeInt right) => !(left == right);

        public static implicit operator RangeInt(ValueTuple<int, int> v) => new(v.Item1, v.Item2);
        public static implicit operator RangeInt(int v) => new(v, v);

        public static implicit operator ValueTuple<int, int>(RangeInt v) => new(v.A, v.B);

        public override readonly string ToString() => $"({A} -> {B})";
    }
}
