using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LifeTimer.Logic.Models;
using LifeTimer.Helpers;
using LifeTimer.Logic;

namespace LifeTimer.Controls.Settings
{
    public sealed partial class TimerEditDialog : ContentDialog, INotifyPropertyChanged
    {
        private string _timerTitle = string.Empty;
        private DateTimeOffset _targetDate = DateTimeOffset.Now.AddDays(30);
        private TimeSpan _targetTime = new TimeSpan(0, 0, 0);
        private bool _displayDaysOnly = false;
        private bool _displayHours = true;
        private bool _displayMinutes = true;
        private bool _displaySeconds = true;
        private bool _isValid = false;
        private ApplicationController _applicationController;

        public TimerEditDialog()
        {
            this.InitializeComponent();
            _applicationController = App.Services.GetRequiredService<ApplicationController>();
            _applicationController.OnTimer += OnGlobalTimer;
            this.Closed += TimerEditDialog_Closed;
            UpdateValidation();
            UpdatePreview();
        }

        public TimerEditDialog(TimerDefinition timerToEdit) : this()
        {
            if (timerToEdit != null)
            {
                TimerTitle = timerToEdit.Title;
                TargetDate = timerToEdit.TargetDateTime;
                TargetTime = timerToEdit.TargetDateTime.TimeOfDay;
                DisplayDaysOnly = timerToEdit.DisplayDaysOnly;
                DisplayHours = timerToEdit.DisplayHours;
                DisplayMinutes = timerToEdit.DisplayMinutes;
                DisplaySeconds = timerToEdit.DisplaySeconds;
            }
        }

        public string TimerTitle
        {
            get => _timerTitle;
            set => SetProperty(ref _timerTitle, value);
        }

        public DateTimeOffset TargetDate
        {
            get => _targetDate;
            set => SetProperty(ref _targetDate, value);
        }

        public TimeSpan TargetTime
        {
            get => _targetTime;
            set => SetProperty(ref _targetTime, value);
        }

        public bool DisplayDaysOnly
        {
            get => _displayDaysOnly;
            set => SetProperty(ref _displayDaysOnly, value);
        }

        public bool DisplayHours
        {
            get => _displayHours;
            set => SetProperty(ref _displayHours, value);
        }

        public bool DisplayMinutes
        {
            get => _displayMinutes;
            set => SetProperty(ref _displayMinutes, value);
        }

        public bool DisplaySeconds
        {
            get => _displaySeconds;
            set => SetProperty(ref _displaySeconds, value);
        }

        public bool IsValid
        {
            get => _isValid;
            private set => SetProperty(ref _isValid, value);
        }

        public string PreviewText
        {
            get
            {
                var targetDateTime = GetCombinedDateTime();
                var now = DateTime.Now;

                if (targetDateTime > now)
                {
                    // Countdown to target
                    return DateTimeFormatHelper.FormatCountdown(targetDateTime, DisplayDaysOnly, DisplayHours, DisplayMinutes, DisplaySeconds);
                }
                else
                {
                    // Countup from target
                    return DateTimeFormatHelper.FormatCountup(targetDateTime, DisplayDaysOnly, DisplayHours, DisplayMinutes, DisplaySeconds);
                }
            }
        }

        public TimerDefinition GetTimerDefinition()
        {
            return new TimerDefinition
            {
                Title = TimerTitle.Trim(),
                TargetDateTime = GetCombinedDateTime(),
                DisplayDaysOnly = DisplayDaysOnly,
                DisplayHours = DisplayHours,
                DisplayMinutes = DisplayMinutes,
                DisplaySeconds = DisplaySeconds
            };
        }

        private DateTime GetCombinedDateTime()
        {
            var date = TargetDate.Date;
            return date.Add(TargetTime);
        }

        private void UpdateValidation()
        {
            var titleValid = !string.IsNullOrWhiteSpace(TimerTitle);

            IsValid = titleValid;

            TitleErrorText.Visibility = titleValid ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnGlobalTimer(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            OnPropertyChanged(nameof(PreviewText));
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateValidation();
        }

        private void TargetDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Preview updates handled by global timer
        }

        private void TargetTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            // Preview updates handled by global timer
        }

        private void DisplayOptionsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Preview updates handled by global timer
        }

        private void TimerEditDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (_applicationController != null)
            {
                _applicationController.OnTimer -= OnGlobalTimer;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);

            if (propertyName == nameof(TimerTitle))
                UpdateValidation();

            // Handle hierarchical checkbox logic
            if (propertyName == nameof(DisplayHours))
                UpdateHierarchicalCheckboxes();
            else if (propertyName == nameof(DisplayMinutes))
                UpdateHierarchicalCheckboxes();

            // Trigger preview updates for display option changes
            if (propertyName == nameof(DisplayDaysOnly) || propertyName == nameof(DisplayHours) ||
                propertyName == nameof(DisplayMinutes) || propertyName == nameof(DisplaySeconds))
            {
                UpdatePreview();
            }

            // Preview updates are also handled by global timer

            return true;
        }

        private void UpdateHierarchicalCheckboxes()
        {
            // If hours is unchecked, uncheck and disable minutes and seconds
            if (!DisplayHours)
            {
                _displayMinutes = false;
                _displaySeconds = false;
                OnPropertyChanged(nameof(DisplayMinutes));
                OnPropertyChanged(nameof(DisplaySeconds));
            }
            // If minutes is unchecked, uncheck and disable seconds
            else if (!DisplayMinutes)
            {
                _displaySeconds = false;
                OnPropertyChanged(nameof(DisplaySeconds));
            }

            // Update control states
            OnPropertyChanged(nameof(IsMinutesEnabled));
            OnPropertyChanged(nameof(IsSecondsEnabled));
        }

        public bool IsMinutesEnabled => DisplayHours;
        public bool IsSecondsEnabled => DisplayHours && DisplayMinutes;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}