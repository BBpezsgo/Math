namespace Maths;

public struct Point4 :
    IEquatable<Point4>,
    IAdditionOperators<Point4, Point4, Point4>,
    IAdditionOperators<Point4, Vector3, Point4>,
    ISubtractionOperators<Point4, Point4, Point4>,
    ISubtractionOperators<Point4, Vector3, Point4>,
    IMultiplyOperators<Point4, Point4, Point4>,
    IMultiplyOperators<Point4, float, Point4>,
    IDivisionOperators<Point4, float, Point4>,
    IEqualityOperators<Point4, Point4, bool>
{
    public float X;
    public float Y;
    public float Z;
    public float W;

    public Point4(float x, float y, float z) : this(x, y, z, 1f) { }
    public Point4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    #region Operators

    public static Point4 operator +(Point4 a, Point4 b) => new(
        a.X + b.X,
        a.Y + b.Y,
        a.Z + b.Z,
        a.W + b.W);

    public static Point4 operator +(Point4 a, Vector3 b) => new(
        a.X + b.X,
        a.Y + b.Y,
        a.Z + b.Z,
        a.W);

    public static Point4 operator -(Point4 a, Point4 b) => new(
        a.X - b.X,
        a.Y - b.Y,
        a.Z - b.Z,
        a.W - b.W);

    public static Point4 operator -(Point4 a, Vector3 b) => new(
        a.X - b.X,
        a.Y - b.Y,
        a.Z - b.Z,
        a.W);

    public static Point4 operator *(Point4 a, float b) => new(
        a.X * b,
        a.Y * b,
        a.Z * b,
        a.W * b);

    public static Point4 operator *(Point4 a, Point4 b) => new(
        a.X * b.X,
        a.Y * b.Y,
        a.Z * b.Z,
        a.W * b.W);

    public static Point4 operator /(Point4 a, float b) => new(
        a.X / b,
        a.Y / b,
        a.Z / b,
        a.W / b);

    public static bool operator ==(Point4 a, Point4 b) =>
        FloatUtils.FloatEquality(a.X, b.X) &&
        FloatUtils.FloatEquality(a.Y, b.Y) &&
        FloatUtils.FloatEquality(a.Z, b.Z) &&
        FloatUtils.FloatEquality(a.W, b.W);

    public static bool operator !=(Point4 a, Point4 b) => !(a == b);

    #endregion

    #region Convertions

    public static implicit operator Point4(System.Numerics.Vector2 v) => new(v.X, v.Y, 0f);
    public static implicit operator Point4(System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);
    public static implicit operator Point4(System.Numerics.Vector4 v) => new(v.X, v.Y, v.Z, v.W);

    public static implicit operator System.Numerics.Vector2(Point4 v) => new(v.X, v.Y);
    public static implicit operator System.Numerics.Vector3(Point4 v) => new(v.X, v.Y, v.Z);
    public static implicit operator System.Numerics.Vector4(Point4 v) => new(v.X, v.Y, v.Z, v.W);

    #endregion

    #region Static Math

    public static float DistanceSqr(Vector3 a, Vector3 b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float dz = b.Z - a.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }

    public static float Distance(Vector3 a, Vector3 b) => MathF.Sqrt(Point4.DistanceSqr(a, b));

    public static float DistanceSqr(Point4 a, Point4 b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float dz = b.Z - a.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }

    public static float Distance(Point4 a, Point4 b) => MathF.Sqrt(Point4.DistanceSqr(a, b));

    #endregion

    public override readonly bool Equals(object? obj) => obj is Point4 other && Equals(other);

    public readonly bool Equals(Point4 other) =>
        X.Equals(other.X) &&
        Y.Equals(other.Y) &&
        Z.Equals(other.Z) &&
        W.Equals(other.W);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    public override readonly string ToString() => $"({X:0.00}, {Y:0.00}, {Z:0.00})";
}
