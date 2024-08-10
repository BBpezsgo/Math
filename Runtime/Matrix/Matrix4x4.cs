using System;
using System.Numerics;
using System.Text;

#nullable enable

namespace Maths
{
    public struct Matrix4x4 :
        ILargeMatrix<Matrix4x4, Matrix3x3>,
        IMultiplyOperators<Matrix4x4, Vector3, Vector3>,
        IMultiplyOperators<Matrix4x4, Point4, Point4>,
        IMultiplyOperators<Matrix4x4, Vector4, Vector4>
    {
        const int Rows = 4;
        const int Columns = 4;

        public static readonly Matrix4x4 Identity = new(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

#if LANG_11
        static Matrix4x4 IMultiplicativeIdentity<Matrix4x4, Matrix4x4>.MultiplicativeIdentity => Identity;
#endif

        float _00;
        float _01;
        float _02;
        float _03;

        float _10;
        float _11;
        float _12;
        float _13;

        float _20;
        float _21;
        float _22;
        float _23;

        float _30;
        float _31;
        float _32;
        float _33;

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
                    3 => _03,
                    _ => throw new IndexOutOfRangeException(),
                },
                1 => c switch
                {
                    0 => _10,
                    1 => _11,
                    2 => _12,
                    3 => _13,
                    _ => throw new IndexOutOfRangeException(),
                },
                2 => c switch
                {
                    0 => _20,
                    1 => _21,
                    2 => _22,
                    3 => _23,
                    _ => throw new IndexOutOfRangeException(),
                },
                3 => c switch
                {
                    0 => _30,
                    1 => _31,
                    2 => _32,
                    3 => _33,
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
                    3 => _03 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                1 => c switch
                {
                    0 => _10 = value,
                    1 => _11 = value,
                    2 => _12 = value,
                    3 => _13 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                2 => c switch
                {
                    0 => _20 = value,
                    1 => _21 = value,
                    2 => _22 = value,
                    3 => _23 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                3 => c switch
                {
                    0 => _30 = value,
                    1 => _31 = value,
                    2 => _32 = value,
                    3 => _33 = value,
                    _ => throw new IndexOutOfRangeException(),
                },
                _ => throw new IndexOutOfRangeException(),
            };
        }

#if LANG_10
        public Matrix4x4()
        {
            this._00 = 0f;
            this._01 = 0f;
            this._02 = 0f;
            this._03 = 0f;
            this._10 = 0f;
            this._11 = 0f;
            this._12 = 0f;
            this._13 = 0f;
            this._20 = 0f;
            this._21 = 0f;
            this._22 = 0f;
            this._23 = 0f;
            this._30 = 0f;
            this._31 = 0f;
            this._32 = 0f;
            this._33 = 0f;
        }
#endif

        public Matrix4x4(Matrix4x4 other)
        {
            _00 = other._00;
            _01 = other._01;
            _02 = other._02;
            _03 = other._03;
            _10 = other._10;
            _11 = other._11;
            _12 = other._12;
            _13 = other._13;
            _20 = other._20;
            _21 = other._21;
            _22 = other._22;
            _23 = other._23;
            _30 = other._30;
            _31 = other._31;
            _32 = other._32;
            _33 = other._33;
        }

        public Matrix4x4(
            float _00, float _01, float _02, float _03,
            float _10, float _11, float _12, float _13,
            float _20, float _21, float _22, float _23,
            float _30, float _31, float _32, float _33
        )
        {
            this._00 = _00;
            this._01 = _01;
            this._02 = _02;
            this._03 = _03;
            this._10 = _10;
            this._11 = _11;
            this._12 = _12;
            this._13 = _13;
            this._20 = _20;
            this._21 = _21;
            this._22 = _22;
            this._23 = _23;
            this._30 = _30;
            this._31 = _31;
            this._32 = _32;
            this._33 = _33;
        }

        #region Operators

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    result[r, c] =
                        (a[r, 0] * b[0, c]) +
                        (a[r, 1] * b[1, c]) +
                        (a[r, 2] * b[2, c]) +
                        (a[r, 3] * b[3, c]);
                }
            }
            return result;
        }

        public static Matrix4x4 operator *(Matrix4x4 a, float b) => new(
            a._00 * b, a._01 * b, a._02 * b, a._03 * b,
            a._10 * b, a._11 * b, a._12 * b, a._13 * b,
            a._20 * b, a._21 * b, a._22 * b, a._23 * b,
            a._30 * b, a._31 * b, a._32 * b, a._33 * b
        );

        public static Point4 operator *(Matrix4x4 a, Point4 b) => new(
            (a[0, 0] * b.X) + (a[0, 1] * b.Y) + (a[0, 2] * b.Z) + (a[0, 3] * b.W),
            (a[1, 0] * b.X) + (a[1, 1] * b.Y) + (a[1, 2] * b.Z) + (a[1, 3] * b.W),
            (a[2, 0] * b.X) + (a[2, 1] * b.Y) + (a[2, 2] * b.Z) + (a[2, 3] * b.W),
            (a[3, 0] * b.X) + (a[3, 1] * b.Y) + (a[3, 2] * b.Z) + (a[3, 3] * b.W)
        );

        public static Vector4 operator *(Matrix4x4 a, Vector4 b) => new(
            (a[0, 0] * b.X) + (a[0, 1] * b.Y) + (a[0, 2] * b.Z) + (a[0, 3] * b.W),
            (a[1, 0] * b.X) + (a[1, 1] * b.Y) + (a[1, 2] * b.Z) + (a[1, 3] * b.W),
            (a[2, 0] * b.X) + (a[2, 1] * b.Y) + (a[2, 2] * b.Z) + (a[2, 3] * b.W),
            (a[3, 0] * b.X) + (a[3, 1] * b.Y) + (a[3, 2] * b.Z) + (a[3, 3] * b.W)
        );

        public static Vector3 operator *(Matrix4x4 a, Vector3 b) => new(
            (a[0, 0] * b.X) + (a[0, 1] * b.Y) + (a[0, 2] * b.Z),
            (a[1, 0] * b.X) + (a[1, 1] * b.Y) + (a[1, 2] * b.Z),
            (a[2, 0] * b.X) + (a[2, 1] * b.Y) + (a[2, 2] * b.Z)
        );

        public static bool operator ==(Matrix4x4 left, Matrix4x4 right) =>
            FloatUtils.FloatEquality(left._00, right._00) &&
            FloatUtils.FloatEquality(left._01, right._01) &&
            FloatUtils.FloatEquality(left._02, right._02) &&
            FloatUtils.FloatEquality(left._03, right._03) &&
            FloatUtils.FloatEquality(left._10, right._10) &&
            FloatUtils.FloatEquality(left._11, right._11) &&
            FloatUtils.FloatEquality(left._12, right._12) &&
            FloatUtils.FloatEquality(left._13, right._13) &&
            FloatUtils.FloatEquality(left._20, right._20) &&
            FloatUtils.FloatEquality(left._21, right._21) &&
            FloatUtils.FloatEquality(left._22, right._22) &&
            FloatUtils.FloatEquality(left._23, right._23) &&
            FloatUtils.FloatEquality(left._30, right._30) &&
            FloatUtils.FloatEquality(left._31, right._31) &&
            FloatUtils.FloatEquality(left._32, right._32) &&
            FloatUtils.FloatEquality(left._33, right._33);
        public static bool operator !=(Matrix4x4 left, Matrix4x4 right) => !(left == right);

        #endregion

        #region Convertions

        // public static implicit operator Matrix4x4(Matrix4x4 m) => new(
        //     m._00, m._01, m._02, m._03,
        //     m._10, m._11, m._12, m._13,
        //     m._20, m._21, m._22, m._23,
        //     m._30, m._31, m._32, m._33);

        public static implicit operator System.Numerics.Matrix4x4(Matrix4x4 m) => new(
            m._00, m._10, m._20, m._30,
            m._01, m._11, m._21, m._31,
            m._02, m._12, m._22, m._32,
            m._03, m._13, m._23, m._33);

        // public static implicit operator Matrix4x4(Matrix4x4 m) => new(
        //     m.M11, m.M12, m.M13, m.M14,
        //     m.M21, m.M22, m.M23, m.M24,
        //     m.M31, m.M32, m.M33, m.M34,
        //     m.M41, m.M42, m.M43, m.M44);

        public static implicit operator Matrix4x4(System.Numerics.Matrix4x4 m) => new(
            m.M11, m.M21, m.M31, m.M41,
            m.M12, m.M22, m.M32, m.M42,
            m.M13, m.M23, m.M33, m.M43,
            m.M14, m.M24, m.M34, m.M44);

        #endregion

        #region Math

        public readonly Matrix4x4 Transpose() => new(
            _00, _10, _20, _30,
            _01, _11, _21, _31,
            _02, _12, _22, _32,
            _03, _13, _23, _33
        );

        public readonly Matrix3x3 Sub(int x, int y)
        {
            Matrix3x3 sub = new();

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

        public readonly float Minor(int x, int y)
        {
            Matrix3x3 sub = Sub(x, y);
            return sub.Det();
        }

        public readonly float Cofactor(int r, int c)
        {
            float result = Minor(r, c);

            if ((r + c) % 2 != 0)
            {
                result = -result;
            }

            return result;
        }

        public readonly float Det() =>
            (this[0, 0] * Sub(0, 0).Det()) -
            (this[1, 0] * Sub(1, 0).Det()) +
            (this[2, 0] * Sub(2, 0).Det()) -
            (this[3, 0] * Sub(3, 0).Det());

        public readonly Matrix4x4 Adjugate()
        {
            Matrix4x4 adj = new();

            // Grab all 16 sub matrices of 3x3 and get the
            // determinate of each, then determine if positive or negative value
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    adj[r, c] = Sub(r, c).Det();
                    if (((r + c) % 2) != 0)
                    {
                        adj[r, c] *= -1f;
                    }
                }
            }
            return adj;
        }

        public readonly Matrix4x4 Inverse()
        {
            Matrix4x4 inverse = new();

            float det = Det();

            if (!FloatUtils.FloatEquality(det, 0f))
            {
                inverse = Adjugate().Transpose() * (1f / det);
            }

            return inverse;
        }

        public readonly Matrix4x4 Translate(float x, float y, float z) => this * CreateTranslation(x, y, z);
        public readonly Matrix4x4 Scale(float x, float y, float z) => this * CreateScale(x, y, z);
        public readonly Matrix4x4 Rotate(float x, float y, float z) => this * CreateRotation(x, y, z);
        public readonly Matrix4x4 RotateX(float x) => this * CreateRotationX(x);
        public readonly Matrix4x4 RotateY(float y) => this * CreateRotationY(y);
        public readonly Matrix4x4 RotateZ(float z) => this * CreateRotationZ(z);
        public readonly Matrix4x4 Shear(float xy, float xz, float yx, float yz, float zx, float zy) => this * ShearMatrix(xy, xz, yx, yz, zx, zy);

        #endregion

        #region Static Math

        public static Matrix4x4 CreateTranslation(float x, float y, float z) => new(
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, z,
            0, 0, 0, 1
        );
        public static Matrix4x4 CreateTranslation(Point4 v) => CreateTranslation(v.X, v.Y, v.Z);
        public static Matrix4x4 CreateTranslation(Vector3 v) => CreateTranslation(v.X, v.Y, v.Z);

        public static Matrix4x4 CreateScale(float x, float y, float z) => new(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1
        );
        public static Matrix4x4 CreateScale(Point4 v) => CreateScale(v.X, v.Y, v.Z);
        public static Matrix4x4 CreateScale(Vector3 v) => CreateScale(v.X, v.Y, v.Z);

        public static Matrix4x4 CreateRotation(float x, float y, float z)
        {
            // Order here matters, so be careful!
            Matrix4x4 tempX = CreateRotationX(x);
            Matrix4x4 tempY = CreateRotationY(y);
            Matrix4x4 tempZ = CreateRotationZ(z);
            return tempZ * tempY * tempX;
        }

        public static Matrix4x4 CreateRotationX(float x) => new(
            1, 0, 0, 0,
            0, MathF.Cos(x), -MathF.Sin(x), 0,
            0, MathF.Sin(x), MathF.Cos(x), 0,
            0, 0, 0, 1
        );

        public static Matrix4x4 CreateRotationY(float y) => new(
            MathF.Cos(y), 0, MathF.Sin(y), 0,
            0, 1, 0, 0,
            -MathF.Sin(y), 0, MathF.Cos(y), 0,
            0, 0, 0, 1
        );

        public static Matrix4x4 CreateRotationZ(float z) => new(
            MathF.Cos(z), -MathF.Sin(z), 0, 0,
            MathF.Sin(z), MathF.Cos(z), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        public static Matrix4x4 ShearMatrix(float xy, float xz, float yx, float yz, float zx, float zy) => new(
            1f, xy, xz, 0f,
            yx, 1f, yz, 0f,
            zx, zy, 1f, 0f,
            0f, 0f, 0f, 1f
        );

        #endregion

        public override readonly int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(_00);
            hash.Add(_01);
            hash.Add(_02);
            hash.Add(_03);
            hash.Add(_10);
            hash.Add(_11);
            hash.Add(_12);
            hash.Add(_13);
            hash.Add(_20);
            hash.Add(_21);
            hash.Add(_22);
            hash.Add(_23);
            hash.Add(_30);
            hash.Add(_31);
            hash.Add(_32);
            hash.Add(_33);
            return hash.ToHashCode();
        }

        public override readonly bool Equals(object? obj) => obj is Matrix4x4 other && Equals(other);
        public readonly bool Equals(Matrix4x4 other) =>
            _00.Equals(other._00) &&
            _01.Equals(other._01) &&
            _02.Equals(other._02) &&
            _03.Equals(other._03) &&
            _10.Equals(other._10) &&
            _11.Equals(other._11) &&
            _12.Equals(other._12) &&
            _13.Equals(other._13) &&
            _20.Equals(other._20) &&
            _21.Equals(other._21) &&
            _22.Equals(other._22) &&
            _23.Equals(other._23) &&
            _30.Equals(other._30) &&
            _31.Equals(other._31) &&
            _32.Equals(other._32) &&
            _33.Equals(other._33);

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
