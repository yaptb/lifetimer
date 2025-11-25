using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LifeTimer.FirstRun
{


    public class WizardStep
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string StepText { get; set; }
    }


    public class FirstRunWizardDefinition
    {
        public ObservableCollection<WizardStep> WizardSteps { get; set; } = new ObservableCollection<WizardStep>();
        public string Title { get; set; }
        public bool ShowCTAScreen { get;set; } //true if we want to show the CTA screen after the wizard steps
    }



    public class FirstRunService
    {

        private readonly bool ForceFirstRun = false;
        public const int FirstRunVersion = 1016;


        private ApplicationController _appController;
        private SettingsManager _settingsManager;
        private ILogger<FirstRunService> _logger;
        private FirstRunWizardDefinition _definition { get; set; }
        private FirstRunViewModel _model { get; set; }




        public FirstRunService(SettingsManager settingsManager, ILogger<FirstRunService> logger) {
            _appController = AppManager.Services.GetRequiredService<ApplicationController>();
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


        public void ConfigureFirstRunWizard(FirstRunWizardDefinition definition)
        {
            _definition = definition;
        }




        public FirstRunViewModel GetFirstRunViewModel()
        {
            if (_definition == null)
                throw new System.InvalidOperationException("First Run Wizard Not Configured");

            if (_model == null)
            {
                //lazy init view model so that we can determine which product version we have
                var isFreeVersion = _appController.CheckIsFreeVersion();
                _model = new FirstRunViewModel(_definition, isFreeVersion);
            }

            return _model;
        }


        

    }

}
