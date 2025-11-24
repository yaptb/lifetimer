using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace LifeTimer.Logic.Models
{
    public class FontDefinitionViewModel
    {
        public string FontFamilyName { get; set; } = "Segoe UI";
        public FontWeight FontWeight { get; set; } = Microsoft.UI.Text.FontWeights.Medium;
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;
        public double FontSize { get; set; } = 14.0;

        public FontFamily GetWinUIFontFamily()
        {
            return new FontFamily(FontFamilyName);
        }

        public static FontDefinitionViewModel CreateDefault(string fontFamily = "Segoe UI",
            double fontSize = 14.0)
        {
            return new FontDefinitionViewModel
            {
                FontFamilyName = fontFamily,
                FontSize = fontSize,
                FontWeight = FontWeights.Normal,
                FontStyle = FontStyle.Normal
            };
        }
    }
}
