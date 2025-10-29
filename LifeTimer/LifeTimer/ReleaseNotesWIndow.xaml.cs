using LifeTimer.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReleaseNotesWindow : Window
    {
        private AppWindow appWindow;


        public ReleaseNotesWindow()
        {
            InitializeComponent();

            // Get the AppWindow for this window
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            //set the icon
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon("Resources\\app_icon.ico");


            //disable release notes rich text viewer flyouts
            this.ReleaseNotesViewer.ContextFlyout = null;
            this.ReleaseNotesViewer.SelectionFlyout = null;
        }

        public void ConfigureWindow()
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var dpiDimensions = GetScaledWindowDimensions(800, 600);
            WindowHelper.SetWindowBounds(appWindow, 200, 200, dpiDimensions.Item1, dpiDimensions.Item2);
            WindowHelper.BringToFront(this);
        }

        public void SetReleaseNotes(string releaseNotesText)
        {
            _releaseNotesText = releaseNotesText;
        }


        private Tuple<int, int> GetScaledWindowDimensions(int width, int height)
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            return WindowHelper.getDpiScaledPixelsCurrentMonitor(hWnd, width, height);
        }


        private string _releaseNotesText;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ReleaseNotesViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ReleaseNotesViewer.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, _releaseNotesText);
            ReleaseNotesViewer.IsReadOnly = true;
        }
    }
}
