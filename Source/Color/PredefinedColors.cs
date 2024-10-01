namespace Maths
{
    public partial struct ColorF
    {
        public static ColorF Red => new(1f, 0f, 0f);
        public static ColorF Green => new(0f, 1f, 0f);
        public static ColorF Blue => new(0f, 0f, 1f);
        public static ColorF Yellow => new(1f, 1f, 0f);
        public static ColorF Cyan => new(0f, 1f, 1f);
        public static ColorF Magenta => new(1f, 0f, 1f);
        public static ColorF Black => new(0f, 0f, 0f);
        public static ColorF White => new(1f, 1f, 1f);
        public static ColorF Gray => new(.5f, .5f, .5f);
    }

    public static class CoolColors
    {
        public static ColorF White => new(1f, 1f, 1f);
        public static ColorF Black => new(0f, 0f, 0f);

        public static ColorF Red => new(0.921568632f, 0.137254909f, 0.08235294f);
        public static ColorF BrightRed => new(0.8588235f, 0.3607843f, 0.282353f);
        public static ColorF DarkRed => new(0.4313726f, 0.05490196f, 0.1176471f);

        public static ColorF Green => new(0.06666667f, 0.7411765f, 0.235294119f);
        public static ColorF BrightGreen => new(0.4941176f, 0.8392157f, 0.3176471f);
        public static ColorF DarkGreen => new(0.09411765f, 0.3803922f, 0.2f);

        public static ColorF Blue => new(0.117647059f, 0.254901975f, 0.921568632f);
        public static ColorF BrightBlue => new(0.1098039f, 0.4352941f, 1f);
        public static ColorF DarkBlue => new(0.05490196f, 0.0627451f, 0.549019635f);

        public static ColorF Yellow => new(0.8862745f, 0.9294118f, 0.09411765f);
        public static ColorF Magenta => new(0.6980392f, 0.01960784f, 0.7098039f);
        public static ColorF Cyan => new(0.1647059f, 0.8313726f, 0.9215686f);

        public static ColorF Orange => new(0.9215686f, 0.5450981f, 0.01960784f);
        public static ColorF Pink => new(0.9490196f, 0.3647059f, 0.9607843f);
        public static ColorF Purple => new(0.5568628f, 0.06666667f, 0.9607843f);
    }
}
