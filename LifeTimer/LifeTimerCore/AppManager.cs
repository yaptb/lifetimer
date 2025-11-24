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
    public  class AppManager
    {

        public static IServiceProvider Services { get; private set; }

        public static Application Current { get; private set; }

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            Services= serviceProvider;
        }

        public static void SetApplication(Application application)
        {
           Current= application;
        }



        public AppManager(Application app)
        {
            SetApplication(app);
        }


        public void InitializeApplication(bool isPlusVersion)
        {
            this.IsPlusVersion=isPlusVersion;
            ConfigureServices();
        }


        public void ConfigureFirstWizard(ObservableCollection<WizardStep> steps, string title)
        {
            var firstRunService = Services.GetRequiredService<FirstRunService>();
            firstRunService.ConfigureFirstRunWizard(steps, title);
        }


        private void ConfigureServices()
        {

            // Configure Serilog for file logging
            string logPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs", "lifetimer-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    logPath,
                    rollingInterval: Serilog.RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000, // 10MB
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Debug()
                .CreateLogger();

            // Build host with services
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add logging
                    services.AddLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.AddSerilog(Log.Logger);
                    });

                    // Register application services in dependency order

                    services.AddSingleton<LifeTimer.Logic.ApplicationController>();
                    services.AddSingleton<LifeTimer.Logic.SettingsManager>();
                    services.AddSingleton<LifeTimer.Logic.NagTimer>();
                    services.AddSingleton<Logic.TimerRotator>();
                    services.AddSingleton<LifeTimer.Logic.WindowsStoreHelper>();
                    services.AddSingleton<LifeTimer.FirstRun.FirstRunService>();

                    //   services.AddSingleton<QuoteManager>();

                    /*
                    services.AddSingleton<Logic.Logger>();
                    services.AddSingleton<Logic.WindowStore>();
                    services.AddSingleton<Logic.FreemiumNagTimer>();
                    */
                })
                .Build();

            //            Services = _host.Services;

            AppManager.SetServiceProvider(_host.Services);
        }



        public async Task OnApplicationLaunched()
        {

            var logger = AppManager.Services.GetRequiredService<ILogger<AppManager>>();
            logger.LogInformation("LifeTimerPlus application starting...");

            //Uniqueness test
            //------------------------------
            // Unique key for your app instance
            string instanceKey = "main";

            // Try to register this instance
            AppInstance instance = AppInstance.FindOrRegisterForKey(instanceKey);

            if (!instance.IsCurrent)
            {
                logger.LogInformation("Found duplicate instance. Redirecting");

                // Redirect activation to the existing instance and exit
                instance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs()).AsTask().Wait();
                Environment.Exit(0);
            }

            await EnsureAppInitialized();

            await GetAvailableProductsandStoreVersion();

            DisplayFirstRunOrMainWindow();
        }



        private async Task EnsureAppInitialized()
        {
            var appController = AppManager.Services.GetRequiredService<ApplicationController>();
            await appController.Initialize();
        }


        private async Task GetAvailableProductsandStoreVersion()
        {

            var logger = AppManager.Services.GetRequiredService<ILogger<AppManager>>();
            var storeHelper = AppManager.Services.GetRequiredService<WindowsStoreHelper>();

            if (IsPlusVersion)
            {
                //LifeTimer plus
                storeHelper.ForcePlusVersion();
            }
            else
            {
                //LifeTimer standard
                await storeHelper.CheckAndCacheProductAvailability();
                await storeHelper.CheckAndCacheProductVersionAsync();
            }

        }


        private void OnMainWindowClosed(object sender, WindowEventArgs args)
        {
            Application.Current.Exit();
        }

        private void DisplayMainWindow()
        {
            _window = new MainWindow();

            _window.Closed += OnMainWindowClosed;

            _window.Activate();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception or show a dialog
            var logger = AppManager.Services.GetRequiredService<ILogger<AppManager>>();
            logger.LogError("Unexpected Exception: " + e.Message);

            if (e.Exception != null && e.Exception.StackTrace != null)
            {
                logger.LogError("Stack Trace: " + e.Exception.StackTrace.ToString());
            }
            Application.Current.Exit();
        }

        #region FirstRun
        private FirstRunWindow _firstRunWindow;


        private void DisplayFirstRunOrMainWindow()
        {
            FirstRunService firstRunService = Services.GetRequiredService<FirstRunService>();



            var isFirstRun = firstRunService.CheckIsFirstRun();
            if (isFirstRun)
                DisplayFirstRun();
            else
                DisplayMainWindow();
        }


        private void DisplayFirstRun()
        {
            _firstRunWindow = new FirstRunWindow();
            _firstRunWindow.Closed += _firstRunWindow_Closed;
            _firstRunWindow.Activate();
        }


        private void _firstRunWindow_Closed(object sender, WindowEventArgs args)
        {
            FirstRunService firstRunService = Services.GetRequiredService<FirstRunService>();
            firstRunService.CancelFirstRun();

            DisplayMainWindow();
        }

        #endregion


        private IHost _host;
        private Window? _window;
        private bool IsPlusVersion { get; set; }
    }
}
