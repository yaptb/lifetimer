
using LifeTimer.Helpers;
using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.FirstRun
{

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunWindow : Window
    {

        private FirstRunService _service;
        private const int InitialWindowWidth = 1024;
        private const int InitialWindowHeight = 768;
        private IntPtr _hWnd;
        private AppWindow _appWindow;
        private FirstRunViewModel _viewModel;


        public FirstRunWindow()
        {
            InitializeComponent();

            _service = AppManager.Services.GetRequiredService<FirstRunService>();

            _viewModel = _service.GetFirstRunViewModel();
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
            
            this.RootElement.DataContext = _viewModel;


            var storeHelper = AppManager.Services.GetRequiredService<WindowsStoreHelper>();
            storeHelper.SetUpgradeContextWindow(this);


            SetWindowAppearance();

        }

    

        private void SetWindowAppearance()
        {
            this.Title = _viewModel.ApplicationTitle + " Getting Started Guide";
            this.Greeting.Text = "Welcome to " + _viewModel.ApplicationTitle;

            // Get the AppWindow for this window
            var _hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(_hWnd);

            _appWindow = AppWindow.GetFromWindowId(windowId);
            _appWindow.SetIcon("Resources\\app_icon.ico");

            var width = InitialWindowWidth;
            var height = InitialWindowHeight;
            var x = this._appWindow.Position.X;
            var y = this._appWindow.Position.Y;
            var scaled_size = WindowHelper.getDpiScaledPixelsCurrentMonitor(_hWnd, width, height);

           if(scaled_size.Item1 > width || scaled_size.Item2 > height)
            {
                width = scaled_size.Item1;
                height = scaled_size.Item2;
            }
            WindowHelper.SetWindowBounds(_appWindow, x, y, width, height); ;

        }


        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoPrevious();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoNext();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void _viewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (_viewModel.IsCTAStep)
            {
                this.StepControl.Visibility = Visibility.Collapsed;
                this.CTAControl.Visibility = Visibility.Visible;
            }
            else
            {
                this.StepControl.Visibility = Visibility.Visible;
                this.CTAControl.Visibility = Visibility.Collapsed;
            }
        }
    }
}
