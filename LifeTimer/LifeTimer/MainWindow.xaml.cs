using LifeTimer.Helpers;
using LifeTimer.Logic;
using LifeTimer.Logic.Models;
using LifeTimer.Transparency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Drawing.Text;
using System.Transactions;
using Windows.UI.WebUI;
using WinRT.Interop;

namespace LifeTimer
{
    public sealed partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger;
        private ApplicationController AppController { get; set; }
        private AppWindow _appWindow;
        private bool _activedFlag = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = ResourceHelper.GetString("MainWindow_Title");
            this.Activated += MainWindow_ActivatedHandler;


            _logger = App.Services.GetRequiredService<ILogger<MainWindow>>();
            AppController = App.Services.GetRequiredService<ApplicationController>();

            _logger.LogInformation("MainWindow initializing...");

            // Get the AppWindow for this window
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            _appWindow.SetIcon("Resources\\app_icon.ico");

            //set icon and respond to events
            _appWindow.Changed += _appWindow_Changed;
            _appWindow.Closing += _appWindow_Closing;


            // Add Event Listeners for the Notify window commands
            Interactive.ExecuteRequested += Interactive_ExecuteRequested;
            Settings.ExecuteRequested += Settings_ExecuteRequested;
            Exit.ExecuteRequested += Exit_ExecuteRequested;

            InitializeNagOverlay();

            AppController.RegisterMainWindow(this);
            AppController.InitialisePreMain();

            _logger.LogInformation("MainWindow initialized successfully");

            InitializeTransparency();
            
