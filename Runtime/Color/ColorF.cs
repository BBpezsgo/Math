using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

#nullable enable

namespace Maths
{
    [DebuggerDisplay("{{" + nameof(GetDebuggerDisplay) + "(),nq}}")]
    public partial struct ColorF :
        IEquatable<ColorF>,
        IEqualityOperators<ColorF, ColorF, bool>,

        IAdditionOperators<ColorF, ColorF, ColorF>,
        IDivisionOperators<ColorF, ColorF, ColorF>,
        IMultiplyOperators<ColorF, ColorF, ColorF>,
        ISubtractionOperators<ColorF, ColorF, ColorF>,

        IDivisionOperators<ColorF, float, ColorF>,
        IMultiplyOperators<ColorF, float, ColorF>,

        IParsable<ColorF>
    {
        public float R;
        public float G;
        public float B;

        public readonly float Luminance => (0.2126f * R) + (0.7152f * G) + (0.0722f * B);
        public readonly float Intensity => (R + G + B) / 3f;
        /// <summary>
        /// Source: .NET 7 source code
        /// </summary>
        public readonly float Saturation
        {
            get
            {
                if (R == G && G == B) return 0f;

                float min = MinChannel;
                float max = MaxChannel;

                float div = max + min;
                if (div > 1f)
                { div = 2f - max - min; }

                return (max - min) / div;
                /*
                float l = (min + max) * .5f;

                if (l is <= float.Epsilon or >= 1f)
                { return 0f; }

                float s = max - min;

                if (s <= 0f)
                { return s; }

                if (l <= 0.5f)
                { return s / (max + min); }
                else
                { return s / (2f - max - min); }
                */
            }
        }
        public readonly float Lightness => (MinChannel + MaxChannel) * .5f;
        public readonly float Chroma => MaxChannel - MinChannel;
        public readonly float MaxChannel => Math.Max(R, Math.Max(G, B));
        public readonly float MinChannel => Math.Min(R, Math.Min(G, B));
        /// <summary>
        /// Source: .NET 7 source code
        /// </summary>
        public readonly float Hue
        {
            get
            {
                if (R == G && G == B) return 0f;

                float min = MinChannel;
                float max = MaxChannel;

                float delta = max - min;
                float hue;

                if (R == max)
                { hue = (G - B) / delta; }
                else if (G == max)
                { hue = ((B - R) / delta) + 2f; }
                else
                { hue = ((R - G) / delta) + 4f; }

                hue *= 60f;
                if (hue < 0f)
                { hue += 360f; }

                return hue;
            }
        }

        public static ColorF Zero => new(0f);
        public static ColorF One => new(1f);

        public ColorF(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
        public ColorF(float v)
        {
            R = v;
            G = v;
            B = v;
        }

        public ColorF Clamp()
        {
            R = Math.Clamp(R, 0f, 1f);
            G = Math.Clamp(G, 0f, 1f);
            B = Math.Clamp(B, 0f, 1f);

            return this;
        }
        public readonly ColorF Clamped => new(Math.Clamp(R, 0f, 1f), Math.Clamp(G, 0f, 1f), Math.Clamp(B, 0f, 1f));

        public override readonly bool Equals(object? obj) => obj is ColorF other && Equals(other);
        public readonly bool Equals(ColorF other) => R == other.R && G == other.G && B == other.B;
        public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

        public static bool operator ==(ColorF left, ColorF right) =>
            FloatUtils.FloatEquality(left.R, right.R) &&
            FloatUtils.FloatEquality(left.G, right.G) &&
            FloatUtils.FloatEquality(left.B, right.B);
        public static bool operator !=(ColorF left, ColorF right) => !(left == right);

        public static ColorF operator +(ColorF a, ColorF b) => new(a.R + b.R, a.G + b.G, a.B + b.B);
        public static ColorF operator -(ColorF a, ColorF b) => new(a.R - b.R, a.G - b.G, a.B - b.B);
        public static ColorF operator *(ColorF a, ColorF b) => new(a.R * b.R, a.G * b.G, a.B * b.B);
        public static ColorF operator /(ColorF a, ColorF b) => new(a.R / b.R, a.G / b.G, a.B / b.B);

        public static ColorF operator *(ColorF a, float b) => new(a.R * b, a.G * b, a.B * b);
        public static ColorF operator /(ColorF a, float b) => new(a.R / b, a.G / b, a.B / b);

        public static ColorF operator *(float a, ColorF b) => new(a * b.R, a * b.G, a * b.B);

        public override readonly string ToString() => $"({R:0.00}, {G:0.00}, {B:0.00})";
        readonly string GetDebuggerDisplay() => ToString();

        /// <summary>
        /// return the squared Euclidean distance between two colors
        /// </summary>
        public static float DistanceSqr(ColorF a, ColorF b)
        {
            ColorF d = a - b;
            return (d.R * d.R) + (d.G * d.G) + (d.B * d.B);
        }

        /// <exception cref="FormatException"/>
        public static ColorF Parse(string s, IFormatProvider? provider = null) => Parse(s.AsSpan(), provider);

        /// <exception cref="FormatException"/>
        public static ColorF Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            ColorF color = default;
            s = s.Trim();
            if (s.IsEmpty)
            { throw new FormatException(); }

            if (s[0] == '#')
            {
                if (!int.TryParse(s.Slice(1, 2), NumberStyles.HexNumber, provider, out int r))
                { throw new FormatException(); }
                if (!int.TryParse(s.Slice(3, 2), NumberStyles.HexNumber, provider, out int g))
                { throw new FormatException(); }
                if (!int.TryParse(s.Slice(5, 2), NumberStyles.HexNumber, provider, out int b))
                { throw new FormatException(); }
                color.R = (float)r / 255f;
                color.G = (float)g / 255f;
                color.B = (float)b / 255f;
                return color;
            }

            Span<System.Range> parts = stackalloc System.Range[4];

            {
                int n = s.Split(parts, ' ');
                parts = parts[..n];
            }

            if (parts.Length != 3)
            { throw new FormatException(); }

            if (!float.TryParse(s[parts[0]], NumberStyles.Float, provider, out color.R))
            { throw new FormatException(); }
            if (!float.TryParse(s[parts[1]], NumberStyles.Float, provider, out color.G))
            { throw new FormatException(); }
            if (!float.TryParse(s[parts[2]], NumberStyles.Float, provider, out color.B))
            { throw new FormatException(); }

            return color;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ColorF result)
        {
            result = default;

            if (s == null) return false;

            s = s.Trim();
            string[] parts = s.Split(' ');

            if (parts.Length != 3)
            { return false; }

            if (!float.TryParse(parts[0], NumberStyles.Float, provider, out result.R))
            { return false; }
            if (!float.TryParse(parts[1], NumberStyles.Float, provider, out result.G))
            { return false; }
            if (!float.TryParse(parts[2], NumberStyles.Float, provider, out result.B))
            { return false; }

            return true;
        }

        public static ColorF Lerp(ColorF a, ColorF b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return (a * (1f - t)) + (b * t);
        }

        public readonly ColorF NormalizeIntensity()
        {
            float maxChannel = MaxChannel;
            if (maxChannel == 0)
            { return default; }

            float r = Math.Clamp(R / maxChannel, 0, 1);
            float g = Math.Clamp(G / maxChannel, 0, 1);
            float b = Math.Clamp(B / maxChannel, 0, 1);
            return new ColorF(r, g, b);
        }

        public static ColorF Random()
        {
            int r = Maths.Random.Integer();
            int g = Maths.Random.Integer();
            int b = Maths.Random.Integer();
            return new ColorF((float)r / (float)int.MaxValue, (float)g / (float)int.MaxValue, (float)b / (float)int.MaxValue);
        }

        public readonly ColorF RedistributeExcessRGB()
        {
            const float threshold = 1f;
            float m = MaxChannel;
            if (m <= threshold) return this;
            float total = R + G + B;
            if (total >= 3 * threshold) return new ColorF(threshold, threshold, threshold);
            float x = ((3 * threshold) - total) / ((3 * m) - total);
            float gray = threshold - (x * m);
            return new ColorF(gray + (x * R), gray + (x * G), gray + (x * B));
        }

        public static (float C, float M, float Y, float K) ToCMYK(ColorF color)
        {
            float k = MathF.Min(255 - color.R, MathF.Min(1f - color.G, 1f - color.B));
            float c = 1f * (1f - color.R - k) / (1f - k);
            float m = 1f * (1f - color.G - k) / (1f - k);
            float y = 1f * (1f - color.B - k) / (1f - k);
            return (c, m, y, k);
        }

        public static ColorF ToRGB(float c, float m, float y, float k)
        {
            float r = -((c * (1f - k)) / 1f + k - 1f);
            float g = -((m * (1f - k)) / 1f + k - 1f);
            float b = -((y * (1f - k)) / 1f + k - 1f);
            return new ColorF(r, g, b);
        }

        public static ColorF MixCMYK(ColorF a, ColorF b)
        {
            var _a = ToCMYK(a);
            var _b = ToCMYK(b);
            return ToRGB(_a.C + _b.C, _a.M + _b.M, _a.Y + _b.Y, _a.K + _b.K);
        }

        public static ColorF Sqrt(ColorF color) => new(MathF.Sqrt(color.R), MathF.Sqrt(color.G), MathF.Sqrt(color.B));
#if UNITY
        public static UnityEngine.Color Sqrt(UnityEngine.Color color) => new(MathF.Sqrt(color.r), MathF.Sqrt(color.g), MathF.Sqrt(color.b));
#endif

        public static ColorF CoolLerp(ColorF a, ColorF b, float t)
        {
            float at = MathF.Min(1f, (t - 1f) * -2f);
            float bt = MathF.Min(1f, t * 2f);
            float cr = MathF.Min((a.R * at) + (b.R * bt), 1f);
            float cg = MathF.Min((a.G * at) + (b.G * bt), 1f);
            float cb = MathF.Min((a.B * at) + (b.B * bt), 1f);
            return new ColorF(cr, cg, cb);
        }
    }
}
