namespace System.Numerics
{
#if !LANG_11
    internal interface IAdditionOperators<TSelf, TOther, TResult> { }
    internal interface ISubtractionOperators<TSelf, TOther, TResult> { }
    internal interface IEqualityOperators<TSelf, TOther, TResult> { }
    internal interface IDivisionOperators<TSelf, TOther, TResult> { }
    internal interface IMultiplyOperators<TSelf, TOther, TResult> { }
    internal interface IMultiplicativeIdentity<TSelf, TResult> { }
#endif
}
