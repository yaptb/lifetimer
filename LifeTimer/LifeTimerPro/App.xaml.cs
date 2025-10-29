﻿using LifeTimer;
using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
using H.NotifyIcon;
using Microsoft.Windows.AppLifecycle;
using System;
using System.IO;
using System.Threading.Tasks;
//using Windows.ApplicationModel;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimerPro
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

                    services.AddSingleton<ApplicationController>();
                    services.AddSingleton<SettingsManager>();
                    services.AddSingleton<TimerRotator>();
                    services.AddSingleton<NagTimer>();
                    services.AddSingleton<WindowsStoreHelper>();
                    services.AddSingleton<ReleaseNotesService>();



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

            await CheckAndDisplayReleaseNotes();
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



        private async Task CheckAndDisplayReleaseNotes()
        {
            var releaseNotesService = Services.GetRequiredService<ReleaseNotesService>();

            if (releaseNotesService.CheckShowReleaseNotes())
            {

                var releaseNotesText = await releaseNotesService.GetReleaseNotesContent();

                if (releaseNotesText != null)
                {
                    _releaseNotesWindow = new ReleaseNotesWindow();
                    _releaseNotesWindow.SetReleaseNotes(releaseNotesText);
                    _releaseNotesWindow.Closed += _releaseNotesWindow_Closed;
                    _releaseNotesWindow.ConfigureWindow();
                    _releaseNotesWindow.Show();
                }
            }
        }


        private void _releaseNotesWindow_Closed(object sender, WindowEventArgs args)
        {
            var releaseNotesService = Services.GetRequiredService<ReleaseNotesService>();
            releaseNotesService.UpdateReleaseNotesStoredVersion();
        }


        private ReleaseNotesWindow _releaseNotesWindow;

    }
}
