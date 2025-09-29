using System;
using Windows.UI;

namespace LifeTimer.Helpers
{
    public static class ColorHelperUtil
    {
        /// <summary>
        /// Converts HSV values to RGB Color
        /// </summary>
        /// <param name="hue">Hue (0-360)</param>
        /// <param name="saturation">Saturation (0-1)</param>
        /// <param name="value">Value/Brightness (0-1)</param>
        /// <returns>RGB Color</returns>
        public static Color HsvToRgb(double hue, double saturation, double value)
        {
            // Ensure values are in valid ranges
            hue = Math.Max(0, Math.Min(360, hue));
            saturation = Math.Max(0, Math.Min(1, saturation));
            value = Math.Max(0, Math.Min(1, value));

            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            double r = 0, g = 0, b = 0;

            if (hue >= 0 && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue >= 60 && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue >= 120 && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue >= 180 && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue >= 240 && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (hue >= 300 && hue < 360)
            {
                r = c; g = 0; b = x;
            }

            byte red = (byte)Math.Round((r + m) * 255);
            byte green = (byte)Math.Round((g + m) * 255);
            byte blue = (byte)Math.Round((b + m) * 255);

            return Color.FromArgb(255, red, green, blue);
        }

        /// <summary>
        /// Converts RGB Color to HSV values
        /// </summary>
        /// <param name="color">RGB Color</param>
        /// <returns>Tuple of (Hue, Saturation, Value)</returns>
        public static (double Hue, double Saturation, double Value) RgbToHsv(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            // Calculate Hue
            double hue = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
                else if (max == g)
                {
                    hue = 60 * ((b - r) / delta + 2);
                }
                else if (max == b)
                {
                    hue = 60 * ((r - g) / delta + 4);
                }
            }

            if (hue < 0) hue += 360;

            // Calculate Saturation
            double saturation = max == 0 ? 0 : delta / max;

            // Calculate Value
            double value = max;

            return (hue, saturation, value);
        }

        /// <summary>
        /// Gets a color from the hue spectrum (full saturation and value)
        /// </summary>
        /// <param name="hue">Hue (0-360)</param>
        /// <returns>Pure hue color</returns>
        public static Color GetHueColor(double hue)
        {
            return HsvToRgb(hue, 1.0, 1.0);
        }

        /// <summary>
        /// Gets a color with the specified hue and position in SV space
        /// </summary>
        /// <param name="hue">Hue (0-360)</param>
        /// <param name="x">X position (0-1) - represents saturation</param>
        /// <param name="y">Y position (0-1) - represents value (inverted)</param>
        /// <returns>Color at the specified HSV coordinates</returns>
        public static Color GetColorFromSvPosition(double hue, double x, double y)
        {
            double saturation = Math.Max(0, Math.Min(1, x));
            double value = Math.Max(0, Math.Min(1, 1 - y)); // Invert Y so top is bright, bottom is dark

            return HsvToRgb(hue, saturation, value);
        }

        /// <summary>
        /// Gets the position in SV space for a given color with the specified hue
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <param name="hue">Current hue value</param>
        /// <returns>Tuple of (X, Y) position in SV space</returns>
        public static (double X, double Y) GetSvPositionFromColor(Color color, double hue)
        {
            var (_, saturation, value) = RgbToHsv(color);
            return (saturation, 1 - value); // Invert Y so top is bright, bottom is dark
        }

        /// <summary>
        /// Interpolates between two colors
        /// </summary>
        /// <param name="color1">First color</param>
        /// <param name="color2">Second color</param>
        /// <param name="t">Interpolation factor (0-1)</param>
        /// <returns>Interpolated color</returns>
        public static Color InterpolateColors(Color color1, Color color2, double t)
        {
            t = Math.Max(0, Math.Min(1, t));

            byte r = (byte)(color1.R + (color2.R - color1.R) * t);
            byte g = (byte)(color1.G + (color2.G - color1.G) * t);
            byte b = (byte)(color1.B + (color2.B - color1.B) * t);

            return Color.FromArgb(255, r, g, b);
        }
    }
}