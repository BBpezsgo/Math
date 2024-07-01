using System;
using System.Collections.Generic;

#if UNITY

namespace Maths
{
    public static class CollectionsMarshal
    {
        public static Span<T> AsSpan<T>(List<T> values) => values.ToArray();
    }
}

#endif
