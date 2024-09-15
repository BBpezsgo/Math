using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maths
{
    public class Frequency<T> : IEnumerable<KeyValuePair<T, int>> where T : notnull
    {
        readonly Dictionary<T, int> _dict;

        public int this[T key] => _dict.TryGetValue(key, out int f) ? f : 0;

        public int Total => _dict.Values.Sum();

        public Frequency()
        {
            _dict = new Dictionary<T, int>();
        }

        public void Add(T item)
        {
            if (!_dict.TryAdd(item, 1))
            {
                _dict[item]++;
            }
        }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        public override int GetHashCode() => _dict.GetHashCode();
        public override bool Equals(object? obj) => obj is Frequency<T> other && _dict.Equals(other._dict);

        public KeyValuePair<T, int>[] ToSorted()
        {
            KeyValuePair<T, int>[] sorted = this.ToArray();
            Array.Sort(sorted, (a, b) => b.Value - a.Value);
            return sorted;
        }
    }

    public static class FrequencyExtensions
    {
        public static int GetAverage(this Frequency<int> self)
        {
            int sum = 0;
            int n = 0;
            foreach (KeyValuePair<int, int> pair in self)
            {
                sum += pair.Key * pair.Value;
                n += pair.Value;
            }
            return sum / n;
        }
    }
}