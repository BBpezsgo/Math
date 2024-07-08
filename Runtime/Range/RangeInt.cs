using System;
using System.Numerics;

#nullable enable

namespace Maths
{
    public struct RangeInt :
        IEquatable<RangeInt>,
        IEquatable<int>,
        IEqualityOperators<RangeInt, RangeInt, bool>,
        IEqualityOperators<RangeInt, int, bool>
    {
        public int Start;
        public int End;

        public readonly int Length => End - Start;

        public RangeInt(int a, int b)
        {
            Start = a;
            End = b;
        }
        public RangeInt(int v)
        {
            Start = v;
            End = v;
        }

        public readonly float SampleFloat(float v)
        {
            if (Start == End) return Start;
            v = Math.Clamp(v, 0, 1);
            return (Start * (1f - v)) + (End * v);
        }
        public readonly int Sample(float v)
        {
            if (Start == End) return Start;
            v = Math.Clamp(v, 0, 1);
            return (int)MathF.Round((Start * (1f - v)) + (End * v));
        }
        public readonly int Random()
        {
            if (Start == End) return Start;
            else return Sample(Maths.Random.Float());
        }

        public override readonly bool Equals(object? obj) => obj is RangeInt other && Equals(other);
        public readonly bool Equals(int other) => Start == other && End == other;
        public readonly bool Equals(RangeInt other) => Start == other.Start && End == other.End;

        public override readonly int GetHashCode() => HashCode.Combine(Start, End);
        public override readonly string ToString() => $"({Start} -> {End})";

        public static bool operator ==(RangeInt left, RangeInt right) => left.Equals(right);
        public static bool operator !=(RangeInt left, RangeInt right) => !left.Equals(right);

        public static bool operator ==(RangeInt left, int right) => left.Equals(right);
        public static bool operator !=(RangeInt left, int right) => !left.Equals(right);

        public static implicit operator RangeInt(ValueTuple<int, int> v) => new(v.Item1, v.Item2);
        public static implicit operator RangeInt(int v) => new(v, v);
        public static implicit operator RangeInt(Range<int> v) => new(v.Start, v.End);
        public static implicit operator RangeInt(MutableRange<int> v) => new(v.Start, v.End);

        public static implicit operator ValueTuple<int, int>(RangeInt v) => new(v.Start, v.End);
        public static implicit operator Range<int>(RangeInt v) => new(v.Start, v.End);
        public static implicit operator MutableRange<int>(RangeInt v) => new(v.Start, v.End);
    }
}
