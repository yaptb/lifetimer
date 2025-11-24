using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsStatusControl : UserControl
    {
        private readonly ILogger<SettingsStatusControl> _logger;
        private readonly ApplicationController _applicationController;

        public SettingsStatusControl()
        {
            InitializeComponent();
            
            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsStatusControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            SetSettingsStatus(_applicationController.LastSettingsStatus);
            SetBrowserStatus(_applicationController.LastBrowserStatus);
            SetLinkRotationStatus(_applicationController.LastRotationStatus);

            _applicationController.NotifySettingsStatusChange += Instance_NotifySettingsStatusChange;
            _applicationController.NotifyTimerStatusChange += Instance_NotifyBrowserStatusChange;
            _applicationController.NotifyLinkRotationStatusChange += Instance_NotifyLinkRotationStatusChange;
            _applicationController.NotifyLinkRotationTimerChange += Instance_NotifyLinkRotationTimerChange;
            
            _logger.LogDebug("SettingsStatusControl initialized");
        }

        private void Instance_NotifyLinkRotationTimerChange(object? sender, string e)
        {
            SetLinkTimerStatus(e);
        }

        private void Instance_NotifySettingsStatusChange(object? sender, string e)
        {
            SetSettingsStatus(e);
        }


        private void Instance_NotifyBrowserStatusChange(object? sender, string e)
        {
            SetBrowserStatus(e);
        }

        private void Instance_NotifyLinkRotationStatusChange(object? sender, string e)
        {
            SetLinkRotationStatus(e);

        }



        private void SetSettingsStatus(string s)
        {
            SettingsStatus.Text = s;

        }

        private void SetBrowserStatus(string s)
        {

            if (s == null)
                return;

            if (s.Length > 200)
            {
                s = s.Substring(0, 200);
            }

            BrowserStatus.Text = s;

        }

        private void SetLinkRotationStatus(string s)
        {
            RotationStatus.Text = s;
        }


        private void SetLinkTimerStatus(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                RotationStatus.Text = _applicationController.LastRotationStatus+" ("+s+")";
            }
        }
    }
}
