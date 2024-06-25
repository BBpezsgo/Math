using System;
using System.Numerics;

namespace Maths
{
    public static class Vector3Utils
    {
        public static Vector3 RotX(this Vector3 self, float angle) => new(
            0f,
            (self.Y * MathF.Cos(angle)) - (self.Z * MathF.Sin(angle)),
            (self.Y * MathF.Sin(angle)) + (self.Z * MathF.Cos(angle))
        );

        public static Vector3 RotY(this Vector3 self, float angle) => new(
            (self.X * MathF.Cos(angle)) - (self.Z * MathF.Sin(angle)),
            0f,
            (self.X * MathF.Sin(angle)) + (self.Z * MathF.Cos(angle))
        );

        public static Vector3 RotZ(this Vector3 self, float angle) => new(
            (self.X * MathF.Cos(angle)) - (self.Y * MathF.Sin(angle)),
            (self.X * MathF.Sin(angle)) + (self.Y * MathF.Cos(angle)),
            0f
        );

        public static Vector3 Cross(this Vector3 a, Vector3 b) => new(
            (a.Y * b.Z) - (a.Z * b.Y),
            (a.Z * b.X) - (a.X * b.Z),
            (a.X * b.Y) - (a.Y * b.X)
        );

        public static Vector3 Reflect(Vector3 incoming, Vector3 normal)
            => incoming - (normal * (2f * Dot(incoming, normal)));

        public static Vector3 Random() => new(
            (Maths.Random.Float() - 0.5f) * 2f,
            (Maths.Random.Float() - 0.5f) * 2f,
            (Maths.Random.Float() - 0.5f) * 2f);

        public static Vector3 Random(float multiplier) => new(
            (Maths.Random.Float() - 0.5f) * (multiplier * 2f),
            (Maths.Random.Float() - 0.5f) * (multiplier * 2f),
            (Maths.Random.Float() - 0.5f) * (multiplier * 2f));

        public static Vector3 FromTo(Point4 from, Point4 to) => to - from;

        public static float Dot(this Vector3 a, Vector3 b) => (b.X * a.X) + (b.Y * a.Y) + (b.Z * a.Z);

        // public static float Dot(this Vector3 a, Point4 b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

        public static Vector3 Abs(Vector3 v) => new(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));

        public static Vector3 Max(Vector3 a, Vector3 b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public static Vector3 Normalized(this Vector3 v) => Vector3.Normalize(v);
    }
}
