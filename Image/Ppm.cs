using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Win32.Gdi32;

namespace Maths;

public static class Ppm
{
    static readonly ImmutableArray<char> Whitespace = ImmutableArray.Create('\r', '\n', '\t', ' ');

    static void ConsumeWhiteSpace(string text, ref int i)
    {
        while (i < text.Length && char.IsWhiteSpace(text[i]))
        {
            i++;
        }
    }

    static string? ConsumeUntil(string text, ref int i, ReadOnlySpan<char> v)
    {
        int i_ = text.Length - 1;
        for (int j = 0; j < v.Length; j++)
        {
            if (text.IndexOf(v[j], i) != -1)
            {
                i_ = Math.Min(i_, text.IndexOf(v[j], i));
            }
        }
        if (i_ < 0) return null;
        string result = text[i..i_];
        i = i_;
        return result;
    }

    static void ConsumeJunk(string text, ref int i)
    {
        ConsumeWhiteSpace(text, ref i);
        if (i >= text.Length) return;
        if (text[i] == '#')
        {
            ConsumeUntil(text, ref i, stackalloc char[] { '\r', '\n' });
            ConsumeWhiteSpace(text, ref i);
        }
    }

    static int? ExpectInt(string text, ref int i)
    {
        string? v = ConsumeUntil(text, ref i, Whitespace.AsSpan());
        if (v == null) return null;
        if (!int.TryParse(v, out int result)) return null;
        return result;
    }

    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="System.Security.SecurityException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="IOException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="Exception"/>
    public static Image LoadFile(string file)
    {
        string data = File.ReadAllText(file, Encoding.ASCII);
        int i = 0;

        string? magicNumber = ConsumeUntil(data, ref i, Whitespace.AsSpan());
        if (magicNumber != "P3") throw new FormatException($"Invalid magic number \"{magicNumber}\"");

        ConsumeJunk(data, ref i);
        int? width = ExpectInt(data, ref i);
        ConsumeJunk(data, ref i);
        int? height = ExpectInt(data, ref i);
        ConsumeJunk(data, ref i);
        int? maxRgbValue = ExpectInt(data, ref i) ?? 255;
        ConsumeJunk(data, ref i);

        if (!width.HasValue) throw new FormatException("Invalid width value");
        if (!height.HasValue) throw new FormatException("Invalid height value");
        if (maxRgbValue == 0) throw new Exception($"Invalid maxRgbValue value {maxRgbValue}");

        int width_ = width.Value;
        int height_ = height.Value;
        int maxRgbValue_ = maxRgbValue.Value;

        ColorF[] result = new ColorF[width_ * height_];

        for (int j = 0; j < result.Length; j++)
        {
            int? r = ExpectInt(data, ref i);
            ConsumeJunk(data, ref i);
            int? g = ExpectInt(data, ref i);
            ConsumeJunk(data, ref i);
            int? b = ExpectInt(data, ref i);
            ConsumeJunk(data, ref i);

            if (!r.HasValue) continue;
            if (!g.HasValue) continue;
            if (!b.HasValue) continue;

            int r_ = r.Value;
            int g_ = g.Value;
            int b_ = b.Value;

            result[j] = new ColorF((float)r_ / (float)maxRgbValue_, (float)g_ / (float)maxRgbValue_, (float)b_ / (float)maxRgbValue_);
        }

        return new Image(ImmutableCollectionsMarshal.AsImmutableArray(result), width_, height_);
    }

    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="IOException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    public static void SaveFile(Image image, string file)
    {
        using FileStream fileStream = File.OpenWrite(file);
        using TextWriter textWriter = new StreamWriter(fileStream);
        Ppm.Save(image, textWriter);
    }

    /// <exception cref="IOException"/>
    public static void Save(Image image, TextWriter writer)
    {
        writer.Write("P3");
        writer.Write(Environment.NewLine);

        writer.Write(image.Width);
        writer.Write(Environment.NewLine);

        writer.Write(image.Height);
        writer.Write(Environment.NewLine);

        writer.Write(byte.MaxValue);
        writer.Write(Environment.NewLine);

        int charCount = 0;
        const int maxCharsPerLine = 80;

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                GdiColor color = (GdiColor)image[x, y];

                string str = $"{(int)color.R} {(int)color.G} {(int)color.B} ";

                charCount += str.Length;
                if (charCount > maxCharsPerLine)
                {
                    writer.Write(Environment.NewLine);
                    charCount = str.Length;
                }

                writer.Write(str);
            }
        }
    }
}
