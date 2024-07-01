using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maths
{
    public static class Rotation
    {
        public const float Deg2Byte = (float)byte.MaxValue / 360f;
        public const float Byte2Deg = 360f / (float)byte.MaxValue;

        public const float Rad2Deg = 180f / MathF.PI;
        public const float Deg2Rad = MathF.PI / 180f;

        /// <summary>
        /// Same as Lerp but makes sure the values interpolate correctly when they wrap around
        /// 360 degrees.
        /// </summary>
        public static float LerpAngle(float a, float b, float t)
        {
            float num = General.Repeat(b - a, 360f);
            if (num > 180f)
            { num -= 360f; }

            return a + (num * Math.Clamp(t, 0f, 1f));
        }

        /// <summary>
        /// Same as MoveTowards but makes sure the values interpolate correctly when they
        /// wrap around 360 degrees.
        /// </summary>
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = DeltaAngle(current, target);
            if (0f - maxDelta < num && num < maxDelta)
            { return target; }

            target = current + num;
            return General.MoveTowards(current, target, maxDelta);
        }

#if UNITY

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
        {
            return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, UnityEngine.Time.deltaTime);
        }

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
        {
            return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, float.PositiveInfinity, UnityEngine.Time.deltaTime);
        }

#endif

        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return General.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Calculates the shortest difference between two given angles given in degrees.
        /// </summary>
        public static float DeltaAngle(float current, float target)
        {
            float num = General.Repeat(target - current, 360f);
            if (num > 180f)
            { num -= 360f; }
            return num;
        }

        /// <summary>
        /// Normalizes <paramref name="angle"/> between <c>[-180,180[</c>
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) return angle - 360f;
            else return angle;
        }

        /// <summary>
        /// Normalizes <paramref name="angle"/> between <c>[0,360[</c>
        /// </summary>
        public static float NormalizeAngle360(float angle)
        {
            angle %= 360f;
            if (angle < 0f) return angle + 360f;
            else return angle;
        }

        public static float GetAngle(Vector2 dir)
        {
            dir = Vector2.Normalize(dir);
            return MathF.Atan2(dir.Y, dir.X);
        }

        public static Vector2 RadianToVector2(float radian) => new(MathF.Cos(radian), MathF.Sin(radian));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 DegreeToVector2(float degree) => RadianToVector2(degree * Deg2Rad);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeDegree(float degree) => (degree + 360) % 360;

#if UNITY
        /// <summary>
        /// Source: <see href="https://www.youtube.com/watch?v=bCz7awDbl58"/>
        /// </summary>
        /// <param name="from">
        /// Target position
        /// </param>
        /// <param name="to">
        /// Us
        /// </param>
        /// <param name="toZeroAngle">
        /// Orientation
        /// </param>
        public static float Vector3AngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal, Vector3 toZeroAngle)
            => Vector3AngleOnPlane(from - to, planeNormal, toZeroAngle);

        /// <summary>
        /// Source: <see href="https://www.youtube.com/watch?v=bCz7awDbl58"/>
        /// </summary>
        /// <param name="toZeroAngle">
        /// Orientation
        /// </param>
        public static float Vector3AngleOnPlane(Vector3 relativePosition, Vector3 planeNormal, Vector3 toZeroAngle)
        {
            UnityEngine.Vector3 projectedVector = UnityEngine.Vector3.ProjectOnPlane(relativePosition.To(), planeNormal.To());
            return UnityEngine.Vector3.SignedAngle(projectedVector, toZeroAngle.To(), planeNormal.To());
        }

#endif

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
