using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsPositionControl : UserControl
    {
        private readonly ILogger<SettingsPositionControl> _logger;
        private readonly ApplicationController _applicationController;

        public SettingsPositionControl()
        {
            InitializeComponent();
            
            _logger = App.Services.GetRequiredService<ILogger<SettingsPositionControl>>();
            _applicationController = App.Services.GetRequiredService<ApplicationController>();

            UpdateBoundsDisplay();
            _applicationController.NotifyBrowserBoundsChange += Instance_NotifyBrowserBoundsChange;
            _logger.LogDebug("SettingsPositionControl initialized");
        }

        private void Instance_NotifyBrowserBoundsChange(object? sender, EventArgs e)
        {
            UpdateBoundsDisplay();
        }

        void UpdateBoundsDisplay()
        {
            this.XPos.Text = _applicationController.CurrentSettings?.WindowPosX.ToString();
            this.YPos.Text = _applicationController.CurrentSettings?.WindowPosY.ToString();
            this.Width.Text = _applicationController.CurrentSettings?.WindowWidth.ToString();
            this.Height.Text = _applicationController.CurrentSettings?.WindowHeight.ToString();
        }
    }
}
