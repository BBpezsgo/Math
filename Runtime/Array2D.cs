#pragma warning disable IDE0032 // Use auto property

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace Maths
{
    public readonly struct Array2D<T> : IList, IList<T>
    {
        readonly T[] _array;
        readonly int _width;

        public int Width => _width;
        public int Height => _array.Length / _width;

        public ref T this[int i] => ref _array[i];
        public ref T this[int x, int y] => ref _array[x + (y * _width)];
        public ref T this[Vector2Int v] => ref _array[v.X + (v.Y * _width)];

        public Array2D(T[] array, int width)
        {
            _array = array;
            _width = width;
        }

        public Array2D(int width, int height)
        {
            _array = new T[width * height];
            _width = width;
        }

        public void CopyTo(Array2D<T> other) => Array.Copy(_array, other._array, _array.Length);
        public T[] AsArray() => _array;
        public void Clear() => Array.Clear(_array, 0, _array.Length);
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
        public override string ToString() => $"({Width}x{Height})";

        public bool IsVisible(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
        public bool IsVisible(Vector2Int p) => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;

        #region Explicit implementations
        int ICollection<T>.Count => _array.Length;
        bool ICollection<T>.IsReadOnly => _array.IsReadOnly;
        bool IList.IsFixedSize => _array.IsFixedSize;
        bool IList.IsReadOnly => _array.IsReadOnly;
        int ICollection.Count => _array.Length;
        bool ICollection.IsSynchronized => _array.IsFixedSize;
        object ICollection.SyncRoot => _array.SyncRoot;
        /// <exception cref="ArgumentNullException"/>
        object? IList.this[int index] { get => _array[index]; set => _array[index] = (T)(value ?? throw new ArgumentNullException(nameof(value))); }
        T IList<T>.this[int index] { get => _array[index]; set => _array[index] = value; }
        [Obsolete("Do not use this")] int IList<T>.IndexOf(T item) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void ICollection<T>.Add(T item) => throw new NotImplementedException();
        [Obsolete("Do not use this")] bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        [Obsolete("Do not use this")] bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        [Obsolete("Do not use this")] int IList.Add(object? value) => throw new NotImplementedException();
        [Obsolete("Do not use this")] bool IList.Contains(object? value) => throw new NotImplementedException();
        [Obsolete("Do not use this")] int IList.IndexOf(object? value) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void IList.Insert(int index, object? value) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void IList.Remove(object? value) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void IList.RemoveAt(int index) => throw new NotImplementedException();
        [Obsolete("Do not use this")] void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        #endregion
    }
}
