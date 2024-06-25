using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maths
{
    public static partial class Vector
    {
        public static Vector4 To4(this Vector2 v) => new(v.X, v.Y, 0f, 1f);
        public static Vector4 To4(this System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z, 1f);

        public static System.Numerics.Vector3 To3(this Vector4 v) => new(v.X, v.Y, v.Z);
        public static System.Numerics.Vector3 To3(this Vector2 v) => new(v.X, v.Y, 0f);

        public static Vector2 To2(this Vector4 v) => new(v.X, v.Y);
        public static Vector2 To2(this System.Numerics.Vector3 v) => new(v.X, v.Y);

        public static bool TryParse(string text, out System.Numerics.Vector3 vector3)
        {
            vector3 = default;
            text = text.Trim();
            string[] parts = text.Split(' ');

            if (parts.Length != 3)
            { return false; }

            if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.X))
            { return false; }
            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.Y))
            { return false; }
            if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out vector3.Z))
            { return false; }

            return true;
        }

        public static bool TryParse(string text, out Vector2 vector2)
        {
            vector2 = default;
            text = text.Trim();
            string[] parts = text.Split(' ');

            if (parts.Length != 2)
            { return false; }

            if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out vector2.X))
            { return false; }
            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out vector2.Y))
            { return false; }

            return true;
        }

        public static bool IsOk(this Vector3 v) =>
            v.X != float.PositiveInfinity && v.X != float.NegativeInfinity && v.X != float.NaN &&
            v.Y != float.PositiveInfinity && v.Y != float.NegativeInfinity && v.Y != float.NaN;

        public static Vector3 Clamp(this Vector3 v, Vector3 limit) => new(
            Math.Clamp(v.X, -limit.X, limit.X),
            Math.Clamp(v.Y, -limit.Y, limit.Y),
            Math.Clamp(v.Z, -limit.Z, limit.Z));

        public static Vector3 Clamp01(this Vector3 v) => new(
            Math.Clamp(v.X, 0, 1),
            Math.Clamp(v.Y, 0, 1),
            Math.Clamp(v.Z, 0, 1));

        public static Vector2 Clamp(this Vector2 v, Vector2 limit) => new(
            Math.Clamp(v.X, -limit.X, limit.X),
            Math.Clamp(v.Y, -limit.Y, limit.Y));

        public static Vector2 Clamp01(this Vector2 v) => new(
            Math.Clamp(v.X, 0, 1),
            Math.Clamp(v.Y, 0, 1));

        public static Vector2 To2D(this Vector3 v) => new(v.X, v.Z);
        public static Vector3 To3D(this Vector2 v) => new(v.X, 0f, v.Y);
        public static Vector3 To3D(this Vector2 v, float yRotation) => new(v.X * MathF.Sin(yRotation), v.Y, v.X * MathF.Cos(yRotation));

        public static Vector2Int ToInt(this Vector2 vector2) => new((int)MathF.Round(vector2.X), (int)MathF.Round(vector2.Y));
        public static Vector2 ToFloat(this Vector2Int vector2) => new(vector2.X, vector2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate(this Vector2 v, float degrees) => v.RotateRadians(degrees * Rotation.Deg2Rad);
        public static Vector2 RotateRadians(this Vector2 v, float radians)
        {
            float ca = MathF.Cos(radians);
            float sa = MathF.Sin(radians);
            return new Vector2((ca * v.X) - (sa * v.Y), (sa * v.X) + (ca * v.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Rotate(this Vector3 v, float degrees) => v.RotateRadians(degrees * Rotation.Deg2Rad);
        public static Vector3 RotateRadians(this Vector3 v, float radians)
        {
            float ca = MathF.Cos(radians);
            float sa = MathF.Sin(radians);
            return new Vector3((ca * v.X) - (sa * v.Y), (sa * v.X) + (ca * v.Y), 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flatten(this Vector3 v) => new(v.X, 0f, v.Z);

        public static bool AreEquals(this Vector2 vectorA, Vector2 vectorB, double tolerance)
        {
            float absX = MathF.Pow(vectorB.X - vectorA.X, 2);
            float absY = MathF.Pow(vectorB.Y - vectorA.Y, 2);

            return MathF.Abs(absX + absY) < tolerance;
        }

        public static bool IsUnitVector(this Vector2 vector) =>
            vector.X >= 0 &&
            vector.X <= 1 &&
            vector.Y >= 0 &&
            vector.Y <= 1;

        public static bool IsUnitVector(this Vector3 vector) =>
            vector.X >= 0 &&
            vector.X <= 1 &&
            vector.Y >= 0 &&
            vector.Y <= 1 &&
            vector.Z >= 0 &&
            vector.Z <= 1;

#if UNITY

        public static UnityEngine.Vector4 To(this Vector4 v) => new(v.X, v.Y, v.Z, v.W);
        public static Vector4 To(this UnityEngine.Vector4 v) => new(v.x, v.y, v.z, v.w);

        public static UnityEngine.Vector3 To(this Vector3 v) => new(v.X, v.Y, v.Z);
        public static Vector3 To(this UnityEngine.Vector3 v) => new(v.x, v.y, v.z);

        public static UnityEngine.Vector2 To(this Vector2 v) => new(v.X, v.Y);
        public static Vector2 To(this UnityEngine.Vector2 v) => new(v.x, v.y);

#endif

    }
}
