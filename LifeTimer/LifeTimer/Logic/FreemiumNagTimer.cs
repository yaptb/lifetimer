using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace LifeTimer.Logic
{
    internal class FreemiumNagTimer
    {
        private readonly ILogger<FreemiumNagTimer> _logger;
        private const int CHECK_LOOP_INTERVAL_MINUTES = 10;
        private const int NAG_DISPLAY_DURATION_MINUTES = 5;

        public FreemiumNagTimer(ILogger<FreemiumNagTimer> logger)
        {
            _logger = logger;
            _logger.LogInformation("FreemiumNagTimer initialized");
        }

        private Timer _checkLoopTimer;
        private Timer _nagDisplayTimer;
        private bool _checkLoopRunning = false;
        private bool _nagDisplayRunning = false;

        public bool IsCheckLoopRunning => _checkLoopRunning;
        public bool IsNagDisplayRunning => _nagDisplayRunning;

        public void StartCheckLoop()
        {
            if (_checkLoopRunning) return;

            var interval = TimeSpan.FromMinutes(CHECK_LOOP_INTERVAL_MINUTES);
            _checkLoopTimer = new Timer(OnCheckLoopElapsed, null, interval, interval);
            _checkLoopRunning = true;
        }

        public void StopCheckLoop()
        {
            if (!_checkLoopRunning) return;

            _checkLoopTimer?.Dispose();
            _checkLoopTimer = null;
            _checkLoopRunning = false;
        }

        public void StartNagDisplay()
        {
            if (_nagDisplayRunning) return;

            var duration = TimeSpan.FromMinutes(NAG_DISPLAY_DURATION_MINUTES);
            _nagDisplayTimer = new Timer(OnNagDisplayElapsed, null, duration, Timeout.InfiniteTimeSpan);
            _nagDisplayRunning = true;
        }

        public void StopNagDisplay()
        {
            if (!_nagDisplayRunning) return;

            _nagDisplayTimer?.Dispose();
            _nagDisplayTimer = null;
            _nagDisplayRunning = false;
        }

        public void StopAll()
        {
            StopCheckLoop();
            StopNagDisplay();
        }

        private void OnCheckLoopElapsed(object state)
        {
            PerformFreemiumNagCheck();
        }

        private void OnNagDisplayElapsed(object state)
        {
            StopNagDisplay();
            NagDisplayTimerStopped();
        }

        public void PerformFreemiumNagCheck()
        {
            // TODO: Implement freemium nag check logic
        }

        public void NagDisplayTimerStopped()
        {
            // TODO: Implement nag display timer stopped logic
        }

        public void Dispose()
        {
            StopAll();
        }
    }
}
