﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable

namespace Maths
{
    public static class General
    {
        public const float PI = MathF.PI;
        public const float Deg2Rad = MathF.PI / 180f;
        public const float Rad2Deg = 180f / MathF.PI;
#if UNITY
        public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;
#else
        public const float Epsilon = float.Epsilon;
#endif
        public static readonly float Sqrt2 = MathF.Sqrt(2);

#if UNITY
        /// <inheritdoc cref="Mathf.ClosestPowerOfTwo"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int ClosestPowerOfTwo(int value) => UnityEngine.Mathf.ClosestPowerOfTwo(value);
        /// <inheritdoc cref="Mathf.IsPowerOfTwo"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPowerOfTwo(int value) => UnityEngine.Mathf.IsPowerOfTwo(value);
        /// <inheritdoc cref="Mathf.NextPowerOfTwo"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int NextPowerOfTwo(int value) => UnityEngine.Mathf.NextPowerOfTwo(value);
        /// <inheritdoc cref="Mathf.GammaToLinearSpace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float GammaToLinearSpace(float value) => UnityEngine.Mathf.GammaToLinearSpace(value);
        /// <inheritdoc cref="Mathf.LinearToGammaSpace"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float LinearToGammaSpace(float value) => UnityEngine.Mathf.LinearToGammaSpace(value);
        /// <inheritdoc cref="Mathf.CorrelatedColorTemperatureToRGB"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UnityEngine.Color CorrelatedColorTemperatureToRGB(float kelvin) => UnityEngine.Mathf.CorrelatedColorTemperatureToRGB(kelvin);
        /// <inheritdoc cref="Mathf.FloatToHalf"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort FloatToHalf(float val) => UnityEngine.Mathf.FloatToHalf(val);
        /// <inheritdoc cref="Mathf.HalfToFloat"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float HalfToFloat(ushort val) => UnityEngine.Mathf.FloatToHalf(val);
        /// <inheritdoc cref="Mathf.PerlinNoise"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float PerlinNoise(float x, float y) => UnityEngine.Mathf.PerlinNoise(x, y);
#endif

        public static float Min(params float[] values)
        {
            int n = values.Length;
            if (n == 0) return 0f;

            float result = values[0];
            for (int i = 1; i < n; i++)
            {
                if (values[i] < result)
                {
                    result = values[i];
                }
            }

            return result;
        }

        public static int Min(params int[] values)
        {
            int n = values.Length;
            if (n == 0) return 0;

            int result = values[0];
            for (int i = 1; i < n; i++)
            {
                if (values[i] < result)
                {
                    result = values[i];
                }
            }

            return result;
        }

        public static float Max(params float[] values)
        {
            int n = values.Length;
            if (n == 0) return 0f;

            float result = values[0];
            for (int i = 1; i < n; i++)
            {
                if (values[i] > result)
                {
                    result = values[i];
                }
            }

            return result;
        }

        public static int Max(params int[] values)
        {
            int n = values.Length;
            if (n == 0) return 0;

            int result = values[0];
            for (int i = 1; i < n; i++)
            {
                if (values[i] > result)
                {
                    result = values[i];
                }
            }

            return result;
        }

        public static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }

        /// <summary>
        /// Linearly interpolates between a and b by t.
        /// </summary>
        public static float Lerp(float a, float b, float t) => a + ((b - a) * Math.Clamp(t, 0f, 1f));

        /// <summary>
        /// Linearly interpolates between a and b by t with no limit to t.
        /// </summary>
        public static float LerpUnclamped(float a, float b, float t) => a + ((b - a) * t);

        /// <summary>
        /// Same as Lerp but makes sure the values interpolate correctly when they wrap around
        /// 360 degrees.
        /// </summary>
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if (num > 180f)
            { num -= 360f; }

            return a + (num * Math.Clamp(t, 0f, 1f));
        }

        /// <summary>
        /// Moves a value current towards target.
        /// </summary>
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (MathF.Abs(target - current) <= maxDelta)
            { return target; }

            return current + (MathF.Sign(target - current) * maxDelta);
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
            return MoveTowards(current, target, maxDelta);
        }

        /// <summary>
        /// Interpolates between min and max with smoothing at the limits.
        /// </summary>
        public static float SmoothStep(float from, float to, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            t = (-2f * t * t * t) + (3f * t * t);
            return (to * t) + (from * (1f - t));
        }

        public static float Gamma(float value, float absMax, float gamma)
        {
            bool flag = value < 0f;
            float num = MathF.Abs(value);
            if (num > absMax)
            { return flag ? (0f - num) : num; }

            float num2 = MathF.Pow(num / absMax, gamma) * absMax;
            return flag ? (0f - num2) : num2;
        }

        /// <summary>
        /// Compares two floating point values and returns true if they are similar.
        /// </summary>
        public static bool Approximately(float a, float b)
        {
            return MathF.Abs(b - a) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), Epsilon * 8f);
        }

