using System;
using System.Windows.Media;

namespace PXLed
{
    public struct Color24
    {
        /// <summary>
        /// Creates Color24 from RGB
        /// </summary>
        /// <param name="r">Red component from 0 to 255</param>
        /// <param name="g">Green component from 0 to 255</param>
        /// <param name="b">Blue component from 0 to 255</param>
        public Color24(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public readonly byte r;
        public readonly byte g;
        public readonly byte b;

        /// <summary>
        /// Creates Color24 from RGB
        /// </summary>
        /// <param name="r">Red component from 0 to 255</param>
        /// <param name="g">Green component from 0 to 255</param>
        /// <param name="b">Blue component from 0 to 255</param>
        public static Color24 FromRGB(byte r, byte g, byte b)
        {
            return new Color24(r, g, b);
        }

        // https://stackoverflow.com/a/1626232
        /// <summary>
        /// Creates Color24 from HSV
        /// </summary>
        /// <param name="hue">Hue from 0 to 360 degrees</param>
        /// <param name="saturation">Saturation from 0 to 1</param>
        /// <param name="value">Value from 0 to 1</param>
        public static Color24 FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return FromRGB(v, t, p);
            else if (hi == 1)
                return FromRGB(q, v, p);
            else if (hi == 2)
                return FromRGB(p, v, t);
            else if (hi == 3)
                return FromRGB(p, q, v);
            else if (hi == 4)
                return FromRGB(t, p, v);
            else
                return FromRGB(v, p, q);
        }

        public static Color24 Random()
        {
            Random r = new();
            return FromRGB((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
        }

        public static implicit operator Color(Color24 c24)
        {
            return Color.FromRgb(c24.r, c24.g, c24.b);
        }

        public static implicit operator Color24(Color c)
        {
            return new Color24(c.R, c.G, c.B);
        }
    }
}
