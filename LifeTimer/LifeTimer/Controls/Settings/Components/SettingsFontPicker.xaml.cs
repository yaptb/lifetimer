using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using LifeTimer.Helpers;
using LifeTimer.Logic.Models;
using Windows.UI.Text;

namespace LifeTimer.Controls.Settings.Components
{
    // Helper classes for ComboBox items
    public class FontWeightItem
    {
        public string DisplayName { get; set; }
        public FontWeight FontWeight { get; set; }

        public FontWeightItem(string displayName, FontWeight fontWeight)
        {
            DisplayName = displayName;
            FontWeight = fontWeight;
        }

        public override string ToString() => DisplayName;
    }

    public class FontStyleItem
    {
        public string DisplayName { get; set; }
        public FontStyle FontStyle { get; set; }

        public FontStyleItem(string displayName, FontStyle fontStyle)
        {
            DisplayName = displayName;
            FontStyle = fontStyle;
        }

        public override string ToString() => DisplayName;
    }

    public sealed partial class SettingsFontPicker : UserControl
    {
        private const string DefaultPreviewText = "The quick brown fox jumps over the lazy dog";
        private bool _isInitializing = false;

   

        public static readonly DependencyProperty FontDefinitionProperty =
            DependencyProperty.Register(
                nameof(FontDefinition),
                typeof(FontDefinitionViewModel),
                typeof(SettingsFontPicker),
                new PropertyMetadata(null, OnFontDefinitionChanged));

        public static readonly DependencyProperty PreviewTextProperty =
            DependencyProperty.Register(
                nameof(PreviewText),
                typeof(string),
                typeof(SettingsFontPicker),
                new PropertyMetadata(DefaultPreviewText, OnPreviewTextChanged));

        public event EventHandler<FontDefinitionViewModel> FontDefinitionChanged;

       

        public FontDefinitionViewModel FontDefinition
        {
            get => (FontDefinitionViewModel)GetValue(FontDefinitionProperty);
            set => SetValue(FontDefinitionProperty, value);
        }

        public string PreviewText
        {
            get => (string)GetValue(PreviewTextProperty);
            set => SetValue(PreviewTextProperty, value);
        }

        public SettingsFontPicker()
        {
            this.InitializeComponent();
            LoadAvailableFonts();
            LoadFontWeights();
            LoadFontStyles();

            // Initialize with default if no FontDefinition set
            if (FontDefinition == null)
            {
                FontDefinition = FontDefinitionViewModel.CreateDefault();
            }

            UpdateControls();
            UpdatePreview();
        }

    

