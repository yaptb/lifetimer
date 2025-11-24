using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LifeTimer.Logic
{
    public class NagTimer
    {

        private readonly ILogger<NagTimer> _logger;
        private ApplicationController _applicationController;
        private Timer _sleepTimer;
        private Timer _screenTimer;
        private Timer _textUpdateTimer;

        private int _nagSleepInitialIntervalSeconds = 30;
        private int _nagSleepIntervalSeconds = 3 * 60; //3 minutes
        private int _nagVisibilityIntervalSeconds = 15;
        private int _textUpdateTimeIntervalSeconds = 10;


        private List<string> _overlayText = new() { "LifeTimer Free Version", "Use Help Page To Upgrade" };
        private int _overlayCount = 0;


        private bool _isRunning = false;

        private readonly string _nagText = String.Empty;


        public NagTimer(ILogger<NagTimer> logger)
        {
            _logger = logger;
        }


        public void Initialize(ApplicationController controller)
        {
            this._applicationController = controller;
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
            _textUpdateTimer?.Dispose();
            _isRunning = false;
        }


        private void OnSleepTimerElapsed(object state)
        {

            ShowNagScreen();
            _overlayCount = 0;
            _screenTimer = new Timer(OnScreenTimerElapsed, null, TimeSpan.FromSeconds(_nagVisibilityIntervalSeconds), Timeout.InfiniteTimeSpan);

            _textUpdateTimer = new Timer(OnTextUpdateTimerElapsed, null, TimeSpan.FromSeconds(_textUpdateTimeIntervalSeconds), Timeout.InfiniteTimeSpan);

        }


        private void OnScreenTimerElapsed(object state)
        {
            HideNagScreen();
        }


        private void OnTextUpdateTimerElapsed(object state)
        {
            _overlayCount++;

            if (_overlayCount >= _overlayText.Count)
                _overlayCount = 0;

            string nagText = _overlayText[_overlayCount];
            ChangeNagText(nagText);

        }


        private void ShowNagScreen()
        {
            string nagText = _overlayText[0];
            _applicationController.RequestShowFreemiumNagScreen(nagText);
        }


        private void ChangeNagText(string nagText)
        {
            _applicationController.RequestChangeNagText(nagText);
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
