using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
//using Windows.ApplicationModel;
using Windows.Storage;

namespace LifeTimer
{
    public partial class App : Application
    {
        private IHost _host;
        private Window? _window;
        public static IServiceProvider Services { get; private set; }

        public App()
        {
            InitializeComponent();

            ConfigureServices();

            //configure exception handler of last resort
            this.UnhandledException += App_UnhandledException;
        }

        private void ConfigureServices()
        {
            // Configure Serilog for file logging
            var logPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs", "lifetimer-.log");

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

                    services.AddSingleton<Logic.ApplicationController>();
                    services.AddSingleton<Logic.SettingsManager>();
                    services.AddSingleton<Logic.TimerRotator>();
                    services.AddSingleton<Logic.NagTimer>();
                    services.AddSingleton<Logic.WindowsStoreHelper>();


                    /*
                    services.AddSingleton<Logic.Logger>();
                    services.AddSingleton<Logic.WindowStore>();
                    services.AddSingleton<Logic.FreemiumNagTimer>();
                    */
                })
                .Build();

            Services = _host.Services;
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            var logger = Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("LifeTimer application starting...");

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

            await GetAvailableProductsandStoreVersion();

            _window = new MainWindow();

            _window.Closed += _window_Closed;

            _window.Activate();
        }



        private async Task GetAvailableProductsandStoreVersion()
        {
                var logger = Services.GetRequiredService<ILogger<App>>();
                var storeHelper = Services.GetRequiredService<WindowsStoreHelper>();

                await storeHelper.CheckAndCacheProductAvailability();
                await storeHelper.CheckAndCacheProductVersionAsync();

        }


        private void _window_Closed(object sender, WindowEventArgs args)
        {
            Application.Current.Exit();
        }


        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception or show a dialog
            var logger = Services.GetRequiredService<ILogger<App>>();
            logger.LogError("Unexpected Exception: " + e.Message);

            if (e.Exception != null && e.Exception.StackTrace != null)
            {
                logger.LogError("Stack Trace: " + e.Exception.StackTrace.ToString());
            }
            Application.Current.Exit();
        }

    }
}