        private static void OnFontDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsFontPicker control && !control._isInitializing)
            {
                control.UpdateControls();
                control.UpdatePreview();
                control.FontDefinitionChanged?.Invoke(control, e.NewValue as FontDefinitionViewModel);
            }
        }

        private static void OnPreviewTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsFontPicker control)
            {
                control.UpdatePreview();
            }
        }

        private void LoadAvailableFonts()
        {
            _isInitializing = true;

            try
            {
                // Get system fonts using FontHelper with PInvoke
                // Use recommended fonts for better UI experience
                var availableFonts = FontHelper.GetRecommendedFontFamilies();

                // Add fonts to ComboBox
                foreach (var fontName in availableFonts)
                {
                    FontComboBox.Items.Add(fontName);
                }

                // Set initial selection
                UpdateSelectedFont();
            }
            catch (Exception)
            {
                // Fallback to basic fonts if FontHelper fails
                var fallbackFonts = new List<string>
                {
                    "Segoe UI", "Arial", "Times New Roman", "Calibri", "Verdana"
                };

                foreach (var fontName in fallbackFonts)
                {
                    FontComboBox.Items.Add(fontName);
                }

                UpdateSelectedFont();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void LoadFontWeights()
        {
            FontWeightComboBox.Items.Clear();
            FontWeightComboBox.Items.Add(new FontWeightItem("Thin", FontWeights.Thin));
            FontWeightComboBox.Items.Add(new FontWeightItem("Extra Light", FontWeights.ExtraLight));
            FontWeightComboBox.Items.Add(new FontWeightItem("Light", FontWeights.Light));
            FontWeightComboBox.Items.Add(new FontWeightItem("Normal", FontWeights.Normal));
            FontWeightComboBox.Items.Add(new FontWeightItem("Medium", FontWeights.Medium));
            FontWeightComboBox.Items.Add(new FontWeightItem("Semi Bold", FontWeights.SemiBold));
            FontWeightComboBox.Items.Add(new FontWeightItem("Bold", FontWeights.Bold));
            FontWeightComboBox.Items.Add(new FontWeightItem("Extra Bold", FontWeights.ExtraBold));
            FontWeightComboBox.Items.Add(new FontWeightItem("Black", FontWeights.Black));
        }

        private void LoadFontStyles()
        {
            FontStyleComboBox.Items.Clear();
            FontStyleComboBox.Items.Add(new FontStyleItem("Normal", FontStyle.Normal));
            FontStyleComboBox.Items.Add(new FontStyleItem("Italic", FontStyle.Italic));
            FontStyleComboBox.Items.Add(new FontStyleItem("Oblique", FontStyle.Oblique));
        }

        private void UpdateControls()
        {
            if (FontDefinition == null) return;

            _isInitializing = true;
            try
            {
                // Update font family
                UpdateSelectedFont();

                // Update font size
                FontSizeNumberBox.Value = FontDefinition.FontSize;

                // Update font weight
                UpdateSelectedFontWeight();

                // Update font style
                UpdateSelectedFontStyle();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void UpdateSelectedFont()
        {
            if (FontDefinition?.FontFamilyName != null)
            {
                var fontName = FontDefinition.FontFamilyName;

                // Try to find and select the matching font in the ComboBox
                for (int i = 0; i < FontComboBox.Items.Count; i++)
                {
                    if (FontComboBox.Items[i].ToString().Equals(fontName, StringComparison.OrdinalIgnoreCase))
                    {
                        FontComboBox.SelectedIndex = i;
                        return;
                    }
                }
            }

            // Default to first item if not found
            if (FontComboBox.Items.Count > 0)
            {
                FontComboBox.SelectedIndex = 0;
            }
        }

        private void UpdateSelectedFontWeight()
        {
            if (FontDefinition?.FontWeight != null)
            {
                for (int i = 0; i < FontWeightComboBox.Items.Count; i++)
                {
                    if (FontWeightComboBox.Items[i] is FontWeightItem item &&
                        item.FontWeight.Weight == FontDefinition.FontWeight.Weight)
                    {
                        FontWeightComboBox.SelectedIndex = i;
                        return;
                    }
                }
            }

            // Default to Normal
            FontWeightComboBox.SelectedIndex = 3; // Normal
        }

        private void UpdateSelectedFontStyle()
        {
            if (FontDefinition != null)
            {
                for (int i = 0; i < FontStyleComboBox.Items.Count; i++)
                {
                    if (FontStyleComboBox.Items[i] is FontStyleItem item &&
                        item.FontStyle == FontDefinition.FontStyle)
                    {
                        FontStyleComboBox.SelectedIndex = i;
                        return;
                    }
                }
            }

            // Default to Normal
            FontStyleComboBox.SelectedIndex = 0; // Normal
        }

        private void UpdatePreview()
        {
            if (FontDefinition != null)
            {
                // Update preview text font
                PreviewTextBlock.FontFamily = FontDefinition.GetWinUIFontFamily();
                PreviewTextBlock.FontSize = FontDefinition.FontSize;
                PreviewTextBlock.FontWeight = FontDefinition.FontWeight;
                PreviewTextBlock.FontStyle = FontDefinition.FontStyle;
                PreviewTextBlock.Text = string.IsNullOrWhiteSpace(PreviewText) ? DefaultPreviewText : PreviewText;

                // Update font info
                FontInfoTextBlock.Text = $"Font: {FontDefinition.FontFamilyName}, {FontDefinition.FontSize}pt";
            }
        }

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || FontDefinition == null) return;

            if (FontComboBox.SelectedItem is string selectedFontName)
            {
                try
                {
                    FontDefinition.FontFamilyName = selectedFontName;
                    UpdatePreview();
                    FontDefinitionChanged?.Invoke(this, FontDefinition);
                }
                catch (Exception)
                {
                    // If font creation fails, keep the current selection
                }
            }
        }

        private void FontSizeNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (_isInitializing || FontDefinition == null) return;

            FontDefinition.FontSize = Math.Max(6, Math.Min(200, sender.Value));
            UpdatePreview();
            FontDefinitionChanged?.Invoke(this, FontDefinition);
        }

        private void FontWeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || FontDefinition == null) return;

            if (FontWeightComboBox.SelectedItem is FontWeightItem selectedItem)
            {
                FontDefinition.FontWeight = selectedItem.FontWeight;
                UpdatePreview();
                FontDefinitionChanged?.Invoke(this, FontDefinition);
            }
        }

        private void FontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || FontDefinition == null) return;

            if (FontStyleComboBox.SelectedItem is FontStyleItem selectedItem)
            {
                FontDefinition.FontStyle = selectedItem.FontStyle;
                UpdatePreview();
                FontDefinitionChanged?.Invoke(this, FontDefinition);
            }
        }


        // Helper method to set font definition
        public void SetFontDefinition(FontDefinitionViewModel fontDefinition)
        {
            if (fontDefinition != null)
            {
                FontDefinition = fontDefinition;
            }
        }

        // Helper method to get font definition
        public FontDefinitionViewModel GetFontDefinition()
        {
            return FontDefinition ?? FontDefinitionViewModel.CreateDefault();
        }

        // Helper method to set font by name (for backwards compatibility)
        public void SetFontFamily(string fontName)
        {
            if (!string.IsNullOrWhiteSpace(fontName) && FontDefinition != null)
            {
                try
                {
                    // Validate font exists using FontHelper
                    if (FontHelper.IsFontFamilyAvailable(fontName))
                    {
                        FontDefinition.FontFamilyName = fontName;
                    }
                    else
                    {
                        // Try to find a similar font or fallback
                        var bestFont = FontHelper.GetBestAvailableFont(fontName, "Segoe UI", "Arial");
                        FontDefinition.FontFamilyName = bestFont;
                    }
                    UpdateControls();
                    UpdatePreview();
                }
                catch (Exception)
                {
                    // If font creation fails, keep current selection
                }
            }
        }

        // Helper method to get font name
        public string GetFontFamilyName()
        {
            return FontDefinition?.FontFamilyName ?? "Segoe UI";
        }
    }
}