#if UNITY

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, UnityEngine.Time.deltaTime);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, float.PositiveInfinity, UnityEngine.Time.deltaTime);
        }

#endif

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = MathF.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + (0.48f * num2 * num2) + (0.235f * num2 * num2 * num2));
            float value = current - target;
            float num4 = target;
            float num5 = maxSpeed * smoothTime;
            value = Math.Clamp(value, 0f - num5, num5);
            target = current - value;
            float num6 = (currentVelocity + (num * value)) * deltaTime;
            currentVelocity = (currentVelocity - (num * num6)) * num3;
            float num7 = target + ((value + num6) * num3);
            if ((num4 - current > 0f) == (num7 > num4))
            {
                num7 = num4;
                currentVelocity = (num7 - num4) / deltaTime;
            }

            return num7;
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
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Loops the value <paramref name="t"/>, so that it is never larger than <paramref name="length"/> and never smaller than 0.
        /// </summary>
        public static float Repeat(float t, float length)
            => Math.Clamp(t - (MathF.Floor(t / length) * length), 0f, length);

        /// <summary>
        /// Returns a value that will increment and decrement between the value
        /// 0 and <paramref name="length"/>.
        /// </summary>
        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - MathF.Abs(t - length);
        }

        /// <summary>
        /// Determines where a value lies between two points.
        /// </summary>
        /// <param name="a">
        /// The start of the range.
        /// </param>
        /// <param name="b">
        /// The end of the range.
        /// </param>
        /// <param name="value">The point within the range you want to calculate.</param>
        /// <returns>
        /// A value between zero and one, representing where the <paramref name="value"/> parameter falls
        /// within the range defined by <paramref name="a"/> and <paramref name="b"/>.
        /// </returns>
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            { return Math.Clamp((value - a) / (b - a), 0f, 1f); }
            return 0f;
        }

        /// <summary>
        /// Calculates the shortest difference between two given angles given in degrees.
        /// </summary>
        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f)
            { num -= 360f; }
            return num;
        }

        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num = p2.X - p1.X;
            float num2 = p2.Y - p1.Y;
            float num3 = p4.X - p3.X;
            float num4 = p4.Y - p3.Y;
            float num5 = (num * num4) - (num2 * num3);
            if (num5 == 0f)
            { return false; }

            float num6 = p3.X - p1.X;
            float num7 = p3.Y - p1.Y;
            float num8 = ((num6 * num4) - (num7 * num3)) / num5;
            result.X = p1.X + (num8 * num);
            result.Y = p1.Y + (num8 * num2);
            return true;
        }

        public static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
        {
            float num = p2.X - p1.X;
            float num2 = p2.Y - p1.Y;
            float num3 = p4.X - p3.X;
            float num4 = p4.Y - p3.Y;
            float num5 = (num * num4) - (num2 * num3);
            if (num5 == 0f)
            { return false; }

            float num6 = p3.X - p1.X;
            float num7 = p3.Y - p1.Y;
            float num8 = ((num6 * num4) - (num7 * num3)) / num5;
            if (num8 < 0f || num8 > 1f)
            { return false; }

            float num9 = ((num6 * num2) - (num7 * num)) / num5;
            if (num9 < 0f || num9 > 1f)
            { return false; }

            result.X = p1.X + (num8 * num);
            result.Y = p1.Y + (num8 * num2);
            return true;
        }

        public static long RandomToLong(System.Random r)
        {
            byte[] array = new byte[8];
            r.NextBytes(array);
            return (long)(BitConverter.ToUInt64(array, 0) & 0x7FFFFFFFFFFFFFFFL);
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

        public static float QuadraticEquation(float a, float b, float c, float sign)
        {
            float discriminant = (b * b) - (4 * a * c);
            return (-b + (sign * MathF.Sqrt(discriminant))) / (2 * a);
        }
        public static (float, float) QuadraticEquation(float a, float b, float c)
        {
            float discriminant = (b * b) - (4 * a * c);
            float dSrt = MathF.Sqrt(discriminant);
            float x1 = (-b + dSrt) / (2 * a);
            float x2 = (-b - dSrt) / (2 * a);

            return (x1, x2);
        }
        public static float Sum(params float[] values)
        {
            float sum = 0f;
            for (int i = 0; i < values.Length; i++)
            { sum += values[i]; }
            return sum;
        }

        public static float Average(params float[] values) => Sum(values) / values.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Average(float a, float b) => (a + b) / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Difference(float a, float b) => MathF.Abs(a - b);
        public static Vector2 Difference(Vector2 a, Vector2 b) => new(
            Difference(a.X, b.X),
            Difference(a.Y, b.Y)
        );
        public static Vector3 Difference(Vector3 a, Vector3 b) => new(
            Difference(a.X, b.X),
            Difference(a.Y, b.Y),
            Difference(a.Z, b.Z)
        );

        public static Vector3 Mult(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public struct Circle
        {
            public Vector2 Center;
            public float Radius;

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            public override readonly string ToString()
                => $"Circle{{ Center: ({Center.X}, {Center.Y}) radius: {Radius} }}";

            public readonly Vector2 GetPoint(float rad)
            {
                float x = (Radius * MathF.Cos(rad)) + Center.X;
                float y = (Radius * MathF.Sin(rad)) + Center.Y;
                return new Vector2(x, y);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Vector2 GetPointAfterTime(float speed, float time, float angleOffsetRad)
                => GetPoint(GetAngle(speed, time) + angleOffsetRad);

            public readonly float GetAngle(Vector2 pointOnCircle)
                => MathF.Atan2(pointOnCircle.Y - Center.Y, pointOnCircle.X - Center.Y);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly float GetAngle(float speed, float time)
                => GetAngle(speed * time);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly float GetAngle(float distance)
                => distance / Radius;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Circumference(float radius)
                => MathF.PI * 2 * radius;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly float Circumference()
                => MathF.PI * 2 * Radius;

            public static Vector2[] GenerateEquadistancePoints(int n, float radius)
            {
                List<Vector2> points = new();

                for (int i = 0; i < n; i++)
                {
                    float k = i + .5f;
                    float r = MathF.Sqrt(k / n);
                    float theta = MathF.PI * (1 + MathF.Sqrt(5)) * k;
                    float x = r * MathF.Cos(theta) * radius;
                    float y = r * MathF.Sin(theta) * radius;
                    points.Add(new Vector2(x, y));
                }

                return points.ToArray();
            }
        }

        public static float IsStraightLine(Vector2 positionA, Vector2 positionB, Vector2 positionC)
            => ((positionA.X * (positionB.Y - positionC.Y)) + (positionB.X * (positionC.Y - positionA.Y)) + (positionC.X * (positionA.Y - positionB.Y))) / 2;

        public static Circle FindCircle(Vector2 positionA, Vector2 positionB, Vector2 positionC)
            => FindCircle(positionA.X, positionA.Y, positionB.X, positionB.Y, positionC.X, positionC.Y);
        public static Circle FindCircle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float x12 = x1 - x2;
            float x13 = x1 - x3;

            float y12 = y1 - y2;
            float y13 = y1 - y3;

            float y31 = y3 - y1;
            float y21 = y2 - y1;

            float x31 = x3 - x1;
            float x21 = x2 - x1;

            float sx13 = MathF.Pow(x1, 2) - MathF.Pow(x3, 2);
            float sy13 = MathF.Pow(y1, 2) - MathF.Pow(y3, 2);
            float sx21 = MathF.Pow(x2, 2) - MathF.Pow(x1, 2);
            float sy21 = MathF.Pow(y2, 2) - MathF.Pow(y1, 2);

            float f = ((sx13 * x12) +
                        (sy13 * x12) +
                        (sx21 * x13) +
                        (sy21 * x13))
                    / (2 * ((y31 * x12) - (y21 * x13)));
            float g = ((sx13 * y12) +
                        (sy13 * y12) +
                        (sx21 * y13) +
                        (sy21 * y13))
                    / (2 * ((x31 * y12) - (x21 * y13)));

            float c = -MathF.Pow(x1, 2) - MathF.Pow(y1, 2) - (2 * g * x1) - (2 * f * y1);
            float h = g * -1;
            float k = f * -1;
            float sqr_of_r = (h * h) + (k * k) - c;

            float r = (sqr_of_r < 0) ? 0f : MathF.Sqrt(sqr_of_r);

            return new Circle(new Vector2(h, k), r);
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

        public static Vector3 LengthDir(Vector3 center, float angle, float distance)
        {
            float x = distance * MathF.Cos((90 + angle) * Deg2Rad);
            float y = distance * MathF.Sin((90 + angle) * Deg2Rad);
            Vector3 newPosition = center;
            newPosition.X += x;
            newPosition.Y += y;
            return newPosition;
        }

        struct PointGroup
        {
            public int GroupID;
            public Vector2 Point1;
            public bool IsGrouped;
        }

        static PointGroup[] GeneratePointGroups(Vector2[] points)
        {
            PointGroup[] groups = new PointGroup[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                groups[i] = new PointGroup() { GroupID = i, IsGrouped = false, Point1 = points[i] };
            }
            return groups;
        }

        static Vector2[][] GetGroupsFromGroups(PointGroup[] pointGroups)
        {
            List<List<Vector2>> vector2s = new();
            Dictionary<int, int> groupIdToIndex = new();
            for (int i = 0; i < pointGroups.Length; i++)
            {
                if (groupIdToIndex.TryGetValue(pointGroups[i].GroupID, out int groupIndex))
                {
                    vector2s[groupIndex].Add(pointGroups[i].Point1);
                }
                else
                {
                    vector2s.Add(new List<Vector2>());
                    groupIdToIndex.Add(pointGroups[i].GroupID, vector2s.Count - 1);
                }
            }
            List<Vector2[]> vector2s1 = new();
            foreach (List<Vector2> item in vector2s)
            {
                vector2s1.Add(item.ToArray());
            }
            return vector2s1.ToArray();
        }

        public static Vector2[][] GroupPoints(Vector2[] points, float tolerance)
        {
            PointGroup[] colls = GeneratePointGroups(points);
            for (int i = 0; i < colls.Length; i++)
            {
                ref PointGroup pg1 = ref colls[i];
                if (!pg1.IsGrouped)
                {
                    for (int j = 0; j < colls.Length; j++)
                    {
                        ref PointGroup pg2 = ref colls[j];
                        if (pg1.Point1.AreEquals(pg2.Point1, tolerance) && !pg2.IsGrouped)
                        {
                            if (pg2.GroupID == j)
                            {
                                pg2.GroupID = pg1.GroupID;
                                pg2.IsGrouped = true;
                            }
                        }
                    }

                    pg1.IsGrouped = true;
                }
            }
            return GetGroupsFromGroups(colls);
        }

        public static (Vector2 BottomLeft, Vector2 TopRight) GetRect(Vector2 a, Vector2 b)
        {
            Vector2 lowerLeft = new(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));
            Vector2 upperRight = new(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));
            return (lowerLeft, upperRight);
        }

#if UNITY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector2 BottomLeft, Vector2 TopRight) GetRect(UnityEngine.Transform a, UnityEngine.Transform b)
            => GetRect(a.position.To().To2(), b.position.To().To2());
#endif

        /// <param name="p1">Angle peak</param>
        public static float CalculateAngle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float numerator = (p2.Y * (p1.X - p3.X)) + (p1.Y * (p3.X - p2.X)) + (p3.Y * (p2.X - p1.X));
            float denominator = ((p2.X - p1.X) * (p1.X - p3.X)) + ((p2.Y - p1.Y) * (p1.Y - p3.Y));
            float ratio = numerator / denominator;

            float angleRad = MathF.Atan(ratio);
            float angleDeg = (angleRad * 180) / MathF.PI;

            if (angleDeg < 0)
            { angleDeg = 180f + angleDeg; }

            return angleDeg;
        }

        public static float MapToRange(float outputStart, float outputEnd, float percent)
        {
            /* Note, "slope" below is a constant for given numbers, so if you are calculating
               a lot of output values, it makes sense to calculate it once.  It also makes
               understanding the Code easier */
            float slope = outputEnd - outputStart;
            return outputStart + (slope * percent);
        }

        public static float MapToRange(float outputStart, float outputEnd, float inputStart, float inputEnd, float input)
        {
            /* Note, "slope" below is a constant for given numbers, so if you are calculating
               a lot of output values, it makes sense to calculate it once.  It also makes
               understanding the Code easier */
            float slope = (outputEnd - outputStart) / (inputEnd - inputStart);
            return outputStart + (slope * (input - inputStart));
        }

