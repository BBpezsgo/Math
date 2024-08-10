using System.Collections.Generic;
using System.Collections.Immutable;

#if UNITY

namespace System.Linq
{
    public static class NotSupportedLinq
    {
        public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> values) => new(values.ToArray());
    }
}

#endif
