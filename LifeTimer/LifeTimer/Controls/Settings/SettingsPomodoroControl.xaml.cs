using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsPomodoroControl : UserControl
    {
        private readonly ILogger<SettingsPomodoroControl> _logger;
        private readonly ApplicationController _applicationController;
        private bool _ignoreToggleChanges = false;

        public SettingsPomodoroControl()
        {
            this.InitializeComponent();
            
            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsPomodoroControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            _applicationController.NotifySettingsChange += _applicationController_NotifySettingsChange;
            UpdateControl();
            _logger.LogDebug("SettingsPomodoroControl  initialized");
        }


        private void _applicationController_NotifySettingsChange(object? sender, EventArgs e)
        {
            UpdateControl();
        }


  
        void UpdateControl()
        {
            _ignoreToggleChanges = true;

              this.PomodoroMinutes.Value = _applicationController.CurrentSettings.Pomodoro.PomodoroMinutes;

            _ignoreToggleChanges = false;
        }



        private void PomodoroMinutes_OnValueChanged(object sender, double e)
        {
            if (_ignoreToggleChanges)
                return;

            int delayMinutes = Convert.ToInt32(this.PomodoroMinutes.Value);
            _applicationController.RequestChangePomodoroMinutes(delayMinutes);
        }

    }

}
