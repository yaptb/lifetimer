using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;

namespace LifeTimer.Logic
{
    public class LinkRotator
    {
        private readonly ILogger<LinkRotator> _logger;
        private  ApplicationController _applicationController;
        private Timer _timer;
        private int _intervalSeconds = 30;
        private int _timeRemaining = 30;
        private bool _isRunning = false;

        public LinkRotator(ILogger<LinkRotator> logger)
        {
            _logger = logger;
 //           _applicationController = applicationController;
            _logger.LogInformation("LinkRotator initialized");
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

            _logger.LogInformation("Starting LinkRotator with interval: {IntervalSeconds} seconds", _intervalSeconds);
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            _timeRemaining = _intervalSeconds;
            _isRunning = true;
            _applicationController.RequestUpdateLinkRotationTimer($"{_timeRemaining}");
        }


        public void Stop()
        {
            if (!_isRunning) return;

            _logger.LogInformation("Stopping LinkRotator");
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
                RotateLinks();
                _timeRemaining = _intervalSeconds;
            }

            _applicationController.RequestUpdateLinkRotationTimer($"{_timeRemaining}");
        }


        public void RotateLinks()
        {
            /*
            var urlList = _applicationController.CurrentSettings.UrlList;
            var currentRotationIndex = _applicationController.CurrentSettings.CurrentRotationIndex;

            if(urlList == null || urlList.Count==0)
            {
                _logger.LogWarning("Cannot rotate links: URL list is empty");
                return;
            }

            if (currentRotationIndex == null)
                currentRotationIndex = -1;

            currentRotationIndex++;
            if(currentRotationIndex < 0 || currentRotationIndex >= urlList.Count) 
                currentRotationIndex = 0;

            string urlString = urlList[currentRotationIndex].ToString();
            
            _logger.LogInformation("Rotating to URL: {Url} (index: {Index})", urlString, currentRotationIndex);
            _applicationController.RequestPerformLinkRotation(urlString, currentRotationIndex);
            */
        }

        public void Dispose()
        {
            Stop();
        }

    }
}
