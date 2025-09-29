using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using LifeTimer.Helpers;
using LifeTimer.Logic.Models;

namespace LifeTimer.Logic
{
    public class ApplicationController
    {

        private const int MaxURLCountFreeVersion = 3;
        private const int MaxURLCountProVersion = 15;

        private readonly ILogger<ApplicationController> _logger;
        private readonly SettingsManager _settingsManager;
        private readonly LinkRotator _linkRotator;
        private readonly NagTimer _nagTimer;
        private readonly WindowsStoreHelper _storeHelper;
        private DispatcherTimer _globalTimer;


        public ApplicationController(ILogger<ApplicationController> logger)
        {
            _logger = logger;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            //avoid circular dependencies
            _settingsManager = App.Services.GetRequiredService<SettingsManager>();

            _storeHelper = App.Services.GetRequiredService<WindowsStoreHelper>();

            _linkRotator = App.Services.GetRequiredService<LinkRotator>();
            _linkRotator.Initialize(this);

            _nagTimer = App.Services.GetRequiredService<NagTimer>();
            _nagTimer.Initialize(this);

            LoadSettings();
            InitializeGlobalTimer();
            _logger.LogInformation("ApplicationController initialized");
        }



        public void RegisterMainWindow(MainWindow window)
        {
            MainWindow = window;
        }


        #region public api


        public event EventHandler<EventArgs> NotifySettingsChange;
        public event EventHandler<EventArgs> NotifyBrowserBoundsChange;
        public event EventHandler<string> NotifyLinkRotationStatusChange;
        public event EventHandler<string> NotifyLinkRotationTimerChange;
        public event EventHandler<string> NotifyBrowserStatusChange;
        public event EventHandler<string> NotifySettingsStatusChange;
        public event EventHandler<EventArgs> NotifyVersionChange;
        public event EventHandler<EventArgs> OnTimer;


        /// <summary>
        /// note - this needs to be called after the main window is activated
        /// </summary>
        public void InitialisePreMain()
        {

            _logger.LogInformation("AppController Initalize start");


            if (MainWindow == null)
                throw new InvalidOperationException("MaiWindow is not registered");

            if (CurrentSettings == null)
                throw new InvalidOperationException("Settings are not loaded");


            //Set our version string so it is displayed when the settings window opens
            string version = $"LifeTimer Version {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            this.LastSettingsStatus = version;

            _logger.LogInformation($"AppController InitalizePreBrowser - version {version}");


            if (CurrentSettings.InteractiveStartup)
            {
                SetToInteractiveMode();
            }
            else
            {
                SetToBackgroundMode();
            }

            ApplyCurrentSettingsToMainWindow();

            _logger.LogInformation("AppController InitalizePreBrowser completed.");

        }

        /// <summary>
        /// this is called to complete initialization after the main window is active and the browser is ready
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task InitialisePostMain()
        {

            _logger.LogInformation("AppController InitalizePostBrowser start");


            if (CurrentSettings == null)
                throw new InvalidOperationException("Settings are not loaded");

            if (MainWindow == null)
                throw new InvalidOperationException("MainWindow is null");

            /*
            if (!String.IsNullOrEmpty(CurrentSettings.CurrentTimerName))
            {
                RequestBrowseToNewUrl(CurrentSettings.CurrentTimerName);
            }
            else
            {
                if (CurrentSettings.UrlList != null)
                    if (CurrentSettings.UrlList.Count > 0)
                        RequestBrowseToNewUrl(CurrentSettings.UrlList[0]);
            }
            */


            if (!CurrentSettings.InteractiveStartup)
            {
                //HACK - set to background mode a second time
                //to overcome the main window popping to the foreground
                //when first loading
                SetToBackgroundMode();
            }




            //HACK - ensure settings window is shown after the
            //browser initializes as the browser grabs the focus on startup
            if (CurrentSettings.ShowSettingsOnStartup)
            {
                ShowSettingsWindow();
            }


            InitializeLinkRotation();

            _logger.LogInformation("AppController InitalizePostBrowser Finished");
            _logger.LogInformation("AppController *** Initialization Complete ***");


        }


