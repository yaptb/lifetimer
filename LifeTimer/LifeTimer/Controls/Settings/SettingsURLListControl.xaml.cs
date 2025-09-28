using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsURLListControl : UserControl
    {
        private readonly ILogger<SettingsURLListControl> _logger;
        private readonly ApplicationController _applicationController;
        public ObservableCollection<string> Urls { get; private set; } = new ObservableCollection<string>();

        public SettingsURLListControl()
        {
            this.InitializeComponent();


            _logger = App.Services.GetRequiredService<ILogger<SettingsURLListControl>>();

            _logger.LogInformation("Settings Window Initializing");

            _applicationController = App.Services.GetRequiredService<ApplicationController>();

            _applicationController.NotifyVersionChange += _applicationController_NotifyVersionChange;
        
            InitializeURLListFromSettings();

            UrlListView.ItemsSource = Urls;
            UrlListView.SelectedIndex = 0;


            _logger.LogInformation("SettingsURLListControl initialized");
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var url = NewUrlTextBox.Text?.Trim();



            if (string.IsNullOrEmpty(url))
                return;

            //enforce a max length for urls
            if (url.Length > 1024)
                url = url.Substring(0, 1024);

            url = url.Replace(";", "");

            Urls.Add(url);
            NewUrlTextBox.Text = string.Empty;
            UpdateSettingsFromUrlList();

        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string url)
            {
                Urls.Remove(url);
                UpdateSettingsFromUrlList();
            }
        }


        private void InitializeURLListFromSettings()
        {

            if (_applicationController == null)
                throw new InvalidOperationException("Application Controller is null");

            if (_applicationController.CurrentSettings == null)
                throw new InvalidOperationException("ApplicationControlller.CurrentSettings is null");
           
            
                var urlList = _applicationController.CurrentSettings.UrlList;
                if (urlList != null)
                    foreach (var item in urlList)
                    {
                        Urls.Add(item);
                    }
             CheckAndUpdateUIForListMaximum();

        }


        private void UpdateSettingsFromUrlList()
        {
            var urlList = new List<string>();

            foreach (var item in Urls)
            {
                urlList.Add(item);
            }

            _applicationController.RequestSettingsUpdateUrlList(urlList);
            CheckAndUpdateUIForListMaximum();
        }


        private void URLGoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListGoButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string url)
            {
                _applicationController.RequestBrowseToNewUrl(url);
            }
        }


        private void _applicationController_NotifyVersionChange(object? sender, System.EventArgs e)
        {
            CheckAndUpdateUIForListMaximum();
        }


        private void CheckAndUpdateUIForListMaximum()
        {
            var listCount = Urls.Count;

            if (_applicationController.CheckURLCountExceeded(listCount))
            {
                this.NewUrlStackPanel.Visibility = Visibility.Collapsed;
                if (_applicationController.CheckIsFreeVersion())
                    this.UrlListFullFreeBlock.Visibility = Visibility.Visible;
                else
                    this.UrlListFullProBlock.Visibility = Visibility.Visible;
            }
            else
            {
                this.NewUrlStackPanel.Visibility = Visibility.Visible;
                this.UrlListFullFreeBlock.Visibility = Visibility.Collapsed;
                this.UrlListFullProBlock.Visibility = Visibility.Collapsed;
            }
        }

    }
}
