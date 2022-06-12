using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.RegularExpressions;
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

        // https://gist.github.com/guri-sharp/fecc601a65fe4b98a080
        [JsonIgnore]
        public double Hue
        {
            get
            {
                float max = Math.Max(Math.Max(r, g), b);
                float min = Math.Min(Math.Min(r, g), b);

                double h;

                if (max == min)
                {
                    h = 0;
                }
                else
                {
                    h = 0;

                    if (max == r)
                    {
                        h = (60 * (g - b) / (max - min));
                        if (h < 0) h += 360;
                    }
                    else if (max == g)
                    {
                        h = (60 * (2 + (b - r) / (max - min)));
                        if (h < 0) h += 360;
                    }
                    else if (max == b)
                    {
                        h = (60 * (4 + (r - g) / (max - min)));
                        if (h < 0) h += 360;
                    }
                }

                return h;
            }
        }

        [JsonIgnore]
        public double Saturation
        {
            get
            {
                float max = Math.Max(Math.Max(r, g), b);
                float min = Math.Min(Math.Min(r, g), b);

                double s;

                if (max == min)
                {
                    s = 0;
                }
                else
                {
                    s = (max - min) / max;
                }

                return s;
            }
        }

        [JsonIgnore]
        public double Value
        {
            get
            {
                float max = Math.Max(Math.Max(r, g), b);
                double v = max / 255;
                return v;
            }
        }

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

        /// <summary>
        /// Constructs a <see cref="Color24"/> from a string with hexadecimal color data ("#RRGGBB")
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static Color24 FromHex(string hex)
        {
            // Regex matches for "#RRGGBB" format
            if (!Regex.Match(hex, @"^#[a-fA-F0-9]{6}$").Success)
                throw new ArgumentException($"{hex} is not a hex string");

            // Convert each component of the hex string to a value
            byte r = Convert.ToByte(hex[1..3], 16);
            byte g = Convert.ToByte(hex[3..5], 16);
            byte b = Convert.ToByte(hex[5..7], 16);

            return new Color24(r, g, b);
        }

        public static Color24 Random()
        {
            Random r = new();
            return FromRGB((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
        }

        /// <summary>
        /// Linear interpolation between the RGB values of two colors.
        /// </summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <param name="t">How much to interpolate between 0 to 1.</param>
        /// <returns></returns>
        public static Color24 LerpRGB(Color24 a, Color24 b, float t)
        {
            // Lerp each RGB component of the two colors, then create a new color from these components
            float newR = a.r + (b.r - a.r) * t;
            float newG = a.g + (b.g - a.g) * t;
            float newB = a.b + (b.b - a.b) * t;

            return new Color24((byte)Math.Round(newR), (byte)Math.Round(newG), (byte)Math.Round(newB));
        }

        // Inspired by https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=windowsdesktop-6.0
        /// <summary>
        /// Converts the color to 24-bit integer where each component takes up 8 bits.
        /// </summary>
        public int ToInt24()
        {
            int colorInt = r << 16;
            colorInt |= g << 8;
            colorInt |= b << 0;

            return colorInt;
        }

        /// <summary>
        /// Converts the color into a string with color data in hexadecimal form ("#RRGGBB")
        /// </summary>
        public override string ToString()
        {
            // Start String with # because format
            StringBuilder stringBuilder = new("#");

            // Add each component to string
            stringBuilder.Append(ComponentToHexString(r));
            stringBuilder.Append(ComponentToHexString(g));
            stringBuilder.Append(ComponentToHexString(b));

            return stringBuilder.ToString();
        }

        string ComponentToHexString(int c)
        {
            // If component is smaller than 16, the converted string will be just one character long, so prevent that for fancy hex string
            if (c < 16)
                return $"0{Convert.ToString(c, 16)}";
            else
                return Convert.ToString(c, 16);
        }

        public static implicit operator Color(Color24 c24)
        {
            return Color.FromRgb(c24.r, c24.g, c24.b);
        }

        public static implicit operator Color24(Color c)
        {
            return new Color24(c.R, c.G, c.B);
        }

        public static bool operator ==(Color24 left, Color24 right)
        {
            return left.r == right.r && left.g == right.g && left.b == right.b;
        }

        public static bool operator !=(Color24 left, Color24 right)
        {
            return left.r != right.r || left.g != right.g || left.b != right.b;
        }
    }
}
