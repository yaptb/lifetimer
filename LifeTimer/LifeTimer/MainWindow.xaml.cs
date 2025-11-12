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
using System.Threading;
using WinRT.Interop;

namespace LifeTimer
{
    public sealed partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger;
        private ApplicationController AppController { get; set; }
        private AppWindow _appWindow;
        private bool _activedFlag = false;

        private bool _isInteractiveMode { get; set; } = false;
        private bool _captureSizeEvents { get; set; } =false;
        private bool _initializedFlag = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = ResourceHelper.GetString("MainWindow_Title");
            this.Activated += MainWindow_ActivatedHandler;


            _logger = AppManager.Services.GetRequiredService<ILogger<MainWindow>>();
            AppController = AppManager.Services.GetRequiredService<ApplicationController>();

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
            InitializeInteractiveHints();

            AppController.RegisterMainWindow(this);
            AppController.InitialisePreMain();

            _logger.LogInformation("MainWindow initialized successfully");

            InitializeTransparency();

            AppController.OnTimer += AppController_TimerTick;
            this.WindowGrid.DoubleTapped += WindowGrid_DoubleTapped;
            InteractiveToolbar.Visibility = Visibility.Visible;

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



            if (_initializedFlag && _captureSizeEvents)
            {
                if (args.DidPositionChange || args.DidSizeChange)
                {
                    //we will be in interactive mode here. Adjust to save 
                    //background mode sizes
                    int x = sender.Position.X;
                    int y = sender.Position.Y;
                    int width = sender.Size.Width;
                    int height = sender.Size.Height;

                    int adjustedX = x - BoundsAdjustRect.Left;
                    int adjustedY = y - BoundsAdjustRect.Top;
                    int adjustedWidth = width - BoundsAdjustRect.Right + BoundsAdjustRect.Left;
                    int adjustedHeight = height - BoundsAdjustRect.Bottom + BoundsAdjustRect.Top;

                    AppController.RegisterMainWindowBoundsChange(adjustedX, adjustedY, adjustedWidth, adjustedHeight);

                }

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

            PomodoroControl.UpdateDisplay();

        }

        public void DisplayInteractiveHints()
        {
            this.ShowInteractiveHint();
        }


        private void UpdateDisplay()
        {
            var currentTimer = AppController.GetCurrentTimer();

            if (currentTimer == null)
            {
                this.NoTimerDefined.Visibility = Visibility.Visible;
                this.TimerDisplay.Visibility = Visibility.Collapsed;
                return;
            }

            this.NoTimerDefined.Visibility = Visibility.Collapsed;
            this.TimerDisplay.Visibility = Visibility.Visible;

            var timerText = DateTimeFormatHelper.GetTimeDisplayForTimer(currentTimer);
            this.TimerTitle.Text = currentTimer.Title;
            this.TimerTime.Text = timerText;
        }





        public void ConfigureForInteractiveMode()
        {
            _captureSizeEvents = false;

            ContextInteractive.Text = ResourceHelper.GetString("MainWindow_NotificationBackground");


            HideNagOverlay();
       
            WindowHelper.HideWindow(this);
            WindowHelper.RestoreWindowToDefault(this);
            WindowHelper.RemoveCloseButton(this);
            WindowHelper.SetNoActivate(this, false);
            WindowHelper.SetClickThrough(this, false);

            //TransparentHelper.SetTransparent(this, false);


            if (!_isInteractiveMode)
            {

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

                AppWindow.Resize(new Windows.Graphics.SizeInt32(adjustedWidth, adjustedHeight));

            }

            _captureSizeEvents = true;
            _isInteractiveMode = true;
            InteractiveCancelButton.Visibility = Visibility.Visible;


            WindowHelper.ShowWindow(this);
            WindowHelper.BringToFront(this);

        }


        public void ConfigureForBackgroundMode()
        {
            _captureSizeEvents = false;
            _isInteractiveMode = false;

            HideNagOverlay();
 
            WindowHelper.HideWindow(this);
            WindowHelper.SetNoActivate(this, true);
            WindowHelper.SetWindowToBorderless(this);
             // WindowHelper.SendToBack(this);
            // WindowHelper.RecalcWindowSize(this);


            // TransparentHelper.SetTransparent(this, true);
            ContextInteractive.Text = ResourceHelper.GetString("MainWindow_NotificationInteractive");

            //set to our standard window size          
            int x = AppController.CurrentSettings.WindowPosX;
            int y = AppController.CurrentSettings.WindowPosY;
            int width = AppController.CurrentSettings.WindowWidth;
            int height = AppController.CurrentSettings.WindowHeight;

            int adjustedX = x - BoundsAdjustRect.Left;
            int adjustedY = y - BoundsAdjustRect.Top;
            int adjustedWidth = width - BoundsAdjustRect.Right + BoundsAdjustRect.Left;
            int adjustedHeight = height - BoundsAdjustRect.Bottom + BoundsAdjustRect.Top;

            WindowHelper.SetWindowBounds(AppWindow, x, y, width, height);

            InteractiveCancelButton.Visibility = Visibility.Collapsed;
            WindowHelper.ShowWindow(this);
        }


        public void ConfigureForTimerMode()
        {

            this.TimerButton.Visibility = Visibility.Collapsed;
            this.PomodoroButton.Visibility = Visibility.Visible;
            this.TimerRegion.Visibility=Visibility.Visible;
            this.PomodoroRegion.Visibility=Visibility.Collapsed;


        }


