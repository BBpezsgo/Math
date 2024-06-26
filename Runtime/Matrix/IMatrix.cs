using System;
using System.Numerics;

namespace Maths
{
    public interface IMatrix
    {
        /// <exception cref="IndexOutOfRangeException"/>
        public float this[int r, int c] { get; set; }
    }

    internal interface IMatrix<TSelf> :
        IMatrix,
        IEquatable<TSelf>,
        IMultiplyOperators<TSelf, TSelf, TSelf>,
        IEqualityOperators<TSelf, TSelf, bool>,
        IMultiplicativeIdentity<TSelf, TSelf>
        where TSelf : IMatrix<TSelf>
    {
        public TSelf Transpose();
    }

    internal interface ILargeMatrix : IMatrix
    {
        public float Minor(int x, int y);
        public float Cofactor(int r, int c);
        public float Det();
    }

    internal interface ILargeMatrix<TSelf, TSmaller>
        : IMatrix<TSelf>,
        ILargeMatrix
        where TSelf : ILargeMatrix<TSelf, TSmaller>
        where TSmaller : IMatrix<TSmaller>
    {
        public TSmaller Sub(int x, int y);
        public TSelf Inverse();
    }
}
