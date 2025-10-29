using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Globalization;
using Windows.UI;
using LifeTimer.Helpers;

namespace LifeTimer.Controls.Settings.Components
{
    public sealed partial class SettingsColorPickerControl : UserControl
    {
        private bool _isUpdating = false;
        private bool _isHueDragging = false;
        private bool _isSvDragging = false;
        private bool _isAlphaDragging = false;

        // HSV values
        private double _currentHue = 0;
        private double _currentSaturation = 1;
        private double _currentValue = 1;
        private double _currentAlpha = 1;


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(SettingsColorPickerControl),
                new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

        public event EventHandler<Color> ColorChanged;



        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public SettingsColorPickerControl()
        {
            this.InitializeComponent();

            // Ensure UI is updated after control is fully loaded
            this.Loaded += (s, e) =>
            {
                var color = SelectedColor;
                UpdateUI(color);
                UpdateSelectorPositions();
            };
        }



        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsColorPickerControl control && e.NewValue is Color color)
            {
                control.UpdateUI(color);
                control.ColorChanged?.Invoke(control, color);
            }
        }

        private bool _updateValuesFromColor = true;

        private void UpdateUI(Color color)
        {
            if (_isUpdating) return;

            _isUpdating = true;

            try
            {
                // Update preview
                PreviewColorBrush.Color = color;

                // Update HSV values from color
                if (_updateValuesFromColor)
                {
                    var (hue, saturation, value) = ColorHelperUtil.RgbToHsv(color);
                    _currentHue = hue;
                    _currentSaturation = saturation;
                    _currentValue = value;
                    _currentAlpha = color.A / 255.0;
                }

                // Update SV panel hue color
                SvHueStop.Color = ColorHelperUtil.GetHueColor(_currentHue);

                // Update alpha gradient colors
                var currentRgb = ColorHelperUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue);
                AlphaStartStop.Color = Color.FromArgb(255, currentRgb.R, currentRgb.G, currentRgb.B);
                AlphaEndStop.Color = Color.FromArgb(0, currentRgb.R, currentRgb.G, currentRgb.B);

                // Update RGBA inputs
                RedNumberBox.Value = color.R;
                GreenNumberBox.Value = color.G;
                BlueNumberBox.Value = color.B;
                AlphaNumberBox.Value = color.A;

                // Update hex input (include alpha)
                HexColorTextBox.Text = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

                // Update selector positions
                UpdateSelectorPositions();




            }
            finally
            {
                _isUpdating = false;
            }


        }

        private void UpdateSelectorPositions()
        {
            // Update hue selector position (vertical bar)
            double hueY = (_currentHue / 360.0) * 150; // 150 is the height of the hue bar
            Canvas.SetTop(HueSelector, Math.Max(0, Math.Min(147, hueY - 1.5))); // Center the 3px tall selector
            Canvas.SetLeft(HueSelector, -1); // Fixed horizontal position

            // Update SV selector position
            double svX = _currentSaturation * 200; // 200 is the width of the SV panel
            double svY = (1 - _currentValue) * 150; // 150 is the height, inverted for Y
            Canvas.SetLeft(SvSelector, Math.Max(-6, Math.Min(194, svX - 6))); // Center the 12px wide selector
            Canvas.SetTop(SvSelector, Math.Max(-6, Math.Min(144, svY - 6))); // Center the 12px high selector

            // Update alpha selector position (vertical bar)
            double alphaY = (1 - _currentAlpha) * 150; // 150 is the height, inverted so top = opaque (alpha = 1)
            Canvas.SetTop(AlphaSelector, Math.Max(0, Math.Min(147, alphaY - 1.5))); // Center the 3px tall selector
            Canvas.SetLeft(AlphaSelector, -1); // Fixed horizontal position
        }

        private void UpdateColorFromHsv()
        {
            _updateValuesFromColor = false;

            var rgb = ColorHelperUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue);
            var newColor = Color.FromArgb((byte)(_currentAlpha * 255), rgb.R, rgb.G, rgb.B);
            SelectedColor = newColor;

            _updateValuesFromColor = true;
        }

        // Hue bar pointer events
        private void HueCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isHueDragging = true;
                canvas.CapturePointer(e.Pointer);
                UpdateHueFromPosition(e.GetCurrentPoint(canvas).Position.Y);
            }
        }

        private void HueCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isHueDragging && sender is Canvas canvas)
            {
                UpdateHueFromPosition(e.GetCurrentPoint(canvas).Position.Y);
            }
        }

        private void HueCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isHueDragging = false;
                canvas.ReleasePointerCapture(e.Pointer);
            }
        }

        // SV panel pointer events
        private void SvCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isSvDragging = true;
                canvas.CapturePointer(e.Pointer);
                var position = e.GetCurrentPoint(canvas).Position;
                UpdateSvFromPosition(position.X, position.Y);
            }
        }

        private void SvCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isSvDragging && sender is Canvas canvas)
            {
                var position = e.GetCurrentPoint(canvas).Position;
                UpdateSvFromPosition(position.X, position.Y);
            }
        }

        private void SvCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isSvDragging = false;
                canvas.ReleasePointerCapture(e.Pointer);
            }
        }

        // Alpha bar pointer events
        private void AlphaCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isAlphaDragging = true;
                canvas.CapturePointer(e.Pointer);
                UpdateAlphaFromPosition(e.GetCurrentPoint(canvas).Position.Y);
            }
        }

        private void AlphaCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isAlphaDragging && sender is Canvas canvas)
            {
                UpdateAlphaFromPosition(e.GetCurrentPoint(canvas).Position.Y);
            }
        }

        private void AlphaCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _isAlphaDragging = false;
                canvas.ReleasePointerCapture(e.Pointer);
            }
        }

        private void UpdateHueFromPosition(double y)
        {

            // Clamp y to canvas bounds
            y = Math.Max(0, Math.Min(150, y));

            // Calculate hue (0-360)
            _currentHue = (y / 150.0) * 360.0;

            // Update SV panel hue color
            SvHueStop.Color = ColorHelperUtil.GetHueColor(_currentHue);

            // Update alpha gradient colors
            var currentRgb = ColorHelperUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue);
            AlphaStartStop.Color = Color.FromArgb(255, currentRgb.R, currentRgb.G, currentRgb.B);
            AlphaEndStop.Color = Color.FromArgb(0, currentRgb.R, currentRgb.G, currentRgb.B);

            // Update selector position
            Canvas.SetTop(HueSelector, Math.Max(0, Math.Min(147, y - 1.5)));
            Canvas.SetLeft(HueSelector, -1);

            // Update the final color
            UpdateColorFromHsv();

        }

        private void UpdateSvFromPosition(double x, double y)
        {

            // Clamp to canvas bounds
            x = Math.Max(0, Math.Min(200, x));
            y = Math.Max(0, Math.Min(150, y));

            // Calculate saturation and value
            _currentSaturation = x / 200.0;
            _currentValue = 1.0 - (y / 150.0); // Invert Y so top is bright

            // Update alpha gradient colors
            var currentRgb = ColorHelperUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue);
            AlphaStartStop.Color = Color.FromArgb(255, currentRgb.R, currentRgb.G, currentRgb.B);
            AlphaEndStop.Color = Color.FromArgb(0, currentRgb.R, currentRgb.G, currentRgb.B);

            // Update selector position
            Canvas.SetLeft(SvSelector, Math.Max(-6, Math.Min(194, x - 6)));
            Canvas.SetTop(SvSelector, Math.Max(-6, Math.Min(144, y - 6)));

            // Update the final color
            UpdateColorFromHsv();
        }

        private void UpdateAlphaFromPosition(double y)
        {
            // Clamp y to canvas bounds
            y = Math.Max(0, Math.Min(150, y));

            // Calculate alpha (0-1), inverted so top is opaque
            _currentAlpha = 1.0 - (y / 150.0);

            // Update alpha gradient colors
            var currentRgb = ColorHelperUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue);
            AlphaStartStop.Color = Color.FromArgb(255, currentRgb.R, currentRgb.G, currentRgb.B);
            AlphaEndStop.Color = Color.FromArgb(0, currentRgb.R, currentRgb.G, currentRgb.B);

            // Update selector position
            Canvas.SetTop(AlphaSelector, Math.Max(0, Math.Min(147, y - 1.5)));
            Canvas.SetLeft(AlphaSelector, -1);

            // Update the final color
            UpdateColorFromHsv();

        }

        private void RgbaNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

            try
            {
                var r = (byte)Math.Clamp(RedNumberBox.Value, 0, 255);
                var g = (byte)Math.Clamp(GreenNumberBox.Value, 0, 255);
                var b = (byte)Math.Clamp(BlueNumberBox.Value, 0, 255);
                var a = (byte)Math.Clamp(AlphaNumberBox.Value, 0, 255);

                var newColor = Color.FromArgb(a, r, g, b);
                SelectedColor = newColor;
            }
            catch (Exception)
            {
                // Invalid values, ignore
            }
        }

        private void HexColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            var text = HexColorTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                var color = ParseHexColor(text);
                SelectedColor = color;
            }
            catch (Exception)
            {
                // Invalid hex format, ignore for now
                // Could add visual feedback for invalid input
            }
        }

        private static Color ParseHexColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Invalid hex color format");

            hex = hex.Trim();

            // Remove # if present
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            // Handle different hex formats
            if (hex.Length == 6)
            {
                // RGB format (RRGGBB)
                var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                return Color.FromArgb(255, r, g, b);
            }
            else if (hex.Length == 8)
            {
                // ARGB format (AARRGGBB)
                var a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                return Color.FromArgb(a, r, g, b);
            }
            else if (hex.Length == 3)
            {
                // Short RGB format (RGB) - expand to RRGGBB
                var r = byte.Parse(hex.Substring(0, 1) + hex.Substring(0, 1), NumberStyles.HexNumber);
                var g = byte.Parse(hex.Substring(1, 1) + hex.Substring(1, 1), NumberStyles.HexNumber);
                var b = byte.Parse(hex.Substring(2, 1) + hex.Substring(2, 1), NumberStyles.HexNumber);
                return Color.FromArgb(255, r, g, b);
            }
            else
            {
                throw new ArgumentException("Invalid hex color format");
            }
        }

        // Helper method to get RGBA components as separate values
        public (byte R, byte G, byte B, byte A) GetRgbaValues()
        {
            return (SelectedColor.R, SelectedColor.G, SelectedColor.B, SelectedColor.A);
        }

        // Helper method to get RGB components as separate values (for backward compatibility)
        public (byte R, byte G, byte B) GetRgbValues()
        {
            return (SelectedColor.R, SelectedColor.G, SelectedColor.B);
        }

        // Helper method to set color from RGBA values
        public void SetRgbaColor(byte r, byte g, byte b, byte a)
        {
            SelectedColor = Color.FromArgb(a, r, g, b);
        }

        // Helper method to set color from RGB values (for backward compatibility)
        public void SetRgbColor(byte r, byte g, byte b)
        {
            SelectedColor = Color.FromArgb(255, r, g, b);
        }

        // Helper method to get hex color string (including alpha)
        public string GetHexColor()
        {
            return $"#{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        }

        // Helper method to set color from hex string
        public void SetHexColor(string hexColor)
        {
            try
            {
                SelectedColor = ParseHexColor(hexColor);
            }
            catch (Exception)
            {
                // Invalid hex color, ignore
            }
        }
    }
}