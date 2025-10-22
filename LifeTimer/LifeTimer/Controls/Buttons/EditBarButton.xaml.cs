using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.Controls.Buttons
{
    public sealed partial class EditBarButton : UserControl
    {

        public EditBarButton()
        {
            InitializeComponent();
        }

        // Glyph property
        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(EditBarButton), new PropertyMetadata("\uE10F"));



        // Scale property
        public double ButtonScale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(EditBarButton), new PropertyMetadata(1.0));



        // Click event
        public event RoutedEventHandler Click;

        private void PART_Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

    }
}
