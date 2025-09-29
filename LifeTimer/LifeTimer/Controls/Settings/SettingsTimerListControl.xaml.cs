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

            _logger = App.Services.GetRequiredService<ILogger<SettingsTimerListControl>>();
            _logger.LogInformation("SettingsTimerListControl Initializing");

            _applicationController = App.Services.GetRequiredService<ApplicationController>();
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
                    UpdateSettingsFromTimerList();
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
                            UpdateSettingsFromTimerList();
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

        private async void ViewTimerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is TimerDefinition timer)
                {
                    var dialog = new ContentDialog
                    {
                       // Title = timer.Title,
                        CloseButtonText = "Close",
                        XamlRoot = this.XamlRoot
                    };

                    var timeRemaining = timer.TargetDateTime - DateTime.Now;
                    var timeRemainingText = DateTimeFormatHelper.FormatTimeRemaining(timeRemaining, timer.DisplayDaysOnly, timer.DisplaySeconds);

                    var content = new StackPanel { Spacing = 12 };
                    content.Children.Add(new TextBlock
                    {
                        Text = $"Target: {timer.TargetDateTime:MMM dd, yyyy HH:mm:ss}",
                        FontSize = 14
                    });
                    content.Children.Add(new TextBlock
                    {
                        Text = $"Time Remaining: {timeRemainingText}",
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                        FontSize = 16
                    });
                    content.Children.Add(new TextBlock
                    {
                        Text = $"Display Options: Days Only: {timer.DisplayDaysOnly}, Show Seconds: {timer.DisplaySeconds}",
                        FontSize = 12,
                        Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                    });

                    dialog.Content = content;
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing timer");
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
                        UpdateSettingsFromTimerList();
                        _logger.LogInformation($"Deleted timer: {timer.Title}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting timer");
            }
        }

        private void UpdateSettingsFromTimerList()
        {
            if (_applicationController?.CurrentSettings != null)
            {
                _applicationController.CurrentSettings.Timers.Clear();
                foreach (var timer in Timers)
                {
                    _applicationController.CurrentSettings.Timers.Add(timer);
                }

                _applicationController.RequestSaveSettings();
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

            if (_applicationController.CheckURLCountExceeded(timerCount))
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