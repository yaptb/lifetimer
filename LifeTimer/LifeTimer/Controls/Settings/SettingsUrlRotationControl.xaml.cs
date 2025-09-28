using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsUrlRotationControl : UserControl
    {
        private readonly ILogger<SettingsUrlRotationControl> _logger;
        private readonly ApplicationController _applicationController;

        public SettingsUrlRotationControl()
        {
            this.InitializeComponent();
            
            _logger = App.Services.GetRequiredService<ILogger<SettingsUrlRotationControl>>();
            _applicationController = App.Services.GetRequiredService<ApplicationController>();
            
            _applicationController.NotifySettingsChange += Instance_NotifySettingsChange;
            UpdateControl();
            _logger.LogDebug("SettingsUrlRotationControl initialized");
        }

        private void Instance_NotifySettingsChange(object? sender, EventArgs e)
        {
            UpdateControl();

        }


        void UpdateControl()
        {
            this.RotationToggle.IsToggled = _applicationController.CurrentSettings.RotateLinks;
            this.RotationDelay.Value = _applicationController.CurrentSettings.LinkRotationDelaySecs;

            if (_applicationController.IsLinkRotationDisabled)
            {
                LinkRotationDisabledUI.Visibility = Visibility.Visible;
                LinkRotationEnabledUI.Visibility = Visibility.Collapsed;
            }
            else
            {
                LinkRotationDisabledUI.Visibility = Visibility.Collapsed;
                LinkRotationEnabledUI.Visibility = Visibility.Visible;
            }
        }

        private void RotationDelay_OnValueChanged(object sender, double e)
        {
            int delaySecs = Convert.ToInt32(this.RotationDelay.Value);
            _logger.LogInformation("User changed link rotation delay to {DelaySecs} seconds", delaySecs);
            _applicationController.RequestChangeLinkRotationDelay(delaySecs);
        }

        private void RotationToggle_ToggledChanged(object sender, bool e)
        {
            bool isToggled = RotationToggle.IsToggled;
            _logger.LogInformation("User {Action} link rotation", isToggled ? "enabled" : "disabled");
            _applicationController.RequestChangeLinkRotationStatus(isToggled);
        }
    }

}
