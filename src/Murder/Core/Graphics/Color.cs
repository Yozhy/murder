using Murder.Utilities;

namespace Murder.Core.Graphics
{
    public struct Color : IEquatable<Color>
    {
        private static readonly Color _white = new(1, 1, 1);
        private static readonly Color _gray = new(0.5f, 0.5f, 0.5f);
        private static readonly Color _black = new(0, 0, 0);
        private static readonly Color _coldGray = new(0.45f, 0.5f, 0.55f);
        private static readonly Color _brightGray = new(0.65f, 0.75f, 0.75f);
        private static readonly Color _warmGray = new(0.55f, 0.5f, 0.45f);
        private static readonly Color _transparent = new(0, 0, 0, 0);
        private static readonly Color _blue = new(0, 0, 1);
        private static readonly Color _green = new(0, 1, 0);
        private static readonly Color _orange= new(1, 0.6f, 0.1f);
        private static readonly Color _red = new(1, 0, 0);
        private static readonly Color _magenta = new(1, 0, 1);
        public static Color White => _white;
        public static Color Gray => _gray;
        public static Color Black => _black;
        public static Color BrightGray => _brightGray;
        public static Color ColdGray => _coldGray;
        public static Color WarmGray => _warmGray;
        public static Color Transparent => _transparent;
        public static Color Red => _red;
        public static Color Blue => _blue;
        public static Color Green => _green;
        public static Color Orange => _orange;
        public static Color Magenta => _magenta;
        public float R = 0;
        public float G = 0;
        public float B = 0;
        public float A = 0;

        public Color(float r, float g, float b) : this(r, g, b, 1) { }
        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public Color FadeAlpha(float alpha) => new(R, G, B, A * alpha);

        public static Color CreateFrom256(int r, int g, int b) =>
            new Color(r / 256f, g / 256f, b / 256f, 1f);

        public static implicit operator Microsoft.Xna.Framework.Color(Color c) => new(c.R, c.G, c.B, c.A);
        public static implicit operator Color(Microsoft.Xna.Framework.Color c) => new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        public static implicit operator Color(System.Numerics.Vector4 c) => new(c.X, c.Y, c.Z, c.W);
        
        public static implicit operator uint(Color c) { uint ret = (uint)(c.A * 255); ret <<= 8; ret += (uint)(c.B * 255); ret <<= 8; ret += (uint)(c.G * 255); ret <<= 8; ret += (uint)(c.R * 255); return ret; }

        public Color Darken(float r) => new(R * r, G * r, B * r, A);
        public static Color operator *(Color l, float r) => new(l.R * r, l.G * r, l.B * r, l.A * r);

        public Color WithAlpha(float alpha) => new(R, G, B, alpha);
        public Color FromNonPremultiplied()
        {
            var color = this;
            return new Color(
                (color.R * color.A / 255),
                (color.G * color.A / 255),
                (color.B * color.A / 255),
                color.A
            );
        }

        public static Color Lerp(Color a, Color b, float factor)
        {
            return new(
                Calculator.Lerp(a.R, b.R, factor),
                Calculator.Lerp(a.G, b.G, factor),
                Calculator.Lerp(a.B, b.B, factor),
                Calculator.Lerp(a.A, b.A, factor)
            );
        }
    }
}