using System;
using System.Diagnostics;

#nullable enable

namespace Maths
{
    [DebuggerDisplay("{{" + nameof(GetDebuggerDisplay) + "(),nq}}")]
    public struct Gradient : IEquatable<Gradient>
    {
        public ColorF A;
        public ColorF B;

        public Gradient(ColorF a, ColorF b)
        {
            A = a;
            B = b;
        }

        public readonly ColorF Sample(float v) => ((1f - v) * A) + (v * B);

        public override readonly bool Equals(object? obj) => obj is Gradient other && Equals(other);
        public readonly bool Equals(Gradient other) => A.Equals(other.A) && B.Equals(other.B);
        public override readonly int GetHashCode() => HashCode.Combine(A, B);

        public static bool operator ==(Gradient left, Gradient right) => left.A == right.A && left.B == right.B;
        public static bool operator !=(Gradient left, Gradient right) => !(left == right);

        public override readonly string ToString() => $"({A} -> {B})";
        private readonly string GetDebuggerDisplay() => ToString();
    }
}
