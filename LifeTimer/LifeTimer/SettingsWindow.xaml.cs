using LifeTimer.Controls.Layout;
using LifeTimer.Helpers;
using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Services.Store;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : Window
    {

        private Dictionary<SidenavItemControl, Control> _tabMapping = new Dictionary<SidenavItemControl, Control>();

        public SettingsWindow()
        {
            InitializeComponent();


            this.Title = ResourceHelper.GetString("SettingsWindow_Title");

            this.Activated += SettingsWindow_Activated;

            // Set the window size (e.g., 800x600)
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            var dpiScaledSize = WindowHelper.getDpiScaledPixelsCurrentMonitor(hWnd, 800, 600);
            var width = dpiScaledSize.Item1;
            var height = dpiScaledSize.Item2;

            appWindow.SetIcon("Resources\\app_icon.ico");
            appWindow.Resize(new SizeInt32(width, height));

            SetUpNavigation();

        }

        private void SettingsWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            WindowHelper.BringToFront(this);
        }



        private void SetUpNavigation()
        {
            //
            // Update this mapping if the navigatio menu changes
            //
            _tabMapping.Add(CommandsItem, CommandsTab);
            _tabMapping.Add(WebPagesItem, WebPagesTab);
            _tabMapping.Add(SettingsItem, SettingsTab);
            _tabMapping.Add(HelpItem, HelpTab);

            //reset state
            foreach (var kvp in _tabMapping)
            {
                var control = kvp.Value;
                control.Visibility = Visibility.Collapsed;
            }

            //select our default tab
            CommandsItem.Select();
            CommandsTab.Visibility = Visibility.Visible;

            //wire up event handlers
            foreach (var kvp in _tabMapping)
            {
                var navItem = kvp.Key as SidenavItemControl;
                navItem.OnSelected += NavItem_OnSelected;
            }

        }

        /// <summary>
        /// process a select event from a nav item control and update our menu state
        /// </summary>
        private void NavItem_OnSelected(object? sender, EventArgs e)
        {
            if (sender == null)
                throw new InvalidOperationException("no sender defined");

            var senderControl = sender as SidenavItemControl;

            if (senderControl == null)
                throw new InvalidOperationException("sender cannot be mapped to sidenavitemcontrol");

            var navControl = sender as SidenavItemControl;

            foreach (var kvp in _tabMapping)
            {
                var navItem = kvp.Key;
                var navTab = kvp.Value as Control;

                if (navItem == senderControl)
                {
                    navItem.Select();
                    navTab.Visibility = Visibility.Visible;
                }
                else
                {
                    navItem.Deselect();
                    navTab.Visibility = Visibility.Collapsed;
                }
            }
        }


        /// <summary>
        /// this is invoked from the application controller to perform a store upgrade
        /// in the context of the settings window UI
        /// </summary>
        public async Task<StorePurchaseResult> PeformStorePurchaseInSettingsUI(string productId)
        {

            var storeHelper = App.Services.GetRequiredService<WindowsStoreHelper>();

            var result = await storeHelper.PeformStorePurchaseInWindow(this, productId);

            return result;
        }


    }
}
