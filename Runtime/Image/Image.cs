using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace Maths
{
    public readonly struct Image
    {
        public readonly ImmutableArray<ColorF> Data;
        public readonly int Width;
        public readonly int Height;

        public ColorF this[int x, int y] => Data[x + (Width * y)];
        public ColorF this[Vector2Int point] => Data[point.X + (Width * point.Y)];

        public ColorF GetPixelWithUV(Vector2 uv, Vector2 point)
        {
            Vector2 transformedPoint = point / uv;
            transformedPoint *= new Vector2(Width, Height);
            Vector2Int imageCoord = transformedPoint.Floor();
            return this[imageCoord];
        }

        public Image(Span2D<ColorF> data)
        {
            Data = ImmutableArray.Create((ReadOnlySpan<ColorF>)data.AsSpan());
            Width = data.Width;
            Height = data.Height;
        }

        public Image(ImmutableArray<ColorF> data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        public Image(IEnumerable<TransparentColor> data, int width, int height)
        {
            Data = data.Select(v => (ColorF)v).ToImmutableArray();
            Width = width;
            Height = height;
        }

        public ColorF NormalizedSample(float texU, float texV)
        {
            int x = (int)(texU * Width);
            int y = (int)(texV * Height);

            x = Math.Clamp(x, 0, Width - 1);
            y = Math.Clamp(y, 0, Height - 1);

            return this[x, y];
        }
    }
}
