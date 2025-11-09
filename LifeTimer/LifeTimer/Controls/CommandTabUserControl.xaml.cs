using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LifeTimer.Logic;

namespace LifeTimer.Controls
{
    public sealed partial class CommandTabUserControl : UserControl
    {
        private readonly ILogger<CommandTabUserControl> _logger;
        private readonly ApplicationController _applicationController;
        private bool _ignoreToggleEvents =false;

        public CommandTabUserControl()
        {
            InitializeComponent();
            
            _logger = AppManager.Services.GetRequiredService<ILogger<CommandTabUserControl>>();
            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();
            _applicationController.NotifyInteractionModeChange += _applicationController_NotifyInteractionModeChange;
            
            Loaded += CommandTabUserControl_Loaded;
            _logger.LogDebug("CommandTabUserControl initialized");
        }

        private void _applicationController_NotifyInteractionModeChange(object? sender, System.EventArgs e)
        {
            UpdateState();
        }


        private void CommandTabUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateState();
        }


        private void UpdateState()
        {
            _ignoreToggleEvents = true;

            if (_applicationController.IsInteractiveMode)
            {
                this.OperatingMode.IsToggled = true;
            }
            else
            {
                this.OperatingMode.IsToggled = false;
            }

            _ignoreToggleEvents=false;
        }

   


        private void OperatingMode_ToggledChanged(object sender, bool e)
        {
           

            if (_ignoreToggleEvents)
                return;

            var toggled = this.OperatingMode.IsToggled;

            if (toggled)
            {
                _logger.LogInformation("User requested interactive mode");
                _applicationController.RequestInteractiveMode();
                UpdateState();
            }
            else
            {
                _logger.LogInformation("User requested background mode");
                _applicationController.RequestBackgroundMode();
                UpdateState();
            }
        }



        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("User requested save settings");
            _applicationController.RequestSaveSettings();
            UpdateState();
        }

        private void ApplicationExit_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("User requested application exit");
            _applicationController.RequestApplicationExit();
        }


    }
}
