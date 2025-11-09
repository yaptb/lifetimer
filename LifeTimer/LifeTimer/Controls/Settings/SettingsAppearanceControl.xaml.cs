using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using LifeTimer.Logic;
using LifeTimer.Logic.Models;
using LifeTimer.Controls.Settings.Components;
using Microsoft.UI;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsAppearanceControl : UserControl
    {
        private readonly ILogger<SettingsAppearanceControl> _logger;
        private readonly ApplicationController _applicationController;

        private AppearanceViewModel _appearanceViewModel;

        private const int PreviewTitleFontSize = 14;
        private const int PreviewTimerFontSize = 24;


        public SettingsAppearanceControl()
        {
            this.InitializeComponent();

            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsAppearanceControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            _appearanceViewModel = _applicationController.CurrentSettings.Appearance;

            InitializeControls();
            UpdateUI();
            _logger.LogInformation("SettingsAppearanceUserControl initialized");
        }


        private void InitializeControls()
        {
           
        }

        private void UpdateUI()
        {
            if (_appearanceViewModel == null) return;

            // Update color previews
            ForegroundColorBrush.Color = _appearanceViewModel.ForegroundColor;
            BackgroundColorBrush.Color = _appearanceViewModel.BackgroundColor;

            // Update font previews
            UpdateFontPreviews();

        }

        private void UpdateFontPreviews()
        {
            if (_appearanceViewModel == null) return;

            // Update title font preview
            if (_appearanceViewModel.TitleFontDefinition != null)
            {
                TitleFontPreview.FontFamily = _appearanceViewModel.TitleFontDefinition.GetWinUIFontFamily();
                TitleFontPreview.FontSize = PreviewTitleFontSize;
                TitleFontPreview.FontWeight = _appearanceViewModel.TitleFontDefinition.FontWeight;
                TitleFontPreview.FontStyle = _appearanceViewModel.TitleFontDefinition.FontStyle;
            }

            // Update timer font preview
            if (_appearanceViewModel.TimerFontDefinition != null)
            {
                TimerFontPreview.FontFamily = _appearanceViewModel.TimerFontDefinition.GetWinUIFontFamily();
                TimerFontPreview.FontSize = PreviewTimerFontSize;
                TimerFontPreview.FontWeight = _appearanceViewModel.TimerFontDefinition.FontWeight;
                TimerFontPreview.FontStyle = _appearanceViewModel.TimerFontDefinition.FontStyle;
            }
        }


        private async void TitleFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Configure Title Font",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Apply",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var fontPicker = new SettingsFontPicker
                {
                    FontDefinition = _appearanceViewModel?.TitleFontDefinition ?? FontDefinitionViewModel.CreateDefault("Segoe UI", 18),
                    PreviewText = "Sample Title Text"
                };

                dialog.Content = fontPicker;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (_appearanceViewModel != null)
                    {
                        _appearanceViewModel.TitleFontDefinition = fontPicker.GetFontDefinition();
                        UpdateUI();
                        ProcessAppearanceChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring title font");
            }
        }

        private async void TimerFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Configure Timer Font",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Apply",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var fontPicker = new SettingsFontPicker
                {
                    FontDefinition = _appearanceViewModel?.TimerFontDefinition ?? FontDefinitionViewModel.CreateDefault("Segoe UI", 48),
                    PreviewText = "00:15:30"
                };

                dialog.Content = fontPicker;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (_appearanceViewModel != null)
                    {
                        _appearanceViewModel.TimerFontDefinition = fontPicker.GetFontDefinition();
                        UpdateUI();
                        ProcessAppearanceChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring timer font");
            }
        }

        private async void ForegroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Select Text Color",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Apply",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var colorPicker = new SettingsColorPickerControl
                {
                    SelectedColor = _appearanceViewModel?.ForegroundColor ?? Colors.White
                };

                dialog.Content = colorPicker;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (_appearanceViewModel != null)
                    {
                        _appearanceViewModel.ForegroundColor = colorPicker.SelectedColor;
                        ForegroundColorBrush.Color = colorPicker.SelectedColor;
                        UpdateUI();
                        ProcessAppearanceChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting foreground color");
            }
        }

        private async void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Timer Color",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Apply",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var colorPicker = new SettingsColorPickerControl
                {
                    SelectedColor = _appearanceViewModel?.BackgroundColor ?? Colors.Black
                };

                dialog.Content = colorPicker;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (_appearanceViewModel != null)
                    {
                        _appearanceViewModel.BackgroundColor = colorPicker.SelectedColor;
                        BackgroundColorBrush.Color = colorPicker.SelectedColor;
                        UpdateUI();
                        ProcessAppearanceChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting background color");
            }
        }

   

        private void ProcessAppearanceChanged()
        {
            _applicationController.RequestChangeMainWindowAppearance(this._appearanceViewModel);
        }


    }
}