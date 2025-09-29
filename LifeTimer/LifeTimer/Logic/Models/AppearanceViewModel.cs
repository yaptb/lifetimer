using Windows.UI;

namespace LifeTimer.Logic.Models
{
    public class AppearanceViewModel
    {
        public FontDefinitionViewModel TitleFontDefinition { get; set; } = new FontDefinitionViewModel();
        public FontDefinitionViewModel TimerFontDefinition { get; set; } = new FontDefinitionViewModel();

        public Color ForegroundColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);

        public int Opacity { get; set; } = 128;

        public static AppearanceViewModel CreateDefaultAppearance()
        {
            var model = new AppearanceViewModel
            {
                TitleFontDefinition = FontDefinitionViewModel.CreateDefault("Segoe UI", 18),
                TimerFontDefinition = FontDefinitionViewModel.CreateDefault("Segoe UI", 48),
                ForegroundColor = Color.FromArgb(255, 255, 255, 255),
                BackgroundColor = Color.FromArgb(255, 0, 0, 0),
                Opacity = 128
            };
            return model;
        }
    }
}
