using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable

namespace Maths
{
    public readonly struct QuadTreeBranch :
        IEquatable<QuadTreeBranch>,
        IEqualityOperators<QuadTreeBranch, QuadTreeBranch, bool>
    {
        readonly uint Branch;
        readonly uint Depth;

        public uint[] Branches
        {
            get
            {
                uint[] result = new uint[Depth];

                uint branch = Branch;

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = branch & 0b_11;
                    branch >>= 2;
                }
                Array.Reverse(result);

                return result;
            }
        }

        public QuadTreeBranch(uint branch, uint depth)
        {
            this.Branch = branch;
            this.Depth = depth;
        }

        public static explicit operator ulong(QuadTreeBranch location) => ((ulong)location.Branch << 32) | location.Depth;
        public static explicit operator QuadTreeBranch(ulong location) => new((uint)(location >> 32), (uint)(location & uint.MaxValue));

        public static bool operator ==(QuadTreeBranch left, QuadTreeBranch right) => left.Equals(right);
        public static bool operator !=(QuadTreeBranch left, QuadTreeBranch right) => !left.Equals(right);

        public override bool Equals(object? obj) => obj is QuadTreeBranch other && Equals(other);
        public bool Equals(QuadTreeBranch other) => Branch == other.Branch && Depth == other.Depth;
        public override int GetHashCode() => HashCode.Combine(Branch, Depth);
    }

    public class QuadTree<T> : IReadOnlyCollection<T>
    {
        const int MaxDepth = 4;

        public int Count
        {
            get
            {
                int count = _container.Count;
                for (int i = 0; i < _children.Length; i++)
                {
                    count += _children[i]?.Count ?? 0;
                }
                return count;
            }
        }

        readonly int _depth;
        readonly RectF[] _childrenRects;
        readonly QuadTree<T>?[] _children;
        readonly List<(RectF Rect, T Item)> _container;

        QuadTree()
        {
            _childrenRects = new RectF[4];
            _children = new QuadTree<T>?[4];
            _container = new List<(RectF, T)>();
        }

        QuadTree(RectF rect, int depth) : this()
        {
            _depth = depth;
            Resize(rect);
        }

        public QuadTree(RectF rect) : this()
        {
            _depth = 0;
            Resize(rect);
        }

        public void Resize(RectF rect)
        {
            Clear();
            RectF childSize = new(rect.Position, rect.Size * 0.5f);
            _childrenRects[0] = childSize;
            _childrenRects[1] = RectF.Move(childSize, childSize.Size.X, 0f);
            _childrenRects[2] = RectF.Move(childSize, 0f, childSize.Size.Y);
            _childrenRects[3] = RectF.Move(childSize, childSize.Size.X, childSize.Size.Y);
        }

        public void Clear()
        {
            _container.Clear();
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i]?.Clear();
                _children[i] = null;
            }
        }

        public QuadTreeBranch Add(T item, RectF itemArea)
        {
            uint branchId = 0;
            uint depth = 0;
            Add(item, itemArea, ref branchId, ref depth);
            return new QuadTreeBranch(branchId, depth);
        }

        void Add(T item, RectF itemArea, ref uint branchId, ref uint depth)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_childrenRects[i].Contains(itemArea) || _depth + 1 >= MaxDepth)
                {
                    continue;
                }

                _children[i] ??= new QuadTree<T>(_childrenRects[i], _depth + 1);

                if (i is < 0b_00 or > 0b_11) throw new Exception("Bruh");

                branchId <<= 2;
                branchId |= (uint)i;
                depth++;

                _children[i]!.Add(item, itemArea, ref branchId, ref depth);
                return;
            }

            _container.Add((itemArea, item));
        }

        public ReadOnlySpan<T> SearchAll(RectF area)
        {
            List<T> result = new();
            SearchAll(area, result);
            return CollectionsMarshal.AsSpan(result);
        }

        public void SearchAll(RectF area, List<T> result)
        {
            for (int i = 0; i < _container.Count; i++)
            {
                if (_container[i].Rect.Overlaps(area))
                { result.Add(_container[i].Item); }
            }

            for (int i = 0; i < 4; i++)
            {
                if (_children[i] is null) continue;

                if (area.Contains(_childrenRects[i]))
                { _children[i]?.All(result); }
                else
                { _children[i]?.SearchAll(area, result); }
            }
        }

        public T? Search(RectF area)
        {
            for (int i = 0; i < _container.Count; i++)
            {
                if (_container[i].Rect.Overlaps(area))
                { return _container[i].Item; }
            }

            for (int i = 0; i < 4; i++)
            {
                if (_children[i] is null) continue;
                T? item = _children[i]!.Search(area);
                if (item is not null) return item;
            }

            return default;
        }

        public void All(List<T> list)
        {
            for (int i = 0; i < _container.Count; i++)
            { list.Add(_container[i].Item); }

            for (int i = 0; i < _children.Length; i++)
            { _children[i]?.All(list); }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _container.Count; i++)
            {
                yield return _container[i].Item;
            }

            for (int i = 0; i < _children.Length; i++)
            {
                QuadTree<T>? child = _children[i];

                if (child is null) continue;

                foreach (T item in child)
                { yield return item; }
            }
        }

        public ReadOnlySpan<QuadTree<T>> Branches(Vector2 point)
        {
            List<QuadTree<T>> result = new();
            Branches(point, result);
            return CollectionsMarshal.AsSpan(result);
        }

        public void Branches(Vector2 point, List<QuadTree<T>> list)
        {
            list.Add(this);
            for (int i = 0; i < _childrenRects.Length; i++)
            {
                if (_childrenRects[i].Contains(point))
                {
                    _children[i]?.Branches(point, list);
                    break;
                }
            }
        }

        QuadTree<T>? GetBranch(int i, ReadOnlySpan<uint> branches)
        {
            if (i >= branches.Length) return this;
            QuadTree<T>? child = _children[branches[i]];
            return child?.GetBranch(i + 1, branches);
        }

        public bool Remove(T? element)
        {
            if (element is null) return false;

            for (int i = _container.Count - 1; i >= 0; i--)
            {
                if (element.Equals(_container[i].Item))
                {
                    _container.RemoveAt(i);
                    return true;
                }
            }

            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i]?.Remove(element) ?? false)
                {
                    return true;
                }
            }

            return true;
        }

        public bool Remove(QuadTreeBranch branch, T? element)
        {
            if (element is null) return false;

            uint[] branches = branch.Branches;

            QuadTree<T>? _branch = GetBranch(0, branches);
            if (_branch is null) return false;

            return _branch.Remove(element);
        }

        public bool Remove<T2>(T2? element) where T2 : IEquatable<T>
        {
            if (element is null) return false;

            for (int i = _container.Count - 1; i >= 0; i--)
            {
                if (element.Equals(_container[i].Item))
                {
                    _container.RemoveAt(i);
                    return true;
                }
            }

            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i]?.Remove<T2>(element) ?? false)
                {
                    return true;
                }
            }

            return true;
        }

        public QuadTreeBranch Relocate(QuadTreeBranch branch, T element, Vector2 position)
            => Relocate(branch, element, new RectF(position, Vector2.Zero));
        public QuadTreeBranch Relocate(QuadTreeBranch branch, T element, RectF rect)
        {
            Remove(branch, element);
            return Add(element, rect);
        }

        public void Relocate(ref QuadTreeBranch branch, T element, Vector2 position)
            => Relocate(ref branch, element, new RectF(position, Vector2.Zero));

        public void Relocate(ref QuadTreeBranch branch, T element, RectF rect)
        {
            Remove(branch, element);
            branch = Add(element, rect);
        }
    }
}
