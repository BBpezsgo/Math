namespace Maths
{
    public delegate TColor Quantizer<TColor>(TColor c);

    public static class Dithering
    {
#if LANG_11
        public static void Dither<TColor>(Array2D<TColor> buffer, Quantizer<TColor> quantize)
            where TColor :
                ISubtractionOperators<TColor, TColor, TColor>,
                IAdditionOperators<TColor, TColor, TColor>,
                IMultiplyOperators<TColor, float, TColor>
            => Dithering.Dither<TColor>((Span2D<TColor>)buffer, quantize);

        public static void DitherNoEdges<TColor>(Array2D<TColor> buffer, Quantizer<TColor> quantize)
            where TColor :
                ISubtractionOperators<TColor, TColor, TColor>,
                IAdditionOperators<TColor, TColor, TColor>,
                IMultiplyOperators<TColor, float, TColor>
            => Dithering.DitherNoEdges<TColor>((Span2D<TColor>)buffer, quantize);

        public static void Dither<TColor>(Span2D<TColor> buffer, Quantizer<TColor> quantize)
            where TColor :
                ISubtractionOperators<TColor, TColor, TColor>,
                IAdditionOperators<TColor, TColor, TColor>,
                IMultiplyOperators<TColor, float, TColor>
        {
            for (int y = 0; y < buffer.Height; y++)
            {
                for (int x = 0; x < buffer.Width; x++)
                {
                    TColor c = buffer[x, y];
                    TColor quantized = quantize.Invoke(c);

                    TColor error = c - quantized;

                    bool left = x > 0;
                    bool bottom = y < buffer.Height - 1;
                    bool right = x < buffer.Width - 1;

                    if (left && bottom && right)
                    {
                        buffer[x + 1, y] += error * (7f / 16f); // Right
                        buffer[x - 1, y + 1] += error * (3f / 16f); // Bottom Left
                        buffer[x, y + 1] += error * (5f / 16f); // Bottom
                        buffer[x + 1, y + 1] += error * (1f / 16f); // Bottom Right
                    }
                    else if (left && bottom && !right)
                    {
                        buffer[x - 1, y + 1] += error * (3f / 16f); // Bottom Left
                        buffer[x, y + 1] += error * (13f / 16f); // Bottom
                    }
                    else if (!bottom && right)
                    {
                        buffer[x + 1, y] += error; // Right
                    }
                    else if (!left && bottom && !right)
                    {
                        buffer[x, y + 1] += error; // Bottom
                    }

                    buffer[x, y] = quantized;
                }
            }
        }

        public static void DitherNoEdges<TColor>(Span2D<TColor> buffer, Quantizer<TColor> quantize)
            where TColor :
                ISubtractionOperators<TColor, TColor, TColor>,
                IAdditionOperators<TColor, TColor, TColor>,
                IMultiplyOperators<TColor, float, TColor>
        {
            for (int y = 0; y < buffer.Height - 1; y++)
            {
                for (int x = 0; x < buffer.Width - 1; x++)
                {
                    TColor c = buffer[x, y];
                    TColor quantized = quantize.Invoke(c);

                    TColor error = c - quantized;

                    buffer[x + 1, y] += error * (7f / 16f); // Right
                    buffer[x - 1, y + 1] += error * (3f / 16f); // Bottom Left
                    buffer[x, y + 1] += error * (5f / 16f); // Bottom
                    buffer[x + 1, y + 1] += error * (1f / 16f); // Bottom Right

                    buffer[x, y] = quantized;
                }
            }
        }
#endif
    }
}
