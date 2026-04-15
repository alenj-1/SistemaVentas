using SistemaVentas.Application.Services;

namespace SistemaVentas.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();

                var orchestrator = scope.ServiceProvider.GetRequiredService<EtlOrchestrator>();

                var metrics = await orchestrator.RunAsync(stoppingToken);

                // Se muestran métricas generales en logs
                _logger.LogInformation("Extraction process complete.");
                _logger.LogInformation("Sources processed: {totalSources}", metrics.TotalSources);
                _logger.LogInformation("Extracted records: {totalRecords}", metrics.TotalRecords);
                _logger.LogInformation("Total duration in ms: {duration}", metrics.TotalDurationInMilliseconds);


                // Se recorren los resultados individuales para mostrarlos en consola/log
                foreach (var result in metrics.Results)
                {
                    _logger.LogInformation(
                        "Source: {source} | Success: {success} | Records: {records} | Message: {message}",
                        result.SourceName,
                        result.WasSuccessful,
                        result.RecordsExtracted,
                        result.Message);
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error general, se registra.
                _logger.LogError(ex, "A general error occurred during the execution of the Worker.");
            }

            // Mensaje final.
            _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
        }
    }
}