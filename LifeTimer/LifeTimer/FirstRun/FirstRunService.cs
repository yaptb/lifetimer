using LifeTimer.Logic;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Bluetooth.Advertisement;

namespace LifeTimer.FirstRun
{

    public class WizardStep
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string StepText { get; set; }
    }


    public class FirstRunService
    {

        private readonly bool ForceFirstRun =false;
        public const int FirstRunVersion = 1015;


        private SettingsManager _settingsManager;
        private ILogger<FirstRunService> _logger;


        public FirstRunService(SettingsManager settingsManager, ILogger<FirstRunService> logger) {
            _settingsManager = settingsManager;
            _logger = logger;
        }



        public bool CheckIsFirstRun()
        {


#if DEBUG
            if(ForceFirstRun)
            {
                _logger.LogWarning("**** DEBUG Forcing First Run Display");
                return true;
            }
#endif

            var result = false;
            var first_run_value = _settingsManager.GetFirstRunVersion();

            if (first_run_value == null || first_run_value < FirstRunVersion)
                result = true;

            _logger.LogDebug("CheckIsFirstRun(): " + result);

            return result;
        }


        public void CancelFirstRun()
        {
            _settingsManager.SetFirstRunVersion(FirstRunVersion);
        }

        public void ResetFirstRun()
        {
            _settingsManager.SetFirstRunVersion(null);
        }


        public void ConfigureFirstRunWizard(ObservableCollection<WizardStep> steps, string title)
        {
            _model = new FirstRunViewModel(this,steps, title);
        }


        public FirstRunViewModel GetFirstRunViewModel()
        {
            if (_model == null)
                throw new System.InvalidOperationException("First Run Wizard Not Configured");

            return _model;
        }


        private FirstRunViewModel _model { get; set; }
        

    }

}
