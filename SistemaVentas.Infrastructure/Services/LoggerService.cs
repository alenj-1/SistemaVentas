using Microsoft.Extensions.Logging;
using SistemaVentas.Application.Interfaces;

namespace SistemaVentas.Infrastructure.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }


        // Registra un mensaje informativo
        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }


        // Registra una advertencia
        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }


        // Registra un error
        public void LogError(string message, Exception? exception = null)
        {
            _logger.LogError(exception, message);
        }
    }
}