using Microsoft.Extensions.Logging;
using System;

namespace LifeTimer.Logic
{
    public class Logger
    {
        private readonly ILogger<Logger> _logger;

        public Logger(ILogger<Logger> logger)
        {
            _logger = logger;
        }

        public void Debug(string source, string message)
        {
            _logger.LogDebug("[{Source}] {Message}", source, message);
        }

        public void Info(string source, string message)
        {
            _logger.LogInformation("[{Source}] {Message}", source, message);
        }

        public void Warning(string source, string message)
        {
            _logger.LogWarning("[{Source}] {Message}", source, message);
        }

        public void Error(string source, string message)
        {
            _logger.LogError("[{Source}] {Message}", source, message);
        }

        public void Error(string source, Exception exception, string message)
        {
            _logger.LogError(exception, "[{Source}] {Message}", source, message);
        }
    }
}
