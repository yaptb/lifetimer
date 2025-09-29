using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LifeTimer.Helpers
{
    public static class FontHelper
    {
        #region PInvoke Declarations

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern int EnumFontFamiliesEx(
            IntPtr hdc,
            [In] ref LOGFONT lpLogfont,
            EnumFontFamExProc lpEnumFontFamExProc,
            IntPtr lParam,
            uint dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private delegate int EnumFontFamExProc(
            ref ENUMLOGFONTEX lpelfe,
            ref NEWTEXTMETRICEX lpntme,
            uint FontType,
            IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct ENUMLOGFONTEX
        {
            public LOGFONT elfLogFont;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string elfFullName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string elfStyle;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string elfScript;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NEWTEXTMETRICEX
        {
            public NEWTEXTMETRIC ntmTm;
            public FONTSIGNATURE ntmFontSig;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NEWTEXTMETRIC
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
            public uint ntmFlags;
            public uint ntmSizeEM;
            public uint ntmCellHeight;
            public uint ntmAvgWidth;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FONTSIGNATURE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] fsUsb;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] fsCsb;
        }

        private const int DEFAULT_CHARSET = 1;
        private const uint TRUETYPE_FONTTYPE = 0x0004;

        #endregion

        private static List<string> _cachedFontFamilies;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets all available font families on the system
        /// </summary>
        /// <param name="includeSymbolFonts">Whether to include symbol/decorative fonts</param>
        /// <returns>List of font family names sorted alphabetically</returns>
        public static List<string> GetAvailableFontFamilies(bool includeSymbolFonts = false)
        {
            lock (_lockObject)
            {
                if (_cachedFontFamilies != null)
                {
                    return new List<string>(_cachedFontFamilies);
                }

                var fontFamilies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    IntPtr hdc = GetDC(IntPtr.Zero);
                    if (hdc != IntPtr.Zero)
                    {
                        var logFont = new LOGFONT
                        {
                            lfCharSet = DEFAULT_CHARSET,
                            lfFaceName = string.Empty
                        };

                        EnumFontFamExProc callback = (ref ENUMLOGFONTEX lpelfe, ref NEWTEXTMETRICEX lpntme, uint FontType, IntPtr lParam) =>
                        {
                            var fontName = lpelfe.elfLogFont.lfFaceName;

                            if (!string.IsNullOrWhiteSpace(fontName) && !fontName.StartsWith("@"))
                            {
                                // Filter fonts with empty names or non-ASCII characters
                                if (string.IsNullOrEmpty(fontName) || !IsAsciiFont(fontName))
                                {
                                    return 1; // Continue enumeration
                                }

                                // Skip symbol fonts if not requested
                                if (!includeSymbolFonts && IsSymbolFont(fontName))
                                {
                                    return 1; // Continue enumeration
                                }

                                // Skip fonts that start with a dot (usually system fonts)
                                if (!fontName.StartsWith("."))
                                {
                                    fontFamilies.Add(fontName);
                                }
                            }

                            return 1; // Continue enumeration
                        };

                        EnumFontFamiliesEx(hdc, ref logFont, callback, IntPtr.Zero, 0);
                        ReleaseDC(IntPtr.Zero, hdc);
                    }
                }
                catch (Exception)
                {
                    // If PInvoke fails, fall back to a basic set of common fonts
                    fontFamilies = GetFallbackFontFamilies();
                }

                _cachedFontFamilies = fontFamilies.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
                return new List<string>(_cachedFontFamilies);
            }
        }

        /// <summary>
        /// Gets a filtered list of recommended fonts for UI use
        /// </summary>
        /// <returns>List of UI-friendly font family names</returns>
        public static List<string> GetRecommendedFontFamilies()
        {
            var allFonts = GetAvailableFontFamilies(includeSymbolFonts: false);
            var recommendedFonts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Always include these common UI fonts if available
            var priorityFonts = new[]
            {
                "Segoe UI", "Arial", "Calibri", "Trebuchet MS", "Verdana", "Tahoma",
                "Times New Roman", "Georgia", "Cambria", "Century Gothic",
                "Franklin Gothic Medium", "Lucida Sans Unicode", "MS Sans Serif"
            };

            foreach (var font in priorityFonts)
            {
                if (allFonts.Contains(font))
                {
                    recommendedFonts.Add(font);
                }
            }

            // Add other non-symbol, readable fonts
            var readableFontKeywords = new[] { "Sans", "Serif", "UI", "Text", "Display" };
            foreach (var font in allFonts.Take(100)) // Limit to first 100 to avoid overwhelming list
            {
                if (!recommendedFonts.Contains(font) &&
                    (readableFontKeywords.Any(keyword => font.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                     IsLikelyReadableFont(font)))
                {
                    recommendedFonts.Add(font);
                }
            }

            return recommendedFonts.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// Checks if a font family name exists on the system
        /// </summary>
        /// <param name="fontFamilyName">Font family name to check</param>
        /// <returns>True if font exists</returns>
        public static bool IsFontFamilyAvailable(string fontFamilyName)
        {
            if (string.IsNullOrWhiteSpace(fontFamilyName))
                return false;

            var availableFonts = GetAvailableFontFamilies(includeSymbolFonts: true);
            return availableFonts.Contains(fontFamilyName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the best available font from a list of preferred fonts
        /// </summary>
        /// <param name="preferredFonts">List of preferred font names in order of preference</param>
        /// <returns>First available font from the list, or "Segoe UI" as fallback</returns>
        public static string GetBestAvailableFont(params string[] preferredFonts)
        {
            foreach (var font in preferredFonts)
            {
                if (IsFontFamilyAvailable(font))
                {
                    return font;
                }
            }

            // Fallback to system default
            return IsFontFamilyAvailable("Segoe UI") ? "Segoe UI" : "Arial";
        }

        /// <summary>
        /// Clears the cached font list, forcing re-enumeration on next access
        /// </summary>
        public static void ClearCache()
        {
            lock (_lockObject)
            {
                _cachedFontFamilies = null;
            }
        }

        private static bool IsSymbolFont(string fontName)
        {
            var symbolFontNames = new[]
            {
                "Symbol", "Webdings", "Wingdings", "Zapf Dingbats", "Marlett",
                "MS Outlook", "MT Extra", "Monotype Sorts"
            };

            return symbolFontNames.Any(symbolFont =>
                fontName.Contains(symbolFont, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsLikelyReadableFont(string fontName)
        {
            // Simple heuristics to identify readable fonts
            if (fontName.Length < 3 || fontName.Length > 30)
                return false;

            // Exclude fonts with numbers only or special characters
            if (fontName.All(char.IsDigit) ||
                fontName.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '.'))
                return false;

            // Exclude known problematic patterns
            var excludePatterns = new[] { "MT ", "MS ", "@", "HoloLens" };
            if (excludePatterns.Any(pattern => fontName.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        private static bool IsAsciiFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                return false;

            // Check if all characters in the font name are ASCII
            return fontName.All(c => c >= 32 && c <= 126);
        }

        private static HashSet<string> GetFallbackFontFamilies()
        {
            // Fallback list if PInvoke fails
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Segoe UI", "Arial", "Times New Roman", "Calibri", "Trebuchet MS",
                "Verdana", "Georgia", "Comic Sans MS", "Impact", "Lucida Console",
                "Tahoma", "Courier New", "Consolas", "Cambria", "Century Gothic",
                "Franklin Gothic Medium", "Garamond", "Book Antiqua", "Arial Black",
                "Palatino Linotype", "MS Sans Serif", "MS Serif"
            };
        }
    }
}