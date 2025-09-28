using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace LifeTimer.Logic
{
    public class NagTimer
    {
        private readonly ILogger<NagTimer> _logger;
        private  ApplicationController _applicationController;
        private Timer _sleepTimer;
        private Timer _screenTimer;

        private int _nagSleepInitialIntervalSeconds = 10;
        private int _nagSleepIntervalSeconds = 5 * 60; //5 minutes
        private int _nagVisibilityIntervalSeconds = 30;
 
        private bool _isRunning = false;

        public NagTimer(ILogger<NagTimer> logger)
        {
            _logger = logger;
        }


        public void Initialize(ApplicationController controller)
        {
            this._applicationController= controller;
            _logger.LogInformation("NagTimer initialized");
        }

        public bool IsRunning => _isRunning;

 
        public void Restart()
        {
            _logger.LogInformation($"Restarting NagTimer");

            if (_isRunning)
            {
                Stop();
            }

            _logger.LogInformation($"  Starting NagTimer with intervals: {_nagSleepInitialIntervalSeconds} {_nagSleepIntervalSeconds} seconds");

            _sleepTimer = new Timer(OnSleepTimerElapsed, null, TimeSpan.FromSeconds(_nagSleepInitialIntervalSeconds), TimeSpan.FromSeconds(_nagSleepIntervalSeconds));
            _isRunning = true;
        }


        public void Stop()
        {
            if (!_isRunning) return;

            _logger.LogInformation("   Stopping NagTimer");

            _sleepTimer?.Dispose();
            _screenTimer?.Dispose();
            _isRunning = false;
        }


        private void OnSleepTimerElapsed(object state)
        {
            ShowNagScreen();
            _screenTimer = new Timer(OnScreenTimerElapsed, null, TimeSpan.FromSeconds(_nagVisibilityIntervalSeconds), Timeout.InfiniteTimeSpan);
            
        }


        private void OnScreenTimerElapsed(object state)
        {
            HideNagScreen();
        }

        private void ShowNagScreen()
        {
            _applicationController.RequestShowFreemiumNagScreen();

        }

        private void HideNagScreen()
        {
            _applicationController.RequestHideFreemiumNagScreen();
        }

        public void Dispose()
        {
            Stop();
        }

    }
}
