using System;
using System.Numerics;

#nullable enable

namespace Maths
{
    public struct RangeF :
        IEquatable<RangeF>,
        IEquatable<float>,
        IEqualityOperators<RangeF, RangeF, bool>,
        IEqualityOperators<RangeF, float, bool>
    {
        public float Start;
        public float End;

        public readonly float Length => End - Start;

        public RangeF(float a, float b)
        {
            Start = a;
            End = b;
        }
        public RangeF(float v)
        {
            Start = v;
            End = v;
        }

        public readonly float Sample(float v)
        {
            if (Start == End) return Start;
            v = Math.Clamp(v, 0f, 1f);
            return (Start * (1f - v)) + (End * v);
        }
        public readonly float Random()
        {
            if (Start == End) return Start;
            else return Sample(Maths.Random.Float());
        }

        public readonly RangeInt Round() => new((int)MathF.Round(Start), (int)MathF.Round(Start));
        public readonly RangeInt Floor() => new((int)MathF.Floor(Start), (int)MathF.Floor(Start));
        public readonly RangeInt Ceiling() => new((int)MathF.Ceiling(Start), (int)MathF.Ceiling(Start));

        public override readonly bool Equals(object? obj) => obj is RangeF other && Equals(other);
        public readonly bool Equals(float other) => Start == other && End == other;
        public readonly bool Equals(RangeF other) => Start == other.Start && End == other.End;

        public override readonly int GetHashCode() => HashCode.Combine(Start, End);
        public override readonly string ToString() => $"({Start:0.00} -> {End:0.00})";

        public static bool operator ==(RangeF left, RangeF right) => left.Equals(right);
        public static bool operator !=(RangeF left, RangeF right) => !left.Equals(right);

        public static bool operator ==(RangeF left, float right) => left.Equals(right);
        public static bool operator !=(RangeF left, float right) => !left.Equals(right);

        public static implicit operator RangeF(RangeInt v) => new(v.Start, v.End);
        public static implicit operator RangeF(ValueTuple<int, int> v) => new(v.Item1, v.Item2);
        public static implicit operator RangeF(ValueTuple<float, float> v) => new(v.Item1, v.Item2);
        public static implicit operator RangeF(float v) => new(v, v);
        public static implicit operator RangeF(Vector2 v) => new(v.X, v.Y);
        public static implicit operator RangeF(Range<float> v) => new(v.Start, v.End);
        public static implicit operator RangeF(MutableRange<float> v) => new(v.Start, v.End);
#if UNITY
        public static implicit operator RangeF(UnityEngine.Vector2 v) => new(v.x, v.y);
#endif

        public static implicit operator ValueTuple<float, float>(RangeF v) => new(v.Start, v.End);
        public static implicit operator Range<float>(RangeF v) => new(v.Start, v.End);
        public static implicit operator MutableRange<float>(RangeF v) => new(v.Start, v.End);
    }
}
