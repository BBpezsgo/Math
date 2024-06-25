using System;
using System.Numerics;

namespace Maths
{
    public static class Rotation
    {
        public const float Deg2Byte = (float)byte.MaxValue / 360f;
        public const float Byte2Deg = 360f / (float)byte.MaxValue;

        public const float Rad2Deg = 180f / MathF.PI;
        public const float Deg2Rad = MathF.PI / 180f;

        public static void ClampAngle(ref float deg)
        {
            while (deg < 0)
            { deg += 360f; }
            while (deg >= 360f)
            { deg -= 360f; }
        }

        public static float ClampAngle(float deg)
        {
            while (deg < 0)
            { deg += 360f; }
            while (deg >= 360f)
            { deg -= 360f; }
            return deg;
        }

        public static Vector2 FromDeg(float deg) => Rotation.FromRad((float)(deg * Deg2Rad));
        public static Vector2 FromRad(float rad) => new(MathF.Cos(rad), MathF.Sin(rad));

        public static float ToDeg(Vector2 unitVector) => Rotation.ToRad(unitVector) * Rad2Deg;
        public static float ToRad(Vector2 unitVector) => MathF.Atan2(unitVector.Y, unitVector.X);

        public static Vector2 RotateByDeg(Vector2 direction, float deg)
            => Rotation.FromDeg(Rotation.ToDeg(direction) + deg);
        public static void RotateByDeg(ref Vector2 direction, float deg)
            => direction = Rotation.FromDeg(Rotation.ToDeg(direction) + deg);

        public static Vector2 RotateByRad(Vector2 direction, float rad)
            => Rotation.FromRad(Rotation.ToRad(direction) + rad);
        public static void RotateByRad(ref Vector2 direction, float rad)
            => direction = Rotation.FromRad(Rotation.ToRad(direction) + rad);

#if UNITY

        public static float ToDeg(UnityEngine.Vector2 unitVector) => Rotation.ToRad(unitVector) * Rad2Deg;
        public static float ToRad(UnityEngine.Vector2 unitVector) => MathF.Atan2(unitVector.y, unitVector.x);

        public static UnityEngine.Vector2 RotateByDeg(UnityEngine.Vector2 direction, float deg)
            => Rotation.FromDeg(Rotation.ToDeg(direction) + deg).To();
        public static void RotateByDeg(ref UnityEngine.Vector2 direction, float deg)
            => direction = Rotation.FromDeg(Rotation.ToDeg(direction) + deg).To();

        public static UnityEngine.Vector2 RotateByRad(UnityEngine.Vector2 direction, float rad)
            => Rotation.FromRad(Rotation.ToRad(direction) + rad).To();
        public static void RotateByRad(ref UnityEngine.Vector2 direction, float rad)
            => direction = Rotation.FromRad(Rotation.ToRad(direction) + rad).To();

#endif
    }
}
