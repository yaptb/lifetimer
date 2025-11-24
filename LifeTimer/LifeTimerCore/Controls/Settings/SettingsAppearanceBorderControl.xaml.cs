using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using LifeTimer.Logic;
using LifeTimer.Logic.Models;
using LifeTimer.Controls.Settings.Components;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsAppearanceBorderControl : UserControl
    {
        private readonly ILogger<SettingsAppearanceBorderControl> _logger;
        private readonly ApplicationController _applicationController;

        private AppearanceViewModel _appearanceViewModel;

        public SettingsAppearanceBorderControl()
        {
            this.InitializeComponent();

            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsAppearanceBorderControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            _appearanceViewModel = _applicationController.CurrentSettings.Appearance;

            UpdateUI();
            _logger.LogInformation("SettingsAppearanceBorderControl initialized");
        }

        private void UpdateUI()
        {
            if (_appearanceViewModel == null) return;

            // Update border color preview
            BorderColorBrush.Color = _appearanceViewModel.BorderColor;

            // Update Border Thickness
            BorderThicknessField.Value = _appearanceViewModel.BorderThickness;

            // Update Border Radius
            BorderRadiusField.Value = _appearanceViewModel.BorderRadius;
        }

        private async void BorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Select Border Color",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Apply",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var colorPicker = new SettingsColorPickerControl
                {
                    SelectedColor = _appearanceViewModel?.BorderColor ?? Colors.White
                };

                dialog.Content = colorPicker;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (_appearanceViewModel != null)
                    {
                        _appearanceViewModel.BorderColor = colorPicker.SelectedColor;
                        BorderColorBrush.Color = colorPicker.SelectedColor;
                        UpdateUI();
                        ProcessAppearanceChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting border color");
            }
        }

        private void BorderThicknessField_ValueChanged(object sender, double e)
        {
            if (_appearanceViewModel != null)
            {
                _appearanceViewModel.BorderThickness = (int)e;
                ProcessAppearanceChanged();
            }
        }

        private void BorderRadiusField_ValueChanged(object sender, double e)
        {
            if (_appearanceViewModel != null)
            {
                _appearanceViewModel.BorderRadius = (int)e;
                ProcessAppearanceChanged();
            }
        }

        private void ProcessAppearanceChanged()
        {
            _applicationController.RequestChangeMainWindowAppearance(_appearanceViewModel);
        }
    }
}
