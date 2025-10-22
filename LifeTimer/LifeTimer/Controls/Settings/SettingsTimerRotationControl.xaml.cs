using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsTimerRotationControl : UserControl
    {
        private readonly ILogger<SettingsTimerRotationControl> _logger;
        private readonly ApplicationController _applicationController;
        private bool _ignoreToggleChanges = false;

        public SettingsTimerRotationControl()
        {
            this.InitializeComponent();
            
            _logger = App.Services.GetRequiredService<ILogger<SettingsTimerRotationControl>>();
            _applicationController = App.Services.GetRequiredService<ApplicationController>();
            
            //_applicationController.NotifySettingsChange += Instance_NotifySettingsChange;
            _applicationController.NotifyLinkRotationStatusChange += _applicationController_NotifyLinkRotationStatusChange;
            UpdateControl();
            _logger.LogDebug("SettingsTimerRotationControl initialized");
        }

        private void _applicationController_NotifyLinkRotationStatusChange(object? sender, string e)
        {
            UpdateControl();
        }

        private void Instance_NotifySettingsChange(object? sender, EventArgs e)
        {
            UpdateControl();
        }


        void UpdateControl()
        {
            _ignoreToggleChanges = true;

            this.RotationToggle.IsToggled = _applicationController.CurrentSettings.RotateTimers;
            this.RotationDelay.Value = _applicationController.CurrentSettings.TimerRotationDelaySecs;

            if (_applicationController.IsTimerRotationDisabled)
            {
                LinkRotationDisabledUI.Visibility = Visibility.Visible;
                LinkRotationEnabledUI.Visibility = Visibility.Collapsed;
            }
            else
            {
                LinkRotationDisabledUI.Visibility = Visibility.Collapsed;
                LinkRotationEnabledUI.Visibility = Visibility.Visible;
            }

            _ignoreToggleChanges = false;
        }



        private void RotationDelay_OnValueChanged(object sender, double e)
        {
            int delaySecs = Convert.ToInt32(this.RotationDelay.Value);
            _logger.LogInformation("User changed link rotation delay to {DelaySecs} seconds", delaySecs);
            _applicationController.RequestChangeLinkRotationDelay(delaySecs);
        }

        private void RotationToggle_ToggledChanged(object sender, bool e)
        {
            if (_ignoreToggleChanges)
                return;

            bool isToggled = RotationToggle.IsToggled;
            _logger.LogInformation("User {Action} link rotation", isToggled ? "enabled" : "disabled");
            _applicationController.RequestChangeLinkRotationStatus(isToggled);
        }
    }

}
