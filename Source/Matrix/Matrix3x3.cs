using System;
using System.Numerics;
using System.Text;

#nullable enable

namespace Maths
{
    public struct Matrix3x3 :
        ILargeMatrix<Matrix3x3, Matrix2x2>,
        IMultiplyOperators<Matrix3x3, Vector3, Vector3>
    {
        const int Rows = 3;
        const int Columns = 3;

        public static readonly Matrix3x3 Identity = new(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );

#if LANG_11
        static Matrix3x3 IMultiplicativeIdentity<Matrix3x3, Matrix3x3>.MultiplicativeIdentity => Identity;
#endif

        float _00;
        float _01;
        float _02;

        float _10;
        float _11;
        float _12;

        float _20;
        float _21;
        float _22;

        /// <exception cref="IndexOutOfRangeException"/>
        public float this[int r, int c]
        {
            readonly get => r switch
            {
                0 => c switch
                {
                    0 => _00,
                    1 => _01,
                    2 => _02,
                    _ => throw new IndexOutOfRangeException(),
                },
                1 => c switch
                {
                    0 => _10,
                    1 => _11,
                    2 => _12,
                    _ => throw new IndexOutOfRangeException(),
                },
                2 => c switch
                {
                    0 => _20,
                    1 => _21,
                    2 => _22,
                    _ => throw new IndexOutOfRangeException(),
                },
                _ => throw new IndexOutOfRangeException(),
            };
            set => _ = r switch
            {
                0 => c switch
                {
                    0 => _00 = value,
                    1 => _01 = value,
                    2 => _02 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                1 => c switch
                {
                    0 => _10 = value,
                    1 => _11 = value,
                    2 => _12 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                2 => c switch
                {
                    0 => _20 = value,
                    1 => _21 = value,
                    2 => _22 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                _ => throw new IndexOutOfRangeException(),
            };
        }

#if LANG_10
        public Matrix3x3()
        {
            this._00 = 0f;
            this._01 = 0f;
            this._02 = 0f;
            this._10 = 0f;
            this._11 = 0f;
            this._12 = 0f;
            this._20 = 0f;
            this._21 = 0f;
            this._22 = 0f;
        }
#endif

        public Matrix3x3(Matrix3x3 v)
        {
            _00 = v._00;
            _01 = v._01;
            _02 = v._02;
            _10 = v._10;
            _11 = v._11;
            _12 = v._12;
            _20 = v._20;
            _21 = v._21;
            _22 = v._22;
        }

        public Matrix3x3(
            float _00, float _01, float _02,
            float _10, float _11, float _12,
            float _20, float _21, float _22
            )
        {
            this._00 = _00;
            this._01 = _01;
            this._02 = _02;
            this._10 = _10;
            this._11 = _11;
            this._12 = _12;
            this._20 = _20;
            this._21 = _21;
            this._22 = _22;
        }

        #region Operators

        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            Matrix3x3 result = new();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    result[r, c] =
                        (a[r, 0] * b[0, c]) +
                        (a[r, 1] * b[1, c]) +
                        (a[r, 2] * b[2, c]);
                }
            }
            return result;
        }

        public static Matrix3x3 operator *(Matrix3x3 a, float b) => new(
            a._00 * b, a._01 * b, a._02 * b,
            a._10 * b, a._11 * b, a._12 * b,
            a._20 * b, a._21 * b, a._22 * b
        );

        public static Vector3 operator *(Matrix3x3 a, Vector3 b) => new(
            (a[0, 0] * b.X) + (a[0, 1] * b.Y) + (a[0, 2] * b.Z),
            (a[1, 0] * b.X) + (a[1, 1] * b.Y) + (a[1, 2] * b.Z),
            (a[2, 0] * b.X) + (a[2, 1] * b.Y) + (a[2, 2] * b.Z)
        );

        public static Matrix3x3 operator +(Matrix3x3 a, Matrix3x3 b) => new(
            a._00 + b._00, a._01 + b._01, a._02 + b._02,
            a._10 + b._10, a._11 + b._11, a._12 + b._12,
            a._20 + b._20, a._21 + b._21, a._22 + b._22
        );

        public static Matrix3x3 operator -(Matrix3x3 a, Matrix3x3 b) => new(
            a._00 - b._00, a._01 - b._01, a._02 - b._02,
            a._10 - b._10, a._11 - b._11, a._12 - b._12,
            a._20 - b._20, a._21 - b._21, a._22 - b._22
        );

        public static bool operator ==(Matrix3x3 left, Matrix3x3 right) =>
            FloatUtils.FloatEquality(left._00, right._00) &&
            FloatUtils.FloatEquality(left._01, right._01) &&
            FloatUtils.FloatEquality(left._02, right._02) &&
            FloatUtils.FloatEquality(left._10, right._10) &&
            FloatUtils.FloatEquality(left._11, right._11) &&
            FloatUtils.FloatEquality(left._12, right._12) &&
            FloatUtils.FloatEquality(left._20, right._20) &&
            FloatUtils.FloatEquality(left._21, right._21) &&
            FloatUtils.FloatEquality(left._22, right._22);
        public static bool operator !=(Matrix3x3 left, Matrix3x3 right) => !(left == right);

        #endregion

        #region Math

        public readonly Matrix3x3 Transpose() => new(
            _00, _10, _20,
            _01, _11, _21,
            _02, _12, _22
        );

        public readonly Matrix2x2 Sub(int x, int y)
        {
            Matrix2x2 sub = new();

            int i = 0;
            for (int r = 0; r < Rows; r++)
            {
                int j = 0;
                if (r == x) continue;

                for (int c = 0; c < Columns; c++)
                {
                    if (c == y) continue;

                    sub[i, j] = this[r, c];

                    j++;
                }

                i++;
            }

            return sub;
        }

        public readonly float Minor(int r, int c)
        {
            Matrix2x2 sub = Sub(r, c);
            return sub.DeterminantReciprocal();
        }

        public readonly float Cofactor(int r, int c)
        {
            float result = Minor(r, c);

            if (((r + c) % 2) != 0)
            {
                result *= -1f;
            }

            return result;
        }

        public readonly float Det() =>
            (this[0, 0] * Cofactor(0, 0)) +
            (this[0, 1] * Cofactor(0, 1)) +
            (this[0, 2] * Cofactor(0, 2));

        public readonly Matrix3x3 Inverse()
        {
            Matrix3x3 inverse = new();

            float det = Det();

            if (!FloatUtils.FloatEquality(det, 0f))
            {
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        inverse[r, c] = Cofactor(r, c);
                    }
                }
                inverse.Transpose();
                inverse *= 1f / det;
            }
            return inverse;
        }

        #endregion

        #region Static Math

        public static Matrix3x3 Translate(float x, float y, float z)
        {
            Matrix3x3 result = new();
            result[0, 2] = x;
            result[1, 2] = y;
            result[2, 2] = z;
            return result;
        }

        public static Matrix3x3 Scale(float x, float y, float z)
        {
            Matrix3x3 result = new();
            result[0, 0] = x;
            result[1, 1] = y;
            result[2, 2] = z;
            return result;
        }

        public static Matrix3x3 Rotate(float x, float y, float z)
        {
            Matrix3x3 resultX = RotateX(x);
            Matrix3x3 resultY = RotateY(y);
            Matrix3x3 resultZ = RotateZ(z);
            return resultZ * resultY * resultX;
        }

        public static Matrix3x3 RotateX(float x)
        {
            Matrix3x3 temp = new();
            temp[1, 1] = MathF.Cos(x);
            temp[1, 2] = MathF.Sin(x) * -1.0f;
            temp[2, 1] = MathF.Sin(x);
            temp[2, 2] = MathF.Cos(x);
            return temp;
        }

        public static Matrix3x3 RotateY(float y)
        {
            Matrix3x3 temp = new();
            temp[0, 0] = MathF.Cos(y);
            temp[0, 2] = MathF.Sin(y);
            temp[2, 0] = MathF.Sin(y) * -1.0f;
            temp[2, 2] = MathF.Cos(y);
            return temp;
        }

        public static Matrix3x3 RotateZ(float z)
        {
            Matrix3x3 temp = new();
            temp[0, 0] = MathF.Cos(z);
            temp[0, 1] = MathF.Sin(z) * -1.0f;
            temp[1, 0] = MathF.Sin(z);
            temp[1, 1] = MathF.Cos(z);
            return temp;
        }

        #endregion

        public override readonly int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(_00);
            hash.Add(_01);
            hash.Add(_02);
            hash.Add(_10);
            hash.Add(_11);
            hash.Add(_12);
            hash.Add(_20);
            hash.Add(_21);
            hash.Add(_22);
            return hash.ToHashCode();
        }
        public override readonly bool Equals(object? obj) => obj is Matrix3x3 other && Equals(other);
        public readonly bool Equals(Matrix3x3 other) =>
            _00.Equals(other._00) &&
            _01.Equals(other._01) &&
            _02.Equals(other._02) &&
            _10.Equals(other._10) &&
            _11.Equals(other._11) &&
            _12.Equals(other._12) &&
            _20.Equals(other._20) &&
            _21.Equals(other._21) &&
            _22.Equals(other._22);

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
}
