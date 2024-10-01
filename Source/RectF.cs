﻿using System;
using System.Numerics;

#nullable enable

namespace Maths
{
    public struct RectF : IEquatable<RectF>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Top
        {
            readonly get => Y;
            set
            {
                float diff = Y - value;
                Y = value;
                Height += diff;
            }
        }
        public float Left
        {
            readonly get => X;
            set
            {
                float diff = X - value;
                X = value;
                Width += diff;
            }
        }
        public float Bottom
        {
            readonly get => Y + Height;
            set => Height = value - Y;
        }
        public float Right
        {
            readonly get => X + Width;
            set => Width = value - X;
        }

        public Vector2 Position
        {
            readonly get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size
        {
            readonly get => new(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public RectF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectF(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public readonly bool Contains(Vector2 point) =>
            point.X >= X &&
            point.Y >= Y &&
            point.X <= Right &&
            point.Y <= Bottom;
        public readonly bool Contains(float x, float y) =>
            x >= X &&
            y >= Y &&
            x <= Right &&
            y <= Bottom;

        public override readonly bool Equals(object? obj) =>
            obj is RectF rect &&
            Equals(rect);
        public readonly bool Equals(RectF other) =>
            X == other.X &&
            Y == other.Y &&
            Width == other.Width &&
            Height == other.Height;

        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        public static bool operator ==(RectF left, RectF right) => left.Equals(right);
        public static bool operator !=(RectF left, RectF right) => !(left == right);

        public override readonly string ToString() => $"(X: {X:0.00} Y: {Y:0.00} W: {Width:0.00} H: {Height:0.00})";

        public RectF Expand(int v)
        {
            X -= v;
            Y -= v;
            Width += v * 2;
            Height += v * 2;

            return this;
        }

        public RectF Expand(Vector2 v)
        {
            X -= v.X;
            Y -= v.Y;
            Width += v.X * 2;
            Height += v.Y * 2;

            return this;
        }

        public RectF Expand(float top, float right, float bottom, float left)
        {
            X -= left;
            Y -= top;
            Width += left + right;
            Height += top + bottom;

            return this;
        }

        public static RectF Intersection(RectF a, RectF b)
        {
            float left = Math.Max(a.Left, b.Left);
            float right = Math.Min(a.Right, b.Right);
            float top = Math.Max(a.Top, b.Top);
            float bottom = Math.Min(a.Bottom, b.Bottom);

            return new RectF(left, top, right - left, bottom - top);
        }

        public readonly bool Overlaps(RectF other) =>
            !(this.Left > other.Left + other.Width) &&
            !(this.Left + this.Width < other.Left) &&
            !(this.Top > other.Top + other.Height) &&
            !(this.Top + this.Height < other.Top);

        public readonly bool Contains(RectF other) =>
            this.X <= other.X &&
            this.Y <= other.Y &&
            this.Right >= other.Right &&
            this.Bottom >= other.Bottom;

        public RectF Move(Vector2 offset)
        {
            this.X += offset.X;
            this.Y += offset.Y;
            return this;
        }

        public RectF Move(float x, float y)
        {
            this.X += x;
            this.Y += y;
            return this;
        }

        public static RectF Move(RectF rect, Vector2 offset)
        {
            rect.X += offset.X;
            rect.Y += offset.Y;
            return rect;
        }

        public static RectF Move(RectF rect, float x, float y)
        {
            rect.X += x;
            rect.Y += y;
            return rect;
        }

#if UNITY

        public static implicit operator UnityEngine.Rect(RectF v) => new(v.X, v.Y, v.Width, v.Height);
        public static implicit operator RectF(UnityEngine.Rect v) => new(v.xMin, v.yMin, v.width, v.height);

#endif
    }
}
