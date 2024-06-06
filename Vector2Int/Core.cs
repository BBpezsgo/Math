﻿using System.Diagnostics;
using System.Globalization;

namespace Maths;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial struct Vector2Int : IEquatable<Vector2Int>
{
    public int X;
    public int Y;

    public Vector2Int(int v)
    {
        X = v;
        Y = v;
    }
    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);
    public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);

    public override readonly bool Equals(object? obj) =>
        obj is Vector2Int vector &&
        Equals(vector);
    public readonly bool Equals(Vector2Int other) =>
        X == other.X &&
        Y == other.Y;
    public override readonly int GetHashCode()
        => HashCode.Combine(X, Y);

    public override readonly string ToString()
        => $"({X.ToString("0.00", CultureInfo.InvariantCulture)}, {Y.ToString("0.00", CultureInfo.InvariantCulture)})";
    readonly string GetDebuggerDisplay() => ToString();

    public static Vector2Int Zero => new(0);
    public static Vector2Int One => new(1);
}