            AppController.OnTimer += AppController_TimerTick;

        }



        private async void MainWindow_ActivatedHandler(object sender, WindowActivatedEventArgs args)
        {
            if (!_activedFlag)
            {
                _activedFlag = true;
                CalcBoundsAdjustment();
                UpdateDisplay();
                await AppController.InitialisePostMain();
            }
        }

        private void _appWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            ShutdownMainWindow();
        }

        private void _appWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {

            if (args.DidPositionChange || args.DidPositionChange)
            {
                int x = sender.Position.X;
                int y = sender.Position.Y;
                int width = sender.Size.Width;
                int height = sender.Size.Height;       

                AppController.RegisterMainWindowBoundsChange(x, y, width, height);
            }

        }

        public void ShutdownMainWindow()
        {
            AppController.ProcessMainWindowShutdown();
        }




        #region public interface


        private void AppController_TimerTick(object? sender, EventArgs e)
        {

            UpdateDisplay();

        }


        private void UpdateDisplay()
        {
            var currentTimer = AppController.GetCurrentTimer();

            if(currentTimer == null)
            {
                this.NoTimerDefined.Visibility = Visibility.Visible;
                this.TimerDisplay.Visibility = Visibility.Collapsed;
                return;
            }

            this.NoTimerDefined.Visibility = Visibility.Collapsed;
            this.TimerDisplay.Visibility = Visibility.Visible;

            var timerText = DateTimeFormatHelper.GetTimeDisplayForTimer(currentTimer);            
            this.TimerTitle.Text= currentTimer.Title;
            this.TimerTime.Text = timerText;
        }


   


        public void ConfigureForInteractiveMode()
        {
            ContextInteractive.Text = ResourceHelper.GetString("MainWindow_NotificationBackground");


            HideNagOverlay();

            WindowHelper.RestoreWindowToDefault(this);
            WindowHelper.SetNoActivate(this, false);
            WindowHelper.SetClickThrough(this, false);
            //TransparentHelper.SetTransparent(this, false);

            //resize for framed window
            int x = AppController.CurrentSettings.WindowPosX;
            int y = AppController.CurrentSettings.WindowPosY;
            int width = AppController.CurrentSettings.WindowWidth;
            int height = AppController.CurrentSettings.WindowHeight;

            int adjustedX = x + BoundsAdjustRect.Left;
            int adjustedY = y + BoundsAdjustRect.Top;
            int adjustedWidth = width + BoundsAdjustRect.Right - BoundsAdjustRect.Left;
            int adjustedHeight = height + BoundsAdjustRect.Bottom - BoundsAdjustRect.Top;

            WindowHelper.SetWindowBounds(AppWindow, adjustedX, adjustedY, adjustedWidth, adjustedHeight);


            _interactiveResized = true;

            AppWindow.Resize(new Windows.Graphics.SizeInt32(adjustedWidth, adjustedHeight));


        }


        public void ConfigureForBackgroundMode()
        {
            HideNagOverlay();

            WindowHelper.SetNoActivate(this, true);
            WindowHelper.SetWindowToBorderless(this);
            WindowHelper.SendToBack(this);
            // WindowHelper.RecalcWindowSize(this);


            // TransparentHelper.SetTransparent(this, true);
            ContextInteractive.Text = ResourceHelper.GetString("MainWindow_NotificationInteractive");

            if (_interactiveResized)
            {
                //if we have previously resized, reverse the change
                int x = AppController.CurrentSettings.WindowPosX;
                int y = AppController.CurrentSettings.WindowPosY;
                int width = AppController.CurrentSettings.WindowWidth;
                int height = AppController.CurrentSettings.WindowHeight;

                int adjustedX = x - BoundsAdjustRect.Left;
                int adjustedY = y - BoundsAdjustRect.Top;
                int adjustedWidth = width - BoundsAdjustRect.Right + BoundsAdjustRect.Left;
                int adjustedHeight = height - BoundsAdjustRect.Bottom + BoundsAdjustRect.Top;

                WindowHelper.SetWindowBounds(AppWindow, adjustedX, adjustedY, adjustedWidth, adjustedHeight);

            }


            this.SystemBackdrop = null;

            TransparentHelper.SetTransparent(this, true);

        }


        public void SetWindowBounds(int x, int y, int width, int height)
        {
            WindowHelper.SetWindowBounds(_appWindow, x, y, width, height); ;
        }



        public void SetWindowAppearance(AppearanceViewModel viewModel)
        {

            this.TimerTitle.Foreground = new SolidColorBrush(viewModel.ForegroundColor);
            this.TimerTitle.FontFamily = viewModel.TitleFontDefinition.GetWinUIFontFamily();
            this.TimerTitle.FontSize = viewModel.TitleFontDefinition.FontSize;
            this.TimerTitle.FontWeight = viewModel.TitleFontDefinition.FontWeight;
            this.TimerTitle.FontStyle = viewModel.TitleFontDefinition.FontStyle;

            this.NoTimer.Foreground= new SolidColorBrush(viewModel.ForegroundColor);
            this.NoTimer.FontFamily = viewModel.TitleFontDefinition.GetWinUIFontFamily();
            this.NoTimer.FontSize = viewModel.TitleFontDefinition.FontSize;
            this.NoTimer.FontWeight = viewModel.TitleFontDefinition.FontWeight;
            this.NoTimer.FontStyle = viewModel.TitleFontDefinition.FontStyle;

            this.TimerTime.Foreground = new SolidColorBrush(viewModel.ForegroundColor);
            this.TimerTime.FontFamily = viewModel.TimerFontDefinition.GetWinUIFontFamily();
            this.TimerTime.FontSize = viewModel.TimerFontDefinition.FontSize;
            this.TimerTime.FontWeight = viewModel.TimerFontDefinition.FontWeight;
            this.TimerTime.FontStyle = viewModel.TimerFontDefinition.FontStyle;

            this.WindowBorder.Background = new SolidColorBrush(viewModel.BackgroundColor);
            this.WindowBorder.BorderThickness = new Thickness( viewModel.BorderThickness );
            this.WindowBorder.BorderBrush = new SolidColorBrush(viewModel.BorderColor);
            this.WindowBorder.CornerRadius = new CornerRadius(viewModel.BorderRadius);


        }


        private void SetTransparentBackground()

        {
            this.SystemBackdrop = null;
            WindowHelper.SetWindowTransparentColorKey(this);
            this.WindowGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 255)); // Magenta
        }


        public Tuple<int, int> GetFirstRunScaledWindowDimensions(int width, int height)
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            return WindowHelper.getDpiScaledPixelsCurrentMonitor(hWnd, width, height);
        }


        public void ShowNagOverlay()
        {

            this.FreeVersionNagOverlay.Visibility = Visibility.Visible;

            Storyboard fadeIn = (Storyboard)ContentGrid.Resources["FadeInStoryboard"];
            fadeIn.Begin();

        }

        public void HideNagOverlay()
        {
            Storyboard fadeOut = (Storyboard)ContentGrid.Resources["FadeOutStoryboard"];
            fadeOut.Begin();
        }
        #endregion




        // Property with Instanciation
        internal XamlUICommand Interactive { get; set; } = new XamlUICommand();

        internal XamlUICommand Settings { get; set; } = new XamlUICommand();

        internal XamlUICommand Exit { get; set; } = new XamlUICommand();


        private void Interactive_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppController.RequestInteractiveToggle();

        }

        private void Settings_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppController.RequestShowSettingsWindow();
        }

        private void Exit_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppController.RequestApplicationExit();
        }

        private void InitializeNagOverlay()
        {
            this.FreeVersionNagOverlay.Opacity = 0;
            this.FreeVersionNagOverlay.Visibility = Visibility.Collapsed;
            Storyboard fadeOutStoryBoard = (Storyboard)this.ContentGrid.Resources["FadeOutStoryboard"];
            fadeOutStoryBoard.Completed += FadeOutStoryBoard_Completed;
        }

        private void FadeOutStoryBoard_Completed(object? sender, object e)
        {
            this.FreeVersionNagOverlay.Visibility = Visibility.Collapsed;
        }




        private void InitializeTransparency()
        {
            TransparentHelper.SetTransparent(this, true);
        }


        private void CalcBoundsAdjustment()
        {
            var width = AppController.CurrentSettings.WindowWidth;
            var height = AppController.CurrentSettings.WindowHeight;

            WindowHelper.RECT rect = WindowHelper.GetInteractiveWindowBoundsAdjustment(0, 0);
            Console.WriteLine($"Bounds adjustment LEFT: {rect.Left} TOP:{rect.Top} RIGHT:{rect.Right} BOTTOM:{rect.Bottom} ");
            BoundsAdjustRect = rect;
        }

        private WindowHelper.RECT BoundsAdjustRect { get; set; }

        private bool _interactiveResized { get; set; } = false;

    }
}