        public void ConfigureForPomodoroMode()
        {
            this.TimerButton.Visibility = Visibility.Visible;
            this.PomodoroButton.Visibility = Visibility.Collapsed;

            this.TimerRegion.Visibility = Visibility.Collapsed;
            this.PomodoroRegion.Visibility = Visibility.Visible;

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

            this.NoTimer.Foreground = new SolidColorBrush(viewModel.ForegroundColor);
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
            this.WindowBorder.BorderThickness = new Thickness(viewModel.BorderThickness);
            this.WindowBorder.BorderBrush = new SolidColorBrush(viewModel.BorderColor);
            this.WindowBorder.CornerRadius = new CornerRadius(viewModel.BorderRadius);

            this.PomodoroControl.SetAppearance(viewModel);

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


        public void ShowNagOverlay(string nagText)
        {

            this.FreeVersionNagOverlay.Visibility = Visibility.Visible;
            this.NagText.Text= nagText;

            Storyboard fadeIn = (Storyboard)ContentGrid.Resources["FadeInStoryboard"];
            fadeIn.Begin();

        }

        public void ChangeNagText(string nagText)
        {
            this.NagText.Text = nagText;
        }

        public void HideNagOverlay()
        {
            Storyboard fadeOut = (Storyboard)ContentGrid.Resources["FadeOutStoryboard"];
            fadeOut.Begin();
        }

        public void SetMainWindowInitialized()
        {
            _initializedFlag = true;
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


        private void WindowGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            InitializeInteractiveHints();

            if (_isInteractiveMode)
                AppController.RequestBackgroundMode();
            else
                AppController.RequestInteractiveMode();



        }

        private void InteractiveModeCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppController.RequestBackgroundMode();
        }


        private void InteractiveModeSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AppController.RequestShowSettingsWindow();
        }


        private void InteractiveModeTimerButton_Click(object sender, RoutedEventArgs e)
        {
            AppController.RequestTimerMode();
        }


        private void InteractiveModePomodoroButton_Click(object sender, RoutedEventArgs e)
        {
            AppController.RequestPomodoroMode();
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




        #region interactive hint

        private Timer _interactiveHintTimer;


        private void InitializeInteractiveHints()
        {
            this.InteractiveHintDisplay.Opacity = 0;
            this.InteractiveHintDisplay.Visibility = Visibility.Collapsed;
        }

        private void ShowInteractiveHint()
        {
            this.InteractiveHintDisplay.Visibility = Visibility.Visible;

            Storyboard fadeIn = (Storyboard)ContentGrid.Resources["HintFadeInStoryboard"];
            fadeIn.Begin();

            _interactiveHintTimer = new Timer(HideInteractiveHint, null, TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
        }

        private void HideInteractiveHint(object? state)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Storyboard fadeOut = (Storyboard)ContentGrid.Resources["HintFadeOutStoryboard"];
                fadeOut.Begin();
                _interactiveHintTimer?.Dispose();
            });
        }


        #endregion





        // private void InitializeTransparency()
        //  {
        //      TransparentHelper.SetTransparent(this, true);
        //  }


        private void CalcBoundsAdjustment()
        {
            var width = AppController.CurrentSettings.WindowWidth;
            var height = AppController.CurrentSettings.WindowHeight;

            WindowHelper.RECT rect = WindowHelper.GetInteractiveWindowBoundsAdjustment(0, 0);
            Console.WriteLine($"Bounds adjustment LEFT: {rect.Left} TOP:{rect.Top} RIGHT:{rect.Right} BOTTOM:{rect.Bottom} ");
            BoundsAdjustRect = rect;
        }


        private void InitializeTransparency()
        {
            var windowHandle = new IntPtr((long)this.AppWindow.Id.Value);

            var rgn = TransparencyInterop.CreateRectRgn(-2, -2, -1, -1);

            var blur = new TransparencyInterop.DWM_BLURBEHIND()
            {
                dwFlags = TransparencyInterop.DwmBlurBehindFlags.DWM_BB_ENABLE | TransparencyInterop.DwmBlurBehindFlags.DWM_BB_BLURREGION,
                fEnable = true,
                hRgnBlur = rgn,
            };

            TransparencyInterop.DwmEnableBlurBehindWindow(windowHandle, ref blur);

            TransparentHelper.SetTransparent(this, true);

            wndProcHandler = new TransparencyInterop.SUBCLASSPROC(WndProc);

            TransparencyInterop.SetWindowSubclass(windowHandle, wndProcHandler, 1, IntPtr.Zero);


            TransparentHelper.SetTransparent(this, true);
        }

        private IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
        {
            if (uMsg == (uint)TransparencyInterop.WM_PAINT)
            {
                var hdc = TransparencyInterop.BeginPaint(hWnd, out var ps);

                if (hdc == 0) return new IntPtr(0);

                var brush = TransparencyInterop.GetStockObject(TransparencyInterop.StockObjectType.BLACK_BRUSH);
                TransparencyInterop.FillRect(hdc, ref ps.rcPaint, brush);
                return new IntPtr(1);
            }

            return TransparencyInterop.DefSubclassProc(hWnd, uMsg, wParam, lParam);

        }

        TransparencyInterop.SUBCLASSPROC wndProcHandler;

        private WindowHelper.RECT BoundsAdjustRect { get; set; }


    }
}