        public void RequestApplicationExit()
        {
            MainWindow.ShutdownMainWindow();
            MainWindow.Close();
        }


        /// <summary>
        /// called from the MainWindow as part of the shutdown process
        /// as a callback from ShutdownMainWindow()
        /// this enables the main window to shut down cleanly if it is 
        /// closed via the title bar
        /// </summary>
        public void ProcessMainWindowShutdown()
        {
            HideSettingsWindow();

            //clean up link rotation
            if (_linkRotator.IsRunning)
            {
                _linkRotator.Stop();
            }

            _globalTimer?.Stop();
            _globalTimer = null;
            _nagTimer.Stop();
        }


        public void RequestInteractiveToggle()
        {
            if (IsInteractiveMode)
            {
                SetToBackgroundMode();
            }
            else

            {
                SetToInteractiveMode();
            }

        }

        public void RequestInteractiveMode()
        {
            SetToInteractiveMode();
        }

        public void RequestBackgroundMode()
        {
            SetToBackgroundMode();
        }


        public void RequestShowSettingsWindow()
        {
            ShowSettingsWindow();
        }


        public void RequestSaveSettings()
        {
            SaveSettings();
        }


        //
        //called by the link rotation helper to browser to a new link
        // NOTE - the link rotator runs on a NON UI THREAD
        //
        public void RequestPerformLinkRotation(string urlString, int rotationIndex)
        {
            MarshallToUIThread(() =>
            {
                this.CurrentSettings.CurrentRotationIndex = rotationIndex;
                this.RequestBrowseToNewUrl(urlString);
            });

        }

        public void RequestUpdateLinkRotationTimer(string s)
        {
            MarshallToUIThread(() =>
            {
                this.NotifyLinkRotationTimerChange?.Invoke(this, s);
            });
        }

        public void RequestBrowseToNewUrl(string urlString)
        {
            /*
            try
            {
                MainWindow?.BrowseToUrl(urlString);
                CurrentSettings.CurrentUrl = urlString;
                ProcessBrowserStatusChange(urlString);
            }
            catch (Exception ex)
            {
                //TODO - raise error;
            }
            */
        }

        public void RegisterMainWindowBoundsChange(int x, int y, int width, int height)
        {

            CurrentSettings.WindowPosX = x;
            CurrentSettings.WindowPosY = y;
            CurrentSettings.WindowWidth = width;
            CurrentSettings.WindowHeight = height;

            NotifyBrowserBoundsChange?.Invoke(this, EventArgs.Empty);
        }

        public void RequestSettingsStartInteractiveModeChange(bool interactiveStartup)
        {

            this.CurrentSettings.InteractiveStartup = interactiveStartup;
            ProcessSettingsChange();
        }

        public void RequestSettingsShowSettingsOnStartup(bool showSettingsOnStartup)
        {
            this.CurrentSettings.ShowSettingsOnStartup = showSettingsOnStartup;
            ProcessSettingsChange();
        }

        /*
        public void RequestSettingsAllowInput(bool allowInput)
        {
            this.CurrentSettings.AllowBackgroundInput = allowInput;

            if (!IsInteractiveMode)
            {
                //if the window is in background mode, reconfigure it with the 
                //new suppress input settings
                this.MainWindow.ConfigureForBackgroundMode();
            }

            ProcessSettingsChange();
        }
        */

        public void RequestSettingsBrowserWindowOpacity(int opacity)
        {
            this.CurrentSettings.WindowOpacity = opacity;
            this.MainWindow.SetWindowOpacity(opacity);
            ProcessSettingsChange();
        }


        public void RequestSettingsUpdateUrlList(List<string> urlList)
        {
            /*
            CurrentSettings.UrlList = urlList;
            UpdateLinkRotation();
            ProcessSettingsChange();
            */
        }

        public void RequestChangeLinkRotationDelay(int delaySecs)
        {
            this.CurrentSettings.TimerRotationDelaySecs = delaySecs;
            this._linkRotator.IntervalSeconds = delaySecs;
            ProcessSettingsChange();
        }

