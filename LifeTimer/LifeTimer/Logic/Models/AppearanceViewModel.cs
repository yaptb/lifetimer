using Windows.UI;

namespace LifeTimer.Logic.Models
{
    public class AppearanceViewModel
    {
        public FontDefinitionViewModel TitleFontDefinition { get; set; } = new FontDefinitionViewModel();
        public FontDefinitionViewModel TimerFontDefinition { get; set; } = new FontDefinitionViewModel();

        public Color ForegroundColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color BorderColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public int BorderThickness { get; set; } = 1;
        public int BorderRadius { get; set; } = 20;

        public static AppearanceViewModel CreateDefaultAppearance()
        {
            var model = new AppearanceViewModel
            {
                TitleFontDefinition = FontDefinitionViewModel.CreateDefault("Segoe UI", 18),
                TimerFontDefinition = FontDefinitionViewModel.CreateDefault("Segoe UI", 40),
                ForegroundColor = Color.FromArgb(255, 255, 255, 255),
                BackgroundColor = Color.FromArgb(255, 0, 0, 0),
            };
            return model;
        }
    }
}
