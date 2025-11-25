using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeTimer.FirstRun
{
    public class FirstRunViewModel : INotifyPropertyChanged
    {
        private FirstRunWizardDefinition _definition { get;  set; }
        private bool _showUpgradeCTAScreen { get; set; }
        
 
        
        public FirstRunViewModel( FirstRunWizardDefinition definition, bool isFreeVersion)
        {
            _definition = definition;
            _showUpgradeCTAScreen =_definition.ShowCTAScreen;

            ApplicationTitle = _definition.Title;
            CurrentIndex = 0;
            CanGoNext = true;
            CurrentStep = _definition.WizardSteps[CurrentIndex];

            _totalSteps = _definition.WizardSteps.Count;

            if (_showUpgradeCTAScreen)
                _totalSteps++;

            UpdateStepProgress();
        }



        private void UpdateStepProgress()
        {
            StepProgress = $"({CurrentIndex + 1}/{_totalSteps})";
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

                    if (_currentIndex < _definition.WizardSteps.Count)
                    {
                        CurrentStep = _definition.WizardSteps[_currentIndex];
                        IsCTAStep = false;
                    }
                    else
                        IsCTAStep = true;

                    CanGoNext = CurrentIndex < _totalSteps - 1;
                    CanGoPrevious = CurrentIndex > 0;
                    IsLastPage = CurrentIndex==_totalSteps-1;
                    UpdateStepProgress();
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


        public string StepProgress {
            get => _StepProgress;
            set
            {
                _StepProgress = value;
                OnPropertyChanged(nameof(StepProgress));
            }
        }


        public bool IsCTAStep
        {
            get => _isCTAStep;
            set
            {
                _isCTAStep = value;
                OnPropertyChanged(nameof(IsCTAStep));
            }
        }

 

        private WizardStep _currentStep;
        private string _applicationTitle;
        private bool _canGoNext;
        private bool _canGoPrevious;
        private bool _isLastPage;
        private int _currentIndex;
        private string _StepProgress;
        private bool _isCTAStep;
        private int _totalSteps;

    }
}