        public void RequestChangeLinkRotationStatus(bool newStatus)
        {
            this.CurrentSettings.RotateTimers = newStatus;
            UpdateLinkRotation();
            ProcessSettingsChange();
        }


        public void MarshallToUIThread(Action action)
        {
            _dispatcherQueue.TryEnqueue(() => action());
        }


        public bool CheckIsFreeVersion()
        {
            return _storeHelper.IsFreeVersion;
        }





        public async void RequestVersionUpgrade(string productID)
        {

            if (String.IsNullOrEmpty(productID))
                throw new InvalidOperationException("productID is null");

            if (_settingsManager == null)
                throw new InvalidOperationException("Setting window is null");

            try
            {

                _logger.LogInformation($"Attempting store purchase upgrade");
                var result = await (((SettingsWindow)_settingsWindow).PeformStorePurchaseInSettingsUI(productID));

                if (result != null)
                {

                    if (result.Status == Windows.Services.Store.StorePurchaseStatus.Succeeded)
                    {
                        _logger.LogInformation("Store purchase successful");

                        _storeHelper.InvalidateCache();
                        await ProcessInflightVersionUpgrade();

                        NotifySettingsStatusChange?.Invoke(this, "Upgrade complete");
                    }
                }

            }
            catch (Exception ex)
            {
                NotifySettingsStatusChange?.Invoke(this, "Store upgrade error");
                _logger.LogError("Upgrade error " + ex.Message);
            }

        }


        public bool CheckURLCountExceeded(int urlCount)
        {
            if (CheckIsFreeVersion())
            {
                if (urlCount >= MaxURLCountFreeVersion)
                    return true;
            }
            else
            {
                if (urlCount >= MaxURLCountProVersion)
                    return true;

            }

            return false;

        }


        //invoked by the nag timer to show the nag screen
        //note that the nag timer runs on a separate thread
        public void RequestShowFreemiumNagScreen()
        {

            if (!CheckIsFreeVersion())
            {
                //never show the nag screen for the pro version
            }

            if (this.IsInteractiveMode)
            {
                //dont show the nag screen when running in interactive mode
                return;
            }

            this.MarshallToUIThread(() =>
            {
                MainWindow.ShowNagOverlay();
            });
        }


        //invoked by the nag timer to hide the nag screen
        //note that the nag timer runs on a separate thread
        public void RequestHideFreemiumNagScreen()
        {
            this.MarshallToUIThread(() =>
            {
                MainWindow.HideNagOverlay();
            });
        }

        #endregion

        private void InitializeGlobalTimer()
        {
            _globalTimer = new DispatcherTimer();
            _globalTimer.Interval = TimeSpan.FromSeconds(1);
            _globalTimer.Tick += (sender, e) => OnTimer?.Invoke(this, EventArgs.Empty);
            _globalTimer.Start();
        }

        private void LoadSettings()
        {
            var settings = SettingsManager.LoadSettings();
            CurrentSettings = settings;
        }

        private void SaveSettings()
        {
            string settingsSavedStr = ResourceHelper.GetString("ApplicationController_SettingsSaved");
            SettingsManager.SaveSettings(CurrentSettings);

            ProcessSettingsStatusChange($"{settingsSavedStr} {DateTime.Now.ToLocalTime()}");
        }


        private void SetToInteractiveMode()
        {
            IsInteractiveMode = true;
            MainWindow.ConfigureForInteractiveMode();

            if (CheckIsFreeVersion())
                _nagTimer.Stop();
        }


        private void SetToBackgroundMode()
        {
            IsInteractiveMode = false;
            MainWindow.ConfigureForBackgroundMode();

            if (CheckIsFreeVersion())
                _nagTimer.Restart();
        }



