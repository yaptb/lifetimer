using LifeTimer.Logic;
using LifeTimer.Logic.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.Controls.Pomodoro
{

    public enum PomodoroState
    {
        Idle,
        Running,
        Paused,
        Finished
    }



    public sealed partial class PomodoroUserControl : UserControl
    {

        private readonly ApplicationController _applicationController;

        public PomodoroUserControl()
        {
            InitializeComponent();

            _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();

            State = PomodoroState.Idle;
            UpdateIdleState();

        }



        public void SetAppearance(AppearanceViewModel viewModel)
        {
            this.PomodoroTitle.Foreground = new SolidColorBrush(viewModel.ForegroundColor);
            this.PomodoroTitle.FontFamily = viewModel.TitleFontDefinition.GetWinUIFontFamily();
            this.PomodoroTitle.FontSize = viewModel.TitleFontDefinition.FontSize;
            this.PomodoroTitle.FontWeight = viewModel.TitleFontDefinition.FontWeight;
            this.PomodoroTitle.FontStyle = viewModel.TitleFontDefinition.FontStyle;

            this.PomodoroTime.Foreground = new SolidColorBrush(viewModel.ForegroundColor);
            this.PomodoroTime.FontFamily = viewModel.TimerFontDefinition.GetWinUIFontFamily();
            this.PomodoroTime.FontSize = viewModel.TimerFontDefinition.FontSize;
            this.PomodoroTime.FontWeight = viewModel.TimerFontDefinition.FontWeight;
            this.PomodoroTime.FontStyle = viewModel.TimerFontDefinition.FontStyle;
        }


        //this is called every second by the global timer
        public void UpdateDisplay()
        {

            switch (State)
            {
                case PomodoroState.Idle:
                    UpdateIdleState(); break;
                case PomodoroState.Running:
                    UpdateRunningState();
                    break;
                case PomodoroState.Paused:
                    UpdatePausedState(); break;

                case PomodoroState.Finished:
                    UpdateFinishedState(); break;

            }


        }


        private void UpdateIdleState()
        {

            this.PomodoroTitle.Text = _applicationController.CurrentSettings.Pomodoro.PomodoroTitle+ " Stopped";
            this.StartButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
            this.ResetButton.IsEnabled = true;


            ResetTimer();

            ShowTimeRemaining();
        }


        private void UpdateRunningState()
        {

            this.PomodoroTitle.Text = _applicationController.CurrentSettings.Pomodoro.PomodoroTitle+ " Running";

            this.StartButton.IsEnabled = false;
            this.StopButton.IsEnabled = true;
            this.ResetButton.IsEnabled = false;


            TimeRemaining = TargetTime - DateTime.Now;

            if (TimeRemaining.TotalSeconds <= 0)
            {
                //we've finished - move to our finished state
                State = PomodoroState.Finished;
                UpdateDisplay();
                return;
            }

            ShowTimeRemaining();
        }

        private void UpdatePausedState()
        {
            this.PomodoroTitle.Text = _applicationController.CurrentSettings.Pomodoro.PomodoroTitle + " Paused";
            this.StartButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
            this.ResetButton.IsEnabled = true;
        }


        private void UpdateFinishedState()
        {

            this.PomodoroTitle.Text = _applicationController.CurrentSettings.Pomodoro.PomodoroTitle + " Completed";
            
            this.StartButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
            this.ResetButton.IsEnabled = true;

            ShowFinished();
        }


        private void ShowTimeRemaining()
        {
            string timeStr = $"{TimeRemaining.Minutes:D2}:{TimeRemaining.Seconds:D2}";
            this.PomodoroTime.Text = timeStr;

        }

        private void ShowFinished()
        {
            this.PomodoroTime.Text = _applicationController.CurrentSettings.Pomodoro.PomodoroFinsished;

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.State == PomodoroState.Finished)
            {
                ResetTimer();
            }

            this.State = PomodoroState.Running;
            UpdateRunningState();

        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.State = PomodoroState.Paused;
            UpdatePausedState();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
                //finished or paused
                this.State = PomodoroState.Idle;
                UpdateIdleState();
        }

        private void ResetTimer()
        {
            var targetMinutes = _applicationController.CurrentSettings.Pomodoro.PomodoroMinutes;
            TargetTime = DateTime.Now.AddSeconds(targetMinutes * 60 + 1);
            TimeRemaining = TargetTime - DateTime.Now;


        }


        private TimeSpan TimeRemaining { get; set; }
        private DateTime TargetTime { get; set; }
        private PomodoroState State { get; set; }

    }
}
