using System.Text;

namespace Maths;

public struct Matrix2x2 :
    IMatrix<Matrix2x2>,
    IMultiplyOperators<Matrix2x2, System.Numerics.Vector2, System.Numerics.Vector2>
{
    const int Rows = 2;
    const int Columns = 2;

    public static readonly Matrix2x2 Identity = new(
        1, 0,
        0, 1
    );
    static Matrix2x2 IMultiplicativeIdentity<Matrix2x2, Matrix2x2>.MultiplicativeIdentity => Identity;

    float _00;
    float _01;

    float _10;
    float _11;

    /// <exception cref="IndexOutOfRangeException"/>
    public float this[int r, int c]
    {
        readonly get => r switch
        {
            0 => c switch
            {
                0 => _00,
                1 => _01,
                _ => throw new IndexOutOfRangeException(),
            },
            1 => c switch
            {
                0 => _10,
                1 => _11,
                _ => throw new IndexOutOfRangeException(),
            },
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            _ = r switch
            {
                0 => c switch
                {
                    0 => _00 = value,
                    1 => _01 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                1 => c switch
                {
                    0 => _10 = value,
                    1 => _11 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                _ => throw new IndexOutOfRangeException(),
            };
        }
    }

    public Matrix2x2()
    {
        this._00 = 0f;
        this._01 = 0f;
        this._10 = 0f;
        this._11 = 0f;
    }

    public Matrix2x2(Matrix2x2 v)
    {
        this._00 = v._00;
        this._01 = v._01;
        this._10 = v._10;
        this._11 = v._11;
    }

    public Matrix2x2(
        float _00, float _01,
        float _10, float _11
        )
    {
        this._00 = _00;
        this._01 = _01;
        this._10 = _10;
        this._11 = _11;
    }

    #region Operators

    public static Matrix2x2 operator *(Matrix2x2 a, Matrix2x2 b)
    {
        Matrix2x2 result = new();
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                result[r, c] =
                    (a[r, 0] * b[0, c]) +
                    (a[r, 1] * b[1, c]);
            }
        }
        return result;
    }

    public static Matrix2x2 operator *(Matrix2x2 a, float b) => new(
        a._00 * b, a._01 * b,
        a._10 * b, a._11 * b
    );

    public static System.Numerics.Vector2 operator *(Matrix2x2 a, System.Numerics.Vector2 b) => new(
        (a[0, 0] * b.X) + (a[0, 1] * b.Y),
        (a[1, 0] * b.X) + (a[1, 1] * b.Y)
    );

    public static bool operator ==(Matrix2x2 left, Matrix2x2 right) =>
        FloatUtils.FloatEquality(left._00, right._00) &&
        FloatUtils.FloatEquality(left._01, right._01) &&
        FloatUtils.FloatEquality(left._10, right._10) &&
        FloatUtils.FloatEquality(left._11, right._11);
    public static bool operator !=(Matrix2x2 left, Matrix2x2 right) => !(left == right);

    #endregion

    #region Math

    public readonly Matrix2x2 Transpose() => new(
        _00, _10,
        _01, _11
    );

    /// <exception cref="SingularMatrixException"/>
    public readonly float Determinant()
    {
        float d = DeterminantReciprocal();
        if (FloatUtils.FloatEquality(0f, d)) throw new SingularMatrixException(this);
        return 1f / d;
    }

    public readonly float DeterminantReciprocal()
    {
        float a = this[0, 0];
        float b = this[0, 1];
        float c = this[1, 0];
        float d = this[1, 1];

        return (a * d) - (b * c);
    }

    #endregion

    #region Static Math

    public static Matrix2x2 Translate(float x, float y)
    {
        Matrix2x2 result = new();
        result[0, 2] = x;
        result[1, 2] = y;
        return result;
    }

    public static Matrix2x2 Scale(float x, float y)
    {
        Matrix2x2 result = new();
        result[0, 0] = x;
        result[1, 1] = y;
        return result;
    }

    public static Matrix2x2 Rotate(float z)
    {
        Matrix2x2 result = new();
        result[0, 0] = MathF.Cos(z);
        result[0, 1] = MathF.Sin(z) * -1.0f;
        result[1, 0] = MathF.Sin(z);
        result[1, 1] = MathF.Cos(z);
        return result;
    }

    #endregion

    public override readonly int GetHashCode() => HashCode.Combine(_00, _01, _10, _11);
    public override readonly bool Equals(object? obj) => obj is Matrix2x2 other && Equals(other);
    public readonly bool Equals(Matrix2x2 other) =>
        _00.Equals(other._00) &&
        _01.Equals(other._01) &&
        _10.Equals(other._10) &&
        _11.Equals(other._11);

    public override readonly string ToString()
    {
        StringBuilder result = new();

        for (int r = 0; r < Rows; r++)
        {
            if (r > 0) result.Append('\n');

            for (int c = 0; c < Columns; c++)
            {
                if (c > 0) result.Append(" |");

                float v = MathF.Round(this[r, c] * 100f) / 100f;
                if (v == -0f) v = 0f;

                result.Append(v.ToString("0.00").PadLeft(5, ' '));
            }
        }

        return result.ToString();
    }
}