        private void ApplyCurrentSettingsToMainWindow()
        {

            if (CurrentSettings.WindowWidth == -1)
            {
                //first run - calculate desired pixel size for current DPI
                //and update
                var scaled_size = MainWindow.GetFirstRunScaledWindowDimensions(800, 600);
                CurrentSettings.WindowWidth = scaled_size.Item1;
                CurrentSettings.WindowHeight = scaled_size.Item2;
            }

            MainWindow.SetWindowBounds(CurrentSettings.WindowPosX, CurrentSettings.WindowPosY,
               CurrentSettings.WindowWidth, CurrentSettings.WindowHeight);

            MainWindow.SetWindowOpacity(CurrentSettings.WindowOpacity);
        }

        private void ShowSettingsWindow()
        {

            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += _settingsWindow_Closed;
                _settingsWindow.Show();
            }

            _settingsWindow.Activate();
        }

        private void _settingsWindow_Closed(object sender, WindowEventArgs args)
        {
            _settingsWindow = null;
        }

        private void HideSettingsWindow()
        {

            if (_settingsWindow != null)
            {
                _settingsWindow.Close();
            }
        }

        private void ProcessSettingsChange()
        {
            NotifySettingsChange?.Invoke(this, EventArgs.Empty);
        }

        private void ProcessSettingsStatusChange(string status)
        {
            this.LastSettingsStatus = status;
            NotifySettingsStatusChange?.Invoke(this, status);
        }

        private void ProcessBrowserStatusChange(string status)
        {
            this.LastBrowserStatus = status;
            NotifyBrowserStatusChange?.Invoke(this, status);
        }

        private void ProcessRotationStatusChange(string status)
        {
            this.LastRotationStatus = status;
            NotifyLinkRotationStatusChange?.Invoke(this, status);
        }


        private void InitializeLinkRotation()
        {
            _linkRotator.IntervalSeconds = CurrentSettings.TimerRotationDelaySecs;
            UpdateLinkRotation();
        }

        private void UpdateLinkRotation()
        {
            /*
            string rotationDisabledStr = ResourceHelper.GetString("ApplicationController_RotationDisabled");
            string rotationStoppedStr = ResourceHelper.GetString("ApplicationController_RotationStopped");
            string rotationActiveStr = ResourceHelper.GetString("ApplicationController_RotationActive");

            if (CurrentSettings.UrlList.Count == 0)
            {
                _linkRotator.Stop();
                IsLinkRotationDisabled = true;
                CurrentSettings.RotateTimers = false;
                ProcessRotationStatusChange(rotationDisabledStr);
            }
            else

            {
                IsLinkRotationDisabled = false;

                if (CurrentSettings.RotateTimers == true)
                {
                    _linkRotator.Start();
                    ProcessRotationStatusChange(rotationActiveStr);
                }

                if (CurrentSettings.RotateTimers == false)
                {
                    _linkRotator.Stop();
                    ProcessRotationStatusChange(rotationStoppedStr);
                }

            }
            */
        }

        /// <summary>
        /// handle the transition from free to pro version
        /// while the application is running
        /// </summary>
        private async Task ProcessInflightVersionUpgrade()
        {

            _logger.LogInformation("Performing in flight upgrade");
            //update our internal version tracking
            _storeHelper.InvalidateCache();
            await _storeHelper.CheckAndCacheProductVersionAsync();

            //force stop the nag timer and force hide the nag overlay
            _nagTimer.Stop();
            RequestHideFreemiumNagScreen();

            //reticulate the change to UI controls
            MarshallToUIThread(() =>
            {
                this.NotifyVersionChange?.Invoke(this, EventArgs.Empty);
            });
        }


        private DispatcherQueue _dispatcherQueue;
        private Window _settingsWindow;

        public bool IsInteractiveMode { get; private set; }
        public MainWindow MainWindow { get; private set; }
        public SettingsManager SettingsManager { get { return _settingsManager; } }
        public SettingsViewModel CurrentSettings { get; private set; }

        public string LastSettingsStatus { get; private set; }
        public string LastRotationStatus { get; private set; }
        public string LastBrowserStatus { get; private set; }

        public bool IsLinkRotationDisabled { get; private set; }
        public bool IsLinkRotationActive { get { return _linkRotator.IsRunning; } }

    }
}
