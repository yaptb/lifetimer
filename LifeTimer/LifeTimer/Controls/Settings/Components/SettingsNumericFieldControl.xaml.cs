using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;

namespace LifeTimer.Controls.Settings.Components
{
    public sealed partial class SettingsNumericFieldControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(SettingsNumericFieldControl),
                new PropertyMetadata("Numeric Setting", OnTitleChanged));

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(
                nameof(Subtitle),
                typeof(string),
                typeof(SettingsNumericFieldControl),
                new PropertyMetadata("Enter a numeric value", OnSubtitleChanged));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(SettingsNumericFieldControl),
                new PropertyMetadata(double.MinValue));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(SettingsNumericFieldControl),
                new PropertyMetadata(double.MaxValue));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(SettingsNumericFieldControl),
                new PropertyMetadata(0.0, OnValuePropertyChanged));

        public event EventHandler<double> OnValueChanged;

        private bool _isUpdatingText = false;
        private string _lastValidText = "0";

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public SettingsNumericFieldControl()
        {
            this.InitializeComponent();
            _lastValidText = Value.ToString();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsNumericFieldControl control)
                control.TitleTextBlock.Text = e.NewValue?.ToString();
        }

        private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsNumericFieldControl control)
                control.SubtitleTextBlock.Text = e.NewValue?.ToString();
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsNumericFieldControl control && !control._isUpdatingText)
            {
                control._isUpdatingText = true;
                control.ValueTextBox.Text = ((double)e.NewValue).ToString();
                control._lastValidText = control.ValueTextBox.Text;
                control._isUpdatingText = false;
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText) return;

            var textBox = (TextBox)sender;
            var text = textBox.Text;

            // Allow empty text temporarily during editing
            if (string.IsNullOrEmpty(text))
                return;

            // Try to parse the value
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                // Check if value is within range
                if (value >= Minimum && value <= Maximum)
                {
                    _lastValidText = text;
                    _isUpdatingText = true;
                    Value = value;
                    _isUpdatingText = false;
                    OnValueChanged?.Invoke(this, value);
                }
                else
                {
                    // Value is out of range, revert to last valid text
                    _isUpdatingText = true;
                    textBox.Text = _lastValidText;
                    textBox.SelectionStart = _lastValidText.Length;
                    _isUpdatingText = false;
                }
            }
            else
            {
                // Invalid number format, revert to last valid text
                _isUpdatingText = true;
                textBox.Text = _lastValidText;
                textBox.SelectionStart = _lastValidText.Length;
                _isUpdatingText = false;
            }
        }

        private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            
            // If text is empty on focus lost, set to current value
            if (string.IsNullOrEmpty(textBox.Text))
            {
                _isUpdatingText = true;
                textBox.Text = Value.ToString();
                _lastValidText = textBox.Text;
                _isUpdatingText = false;
            }
            else if (!double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
                     value < Minimum || value > Maximum)
            {
                // Ensure we have a valid value on focus lost
                _isUpdatingText = true;
                textBox.Text = Value.ToString();
                _lastValidText = textBox.Text;
                _isUpdatingText = false;
            }
        }
    }
}