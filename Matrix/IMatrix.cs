namespace Maths;

public interface IMatrix
{
    /// <exception cref="IndexOutOfRangeException"/>
    public float this[int r, int c] { get; set; }
}

public interface IMatrix<TSelf> :
    IMatrix,
    IEquatable<TSelf>,
    IMultiplyOperators<TSelf, TSelf, TSelf>,
    IEqualityOperators<TSelf, TSelf, bool>,
    IMultiplicativeIdentity<TSelf, TSelf>
    where TSelf : IMatrix<TSelf>
{
    public TSelf Transpose();
}

public interface ILargeMatrix : IMatrix
{
    public float Minor(int x, int y);
    public float Cofactor(int r, int c);
    public float Det();
}

public interface ILargeMatrix<TSelf, TSmaller>
    : IMatrix<TSelf>,
    ILargeMatrix
    where TSelf : ILargeMatrix<TSelf, TSmaller>
    where TSmaller : IMatrix<TSmaller>
{
    public TSmaller Sub(int x, int y);
    public TSelf Inverse();
}
