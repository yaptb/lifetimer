using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using System;
using LifeTimer.Logic;
using Windows.ApplicationModel;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsSwitchesControl : UserControl
    {
        private readonly ILogger<SettingsSwitchesControl> _logger;
        private readonly ApplicationController _applicationController;

        private const string StartupTaskID= "LifeTimerStartupId";
        private bool _ignoreToggleChanges = false;


        public SettingsSwitchesControl()
        {
            InitializeComponent();

            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsSwitchesControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            _applicationController.NotifySettingsStatusChange += _applicationController_NotifySettingsStatusChange;

            ApplyCurrentSettings();
            _logger.LogDebug("SettingsSwitchesControl initialized");
        }

        private void _applicationController_NotifySettingsStatusChange(object? sender, string e)
        {
            ApplyCurrentSettings();
        }

        private void ApplyCurrentSettings()
        {
            _ignoreToggleChanges = true;
            this.OperationHints.IsToggled = _applicationController.CurrentSettings.ShowOperationHints;
            this.SettingsStartup.IsToggled = _applicationController.CurrentSettings.ShowSettingsOnStartup;

            GetSystemStartupStatus();

            _ignoreToggleChanges = false;

        }

        private void GetSystemStartupStatus()
        {
            var toggleState = false;

            try
            {

                var task = StartupTask.GetAsync(StartupTaskID);

                task.Wait();

                var startupTask = task.GetResults();

                if (startupTask.State == StartupTaskState.Enabled)
                {
                    toggleState = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to get system startup state");
            }

            this.SystemStartup.IsToggled = toggleState;
        }


        private async void SystemStartup_ToggledChanged(object sender, bool e)
        {
            if (_ignoreToggleChanges)
                return;

            bool isToggled = this.SystemStartup.IsToggled;
            _applicationController.RequestSettingsStartInteractiveModeChange(isToggled);

            try
            {

                var startupTask = await StartupTask.GetAsync(StartupTaskID);

                if (isToggled)
                {
                    await startupTask.RequestEnableAsync();
                }
                else
                {
                    startupTask.Disable();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to set system startup state");
            }


        }


        private void OperationHints_ToggledChanged(object sender, bool e)
        {
            if (_ignoreToggleChanges)
                return;

            bool isToggled = this.OperationHints.IsToggled;
            _applicationController.RequestSettingsStartInteractiveModeChange(isToggled);

        }

        private void SettingsStartup_ToggledChanged(object sender, bool e)
        {
            if (_ignoreToggleChanges)
                return;


            bool isToggled = this.SettingsStartup.IsToggled;
            _applicationController.RequestSettingsShowSettingsOnStartup(isToggled);
        }

   

    }
}
