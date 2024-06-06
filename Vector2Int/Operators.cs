using System.Numerics;

namespace Maths;

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

    public static implicit operator Vector2Int(Win32.Point v) => new(v.X, v.Y);
    public static implicit operator Vector2Int(Win32.Size v) => new(v.Width, v.Height);
    public static implicit operator Vector2Int(Win32.Console.SmallSize v) => new(v.Width, v.Height);
    public static implicit operator Vector2Int(Win32.Console.Coord v) => new(v.X, v.Y);

    public static implicit operator Win32.Point(Vector2Int v) => new(v.X, v.Y);
    public static explicit operator Win32.Console.Coord(Vector2Int v) => new(v.X, v.Y);
    public static implicit operator Vector2(Vector2Int v) => new(v.X, v.Y);
}