#if UNITY

        public static (Vector3 Turret, Vector3 Barrel) TurretAim(Vector3 targetPosition, UnityEngine.Transform turret, UnityEngine.Transform barrel)
        {
            float targetPlaneAngle = Vector3AngleOnPlane(targetPosition - turret.position.To(), -turret.up.To(), turret.forward.To());
            Vector3 turretRotation = new(0f, targetPlaneAngle, 0f);

            float barrelAngle = UnityEngine.Vector3.Angle(targetPosition.To(), barrel.up);
            Vector3 barrelRotation = new(-barrelAngle + 90f, 0f, 0f);

            return (turretRotation, barrelRotation);
        }

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

        public static Triangle3[]? GetTriangles(UnityEngine.Mesh mesh)
        {
            if (!mesh.isReadable) return null;

            UnityEngine.Vector3[] meshVertices = mesh.vertices;
            int[] meshTriangles = mesh.triangles;

            int triCount = meshTriangles.Length / 3;
            Triangle3[] triangles = new Triangle3[triCount];

            for (int i = 0; i < triCount; i++)
            {
                triangles[i] = new Triangle3(
                    meshVertices[meshTriangles[(i * 3) + 0]].To(),
                    meshVertices[meshTriangles[(i * 3) + 1]].To(),
                    meshVertices[meshTriangles[(i * 3) + 2]].To());
            }

            return triangles;
        }

        public static float Volume(UnityEngine.Bounds bounds) => bounds.size.sqrMagnitude;

        public static float TotalMeshVolume(UnityEngine.GameObject @object, bool fallbackToBounds = true)
        {
            float volume = 0f;

            UnityEngine.MeshFilter[] meshFilters = @object.GetComponentsInChildren<UnityEngine.MeshFilter>(false);

            for (int i = 0; i < meshFilters.Length; i++)
            {
                float meshVolume = MeshVolume(meshFilters[i].mesh, fallbackToBounds);
                volume += meshVolume;
            }

            return volume;
        }

        public static float Volume(UnityEngine.MeshFilter? meshFilter, UnityEngine.Collider? collider)
        {
            if (meshFilter != null)
            {
                Triangle3[]? triangles = GetTriangles(meshFilter.mesh);
                if (triangles != null)
                { return Volume(triangles); }
            }

            if (collider != null)
            { return General.Volume(collider.bounds); }

            return default;
        }

        public static float MeshVolume(UnityEngine.MeshFilter mesh, bool fallbackToBounds = true)
            => MeshVolume(mesh.mesh, fallbackToBounds);

        public static float MeshVolume(UnityEngine.Mesh mesh, bool fallbackToBounds = true)
        {
            if (!mesh.isReadable)
            {
                if (!fallbackToBounds)
                { return default; }

                return Volume(mesh.bounds) * .75f;
            }
            Triangle3[]? triangles = GetTriangles(mesh);
            if (triangles == null) return default;
            return Volume(triangles);
        }

#endif

        public static float Volume(Triangle3[] triangles)
        {
            float volumeSum = default;

            for (int i = 0; i < triangles.Length; i++)
            { volumeSum += SignedVolumeOfTriangle(triangles[i]); }

            return MathF.Abs(volumeSum);
        }

        static float SignedVolumeOfTriangle(Triangle3 triangle)
        {
            float v321 = triangle.C.X * triangle.B.Y * triangle.A.Z;
            float v231 = triangle.B.X * triangle.C.Y * triangle.A.Z;
            float v312 = triangle.C.X * triangle.A.Y * triangle.B.Z;
            float v132 = triangle.A.X * triangle.C.Y * triangle.B.Z;
            float v213 = triangle.B.X * triangle.A.Y * triangle.C.Z;
            float v123 = triangle.A.X * triangle.B.Y * triangle.C.Z;
            return (1f / 6f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = a.X - b.X;
            float y = a.Y - b.Y;
            float z = a.Z - b.Z;
            return MathF.Sqrt((x * x) + (y * y) + (z * z));
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            float x = a.X - b.X;
            float y = a.Y - b.Y;
            return MathF.Sqrt((x * x) + (y * y));
        }
    }
}
