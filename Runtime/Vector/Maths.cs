using System;
using System.Numerics;

namespace Maths
{
    public static partial class Vector
    {
        public static System.Numerics.Vector3 IntersectPlane(System.Numerics.Vector3 planePoint, System.Numerics.Vector3 planeNormal, System.Numerics.Vector3 lineStart, System.Numerics.Vector3 lineEnd)
            => Vector.IntersectPlane(planePoint, planeNormal, lineStart, lineEnd, out _);
        public static System.Numerics.Vector3 IntersectPlane(System.Numerics.Vector3 planePoint, System.Numerics.Vector3 planeNormal, System.Numerics.Vector3 lineStart, System.Numerics.Vector3 lineEnd, out float t)
        {
            planeNormal = System.Numerics.Vector3.Normalize(planeNormal);
            float planeD = -System.Numerics.Vector3.Dot(planeNormal, planePoint);
            float ad = System.Numerics.Vector3.Dot(lineStart, planeNormal);
            float bd = System.Numerics.Vector3.Dot(lineEnd, planeNormal);
            t = (-planeD - ad) / (bd - ad);
            System.Numerics.Vector3 lineStartToEnd = lineEnd - lineStart;
            System.Numerics.Vector3 lineToIntersect = lineStartToEnd * t;
            return lineStart + lineToIntersect;
        }

        public static Vector2 MoveTowards(Vector2 a, Vector2 b, float maxDelta)
        {
            Vector2 diff = b - a;
            if (diff.LengthSquared() <= float.Epsilon) return Vector2.Zero;
            maxDelta = Math.Clamp(maxDelta, 0f, diff.Length());
            diff = Vector2.Normalize(diff);
            return diff * maxDelta;
        }

        public static Vector2 LinearLerp(Vector2 a, Vector2 b, float t) => (a * (1f - t)) + (b * t);

        public static System.Numerics.Vector3 LinearLerp(System.Numerics.Vector3 a, System.Numerics.Vector3 b, float t) => (a * (1f - t)) + (b * t);

        public static Vector2Int Round(this Vector2 vector) => new((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));
        public static Vector2Int Floor(this Vector2 vector) => new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));
        public static Vector2Int Ceil(this Vector2 vector) => new((int)MathF.Ceiling(vector.X), (int)MathF.Ceiling(vector.Y));

        public static Vector2Int Round(float x, float y) => new((int)MathF.Round(x), (int)MathF.Round(y));
        public static Vector2Int Floor(float x, float y) => new((int)MathF.Floor(x), (int)MathF.Floor(y));
        public static Vector2Int Ceil(float x, float y) => new((int)MathF.Ceiling(x), (int)MathF.Ceiling(y));
    }
}
