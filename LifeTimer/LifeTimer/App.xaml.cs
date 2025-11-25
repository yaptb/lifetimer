using H.NotifyIcon;
using LifeTimer.FirstRun;
using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
//using Windows.ApplicationModel;
using Windows.Storage;

namespace LifeTimer
{
    public partial class App : Application
    {
        private AppManager _manager;


        private FirstRunWizardDefinition _firstRunWizardDefinition = new FirstRunWizardDefinition()
        {
            WizardSteps = new ObservableCollection<WizardStep>
            {
            new WizardStep { ImagePath = "ms-appx:///FirstRun/Images/interactive.gif", Description = "Double-click to interact" },
            new WizardStep {  ImagePath = "ms-appx:///FirstRun/Images/settings.gif", Description = "Use the Settings Window to configure" },
            new WizardStep { ImagePath = "ms-appx:///FirstRun/Images/pomodoro.gif", Description = "Switch between Timer and Pomodoro modes" },
            new WizardStep {  ImagePath = "ms-appx:///FirstRun/Images/timers_page.png", Description = "Use the Timers page to create and manage timers" },
            new WizardStep { ImagePath = "ms-appx:///FirstRun/Images/appearance_page.png", Description = " Use the Appearance page to configure fonts and colors" },
            new WizardStep { ImagePath = "ms-appx:///FirstRun/Images/settings_page.png", Description = "Use the Settings page to configure startup and Pomodoro options" },
            new WizardStep {  ImagePath = "ms-appx:///FirstRun/Images/help_page.png", Description = "Use the Help page to access our website" },
            new WizardStep {  ImagePath = "ms-appx:///FirstRun/Images/system_tray_icon.png", Description = "Use the System Tray icon for fast access" }
            },
            Title = "LifeTimer",
            ShowCTAScreen = true,
        };





        public App()
        {
            InitializeComponent();

            _manager = new AppManager(this);
            _manager.InitializeApplication(false); //standard version
            _manager.ConfigureFirstWizard(_firstRunWizardDefinition);
        }


        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await _manager.OnApplicationLaunched();
        }

    }
}
