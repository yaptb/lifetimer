using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeTimer.FirstRun
{
    public class FirstRunViewModel: INotifyPropertyChanged
    {
        private FirstRunService _service;
        private ObservableCollection<WizardStep> Steps { get; set; }



        public FirstRunViewModel(FirstRunService service, ObservableCollection<WizardStep> steps, string title)
        {
            _service = service;
            ApplicationTitle= title;
            Steps = steps;
            CurrentIndex = 0;
            CanGoNext = true;
            CurrentStep = steps[CurrentIndex];
        }


        public void GoNext()
        {
            if (CanGoNext)
                CurrentIndex++;
        }

        public void GoPrevious()
        {
            if (CanGoPrevious)
                CurrentIndex--;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;
                    OnPropertyChanged(nameof(CurrentIndex));

                    CurrentStep = Steps[_currentIndex];
                    CanGoNext = CurrentIndex < Steps.Count - 1;
                    CanGoPrevious = CurrentIndex > 0;
                    IsLastPage = CurrentIndex==Steps.Count-1;
                }
            }

    
        }


        public WizardStep CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged(nameof(CurrentStep));
                }
            }
        }


        public bool CanGoNext {
            get => _canGoNext;
            set {
                _canGoNext = value;
                OnPropertyChanged(nameof(CanGoNext));
            }
        }

        public bool CanGoPrevious
        {
            get => _canGoPrevious;
            set
            {
                _canGoPrevious = value;
                OnPropertyChanged(nameof(CanGoPrevious));
            }
        }


        public bool IsLastPage
        {
            get => _isLastPage;
            set {
                _isLastPage= value;
                OnPropertyChanged(nameof(IsLastPage));
            }
         }


        public string ApplicationTitle
        {
            get => _applicationTitle;
            set
            {
                _applicationTitle = value;
                OnPropertyChanged(nameof(ApplicationTitle));
            }
        }

        private WizardStep _currentStep;
        private string _applicationTitle;
        private bool _canGoNext;
        private bool _canGoPrevious;
        private bool _isLastPage;
        private int _currentIndex;

    }
}


