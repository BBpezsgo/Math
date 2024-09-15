using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Maths
{
    public static class RangeExtensions
    {
#if LANG_11

        public static bool IsBackward<T>(this Range<T> range)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>
            => range.Start > range.End;

        public static T Size<T>(this MutableRange<T> range)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>, ISubtractionOperators<T, T, T>
            => General.Max(range.Start, range.End) - General.Min(range.Start, range.End);

        public static T Size<T>(this Range<T> range)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>, ISubtractionOperators<T, T, T>
            => General.Max(range.Start, range.End) - General.Min(range.Start, range.End);

        public static bool Contains<TRange, TValue>(this MutableRange<TRange> range, TValue value)
            where TRange : IEquatable<TRange>, IComparisonOperators<TRange, TValue, bool>, IComparisonOperators<TRange, TRange, bool>
        {
            range = range.Fix();
            return range.Start <= value && range.End >= value;
        }

        public static bool Contains<TRange, TValue>(this Range<TRange> range, TValue value)
            where TRange : IEquatable<TRange>, IComparisonOperators<TRange, TValue, bool>, IComparisonOperators<TRange, TRange, bool>
        {
            range = range.Fix();
            return range.Start <= value && range.End >= value;
        }
        public static bool Overlaps<T>(this MutableRange<T> a, MutableRange<T> b)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>
            => RangeUtils.Overlaps(a, b);

        public static bool Overlaps<T>(this Range<T> a, Range<T> b)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>
            => RangeUtils.Overlaps(a, b);

        public static T Middle<T>(this Range<T> a)
            where T : IEquatable<T>, IAdditionOperators<T, T, T>, IDivisionOperators<T, int, T>
            => (a.Start + a.End) / 2;

        public static Range<T> Fix<T>(this Range<T> v)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>
        {
            T start = General.Min(v.Start, v.End);
            T end = General.Max(v.Start, v.End);
            return new Range<T>(start, end);
        }

        public static MutableRange<T> Fix<T>(this MutableRange<T> v)
            where T : IEquatable<T>, IComparisonOperators<T, T, bool>
        {
            T start = General.Min(v.Start, v.End);
            T end = General.Max(v.Start, v.End);
            return new MutableRange<T>(start, end);
        }

        public static Range<TRange> Offset<TRange, TOffset>(this Range<TRange> v, TOffset offset)
            where TRange : IEquatable<TRange>, IAdditionOperators<TRange, TOffset, TRange>
            => new(v.Start + offset, v.End + offset);

#endif

        public static IEnumerable<int> ForEach(this Range<int> a)
        {
            if (a.End < a.Start) // Reversed
            { return Enumerable.Range(a.End + 1, a.Start - a.End); }
            else
            { return Enumerable.Range(a.Start, a.End - a.Start); }
        }
    }
}
