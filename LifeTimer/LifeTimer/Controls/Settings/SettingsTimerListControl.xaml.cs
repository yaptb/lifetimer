using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using LifeTimer.Logic;
using LifeTimer.Logic.Models;
using LifeTimer.Helpers;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class SettingsTimerListControl : UserControl
    {
        private readonly ILogger<SettingsTimerListControl> _logger;
        private readonly ApplicationController _applicationController;
        public ObservableCollection<TimerDefinition> Timers { get; private set; } = new ObservableCollection<TimerDefinition>();

        public SettingsTimerListControl()
        {
            this.InitializeComponent();

            _logger = AppManager.Services.GetRequiredService<ILogger<SettingsTimerListControl>>();
            _logger.LogInformation("SettingsTimerListControl Initializing");

            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();
            _applicationController.NotifyVersionChange += _applicationController_NotifyVersionChange;

            InitializeTimerListFromSettings();

            TimerListView.ItemsSource = Timers;

            _logger.LogInformation("SettingsTimerListControl initialized");
        }

        private void InitializeTimerListFromSettings()
        {
            if (_applicationController?.CurrentSettings?.Timers != null)
            {
                Timers.Clear();
                foreach (var timer in _applicationController.CurrentSettings.Timers)
                {
                    Timers.Add(timer);
                }
            }

            CheckAndUpdateUIForListMaximum();
        }

        private async void AddTimerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new TimerEditDialog
                {
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var newTimer = dialog.GetTimerDefinition();
                    Timers.Add(newTimer);
                    UpdateControllerTimerList();
                    _logger.LogInformation($"Added new timer: {newTimer.Title}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new timer");
            }
        }

        private async void EditTimerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is TimerDefinition timer)
                {
                    var dialog = new TimerEditDialog(timer)
                    {
                        XamlRoot = this.XamlRoot
                    };

                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        var editedTimer = dialog.GetTimerDefinition();
                        var index = Timers.IndexOf(timer);

                        if (index >= 0)
                        {
                            Timers[index] = editedTimer;
                            UpdateControllerTimerList();
                            _logger.LogInformation($"Edited timer: {editedTimer.Title}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing timer");
            }
        }

        private void ViewTimerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is TimerDefinition timer)
                {
                    _applicationController.RequestSetCurrentTimerId(timer.Id.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating timer");
            }
        }

        private async void DeleteTimerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is TimerDefinition timer)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Delete Timer",
                        Content = $"Are you sure you want to delete the timer '{timer.Title}'?",
                        PrimaryButtonText = "Delete",
                        SecondaryButtonText = "Cancel",
                        DefaultButton = ContentDialogButton.Secondary,
                        XamlRoot = this.XamlRoot
                    };

                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        Timers.Remove(timer);
                        UpdateControllerTimerList();
                        _logger.LogInformation($"Deleted timer: {timer.Title}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting timer");
            }
        }

        private void UpdateControllerTimerList()
        {

            if (_applicationController?.CurrentSettings != null)
            {
                /*
                _applicationController.CurrentSettings.Timers.Clear();
                foreach (var timer in Timers)
                {
                    _applicationController.CurrentSettings.Timers.Add(timer);
                }

                _applicationController.RequestSaveSettings();
                */

                var timers = Timers.ToList();
                _applicationController.RequestUpdateTimerList(timers);

                CheckAndUpdateUIForListMaximum();
            }
        }

        private void _applicationController_NotifyVersionChange(object? sender, EventArgs e)
        {
            CheckAndUpdateUIForListMaximum();
        }

        private void CheckAndUpdateUIForListMaximum()
        {
            var timerCount = Timers.Count;

            if (_applicationController.CheckTimerCountExceeded(timerCount))
            {
                AddTimerButton.Visibility = Visibility.Collapsed;
                if (_applicationController.CheckIsFreeVersion())
                    TimerListFullFreeBlock.Visibility = Visibility.Visible;
                else
                    TimerListFullProBlock.Visibility = Visibility.Visible;
            }
            else
            {
                AddTimerButton.Visibility = Visibility.Visible;
                TimerListFullFreeBlock.Visibility = Visibility.Collapsed;
                TimerListFullProBlock.Visibility = Visibility.Collapsed;
            }
        }

    }
}