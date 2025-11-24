using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace LifeTimer.Logic
{
    public class TimerRotator
    {
        private readonly ILogger<TimerRotator> _logger;
        private  ApplicationController _applicationController;
        private Timer _timer;
        private int _intervalSeconds = 30;
        private int _timeRemaining = 30;
        private bool _isRunning = false;

        public TimerRotator(ILogger<TimerRotator> logger)
        {
            _logger = logger;
 //           _applicationController = applicationController;
            _logger.LogInformation("TimerRotator initialized");
        }


        public void Initialize(ApplicationController controller)
        {
            this._applicationController= controller;
        }



        public int IntervalSeconds
        {
            get => _intervalSeconds;
            set
            {
                _intervalSeconds = value;
                if (_isRunning)
                {
                    Stop();
                    Start();
                }
            }
        }

        public bool IsRunning => _isRunning;

        public void Start()
        {
            if (_isRunning) return;

            _logger.LogInformation("Starting TimerRotator with interval: {IntervalSeconds} seconds", _intervalSeconds);
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            _timeRemaining = _intervalSeconds;
            _isRunning = true;
            _applicationController.RequestUpdateLinkRotationTimer($"{_timeRemaining}");
        }


        public void Stop()
        {
            if (!_isRunning) return;

            _logger.LogInformation("Stopping TimerRotator");
            _timer?.Dispose();
            _timer = null;
            _isRunning = false;
            _timeRemaining = 0;
            _applicationController.RequestUpdateLinkRotationTimer(String.Empty);
        }

        private void OnTimerElapsed(object state)
        {
            _timeRemaining--;

            if (_timeRemaining == 0)
            {
                RotateTimers();
                _timeRemaining = _intervalSeconds;
            }

            _applicationController.RequestUpdateLinkRotationTimer($"{_timeRemaining}");
        }


        public void RotateTimers()
        {
            if (_applicationController.CurrentSettings == null)
                return;

            var timerList = _applicationController.CurrentSettings.Timers;
            
            if(timerList == null || timerList.Count==0)
            {
                _logger.LogWarning("Cannot rotate links: timer list is empty");
                return;
            }

            int currentRotationIndex = -1;

            if (_applicationController.CurrentSettings.CurrentRotationIndex != null) {
                currentRotationIndex = _applicationController.CurrentSettings.CurrentRotationIndex.Value;
            };

            currentRotationIndex++;
            if(currentRotationIndex < 0 || currentRotationIndex >= timerList.Count) 
                currentRotationIndex = 0;

            string timerId = timerList[currentRotationIndex].Id.ToString();
            string timerName = timerList[currentRotationIndex].Title;

            _logger.LogInformation("Rotating to Timer: {ID} (index: {Index})", timerId, currentRotationIndex);
            _applicationController.RequestPerformTimerRotation(timerId, currentRotationIndex);
        }

        public void Dispose()
        {
            Stop();
        }

    }
}
