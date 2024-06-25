using System;

namespace Maths
{
    public interface IParsable<T> { }

    public static class SpanExtensions
    {
        /// <exception cref="IndexOutOfRangeException"/>
        public static int Split<T>(this ReadOnlySpan<T> span, Span<System.Range> result, T divider)
            where T : IEquatable<T>
        {
            int start = 0;
            int bufferPtr = 0;
            for (int i = 0; i < span.Length; i++)
            {
                T c = span[i];
                if (c.Equals(divider))
                {
                    result[bufferPtr++] = new System.Range(start, i);
                    start = i + 1;
                }
            }
            return bufferPtr;
        }
    }
}
