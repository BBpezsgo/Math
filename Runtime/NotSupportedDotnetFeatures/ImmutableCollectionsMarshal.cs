#if UNITY

using System.Collections.Immutable;

namespace Maths
{
    public static class ImmutableCollectionsMarshal
    {
        public static ImmutableArray<T> AsImmutableArray<T>(T[] values) => new(values);
    }
}

#endif
