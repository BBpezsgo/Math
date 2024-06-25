using System;
using System.Numerics;

namespace Maths
{
    public partial struct Vector2Int
    {
        public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2Int operator *(Vector2Int a, Vector2Int b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2Int operator /(Vector2Int a, Vector2Int b) => new(a.X / b.X, a.Y / b.Y);

        public static Vector2Int operator *(Vector2Int a, int b) => new(a.X * b, a.Y * b);
        public static Vector2Int operator /(Vector2Int a, int b) => new(a.X / b, a.Y / b);

        public static Vector2 operator *(Vector2Int a, float b) => new(a.X * b, a.Y * b);
        public static Vector2 operator /(Vector2Int a, float b) => new(a.X / b, a.Y / b);

        public static Vector2Int operator *(int a, Vector2Int b) => new(a * b.X, a * b.Y);
        public static Vector2 operator *(float a, Vector2Int b) => new(a * b.X, a * b.Y);

        public static implicit operator Vector2(Vector2Int v) => new(v.X, v.Y);

        public static implicit operator Vector2Int(ValueTuple<int, int> v) => new(v.Item1, v.Item2);
        public static implicit operator Vector2Int(System.Drawing.Point v) => new(v.X, v.Y);

        public static explicit operator Vector2Int(Vector2 v) => new((int)v.X, (int)v.Y);
        public static explicit operator Vector2Int(System.Drawing.PointF v) => new((int)v.X, (int)v.Y);
    }
}
