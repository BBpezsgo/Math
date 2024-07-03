using System;
using System.Collections.Generic;

namespace Maths
{
    public class VisitInDisorder
    {
        readonly long _maxRange;
        readonly long _prime;
        readonly long _offset;
        long _index;

        public long Current { get; private set; }
        public bool HasNext => _index < _maxRange;

        public VisitInDisorder(long range)
        {
            if (range < 2) throw new ArgumentException("Your range need to be greater than 1 ", nameof(range));
            _maxRange = range;
            _prime = SelectCoPrimeReserve(range / 2, range);
            _offset = (long)Random.Int64(range);
            _index = 0;
            Current = _offset;
        }

        long GetCurrentValue() => (long)((((long)_index * _prime) + _offset) % _maxRange);

        public long Next()
        {
            Current += _prime;
            if (Current >= _maxRange) Current -= _maxRange;
            _index++;
            if (Current != GetCurrentValue()) throw new Exception("Bug");

            return Current;
        }

        const long MAX_COUNT = 100000;

        static long SelectCoPrimeReserve(long min, long target)
        {
            long count = 0;
            long selected = 0;
            for (long val = min; val < target; ++val)
            {
                if (Coprime(val, target))
                {
                    count++;
                    if ((count == 1) || (Random.Int64(count) < 1))
                    {
                        selected = val;
                    }
                }

                if (count == MAX_COUNT) return val;
            }
            return selected;
        }

        static bool Coprime(long u, long v) => Gcd(u, v) == 1;

        static long Gcd(long u, long v)
        {
            if (u == 0) return v;
            if (v == 0) return u;

            int shift;
            for (shift = 0; ((u | v) & 1) == 0; ++shift)
            {
                u >>= 1;
                v >>= 1;
            }

            while ((u & 1) == 0)
            { u >>= 1; }

            do
            {
                while ((v & 1) == 0)
                { v >>= 1; }

                if (u > v)
                {
                    long temp = v;
                    v = u;
                    u = temp;
                }
                v -= u;
            } while (v != 0);

            return u << shift;
        }
    }

    public static class DisorderExtensions
    {
        public static IEnumerable<int> Disorder(this RangeInt range)
        {
            VisitInDisorder visit = new(range.B - range.A);
            while (visit.HasNext)
            { yield return (int)(visit.Next() + range.A); }
        }

        public static IEnumerable<T> Disorder<T>(this IReadOnlyList<T> values)
        {
            VisitInDisorder visit = new(values.Count);
            while (visit.HasNext)
            { yield return values[(int)visit.Next()]; }
        }
    }
}
