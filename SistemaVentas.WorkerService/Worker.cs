using SistemaVentas.Application.Interfaces;
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


        // Método que permite la extracción y carga
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();

                var orchestrator = scope.ServiceProvider.GetRequiredService<EtlOrchestrator>();

                var dataWarehouseLoadService = scope.ServiceProvider.GetRequiredService<IDataWarehouseLoadService>();

                // Primero se ejecuta la extracción completa
                var metrics = await orchestrator.RunAsync(stoppingToken);

                _logger.LogInformation("Extraction complete. Sources processed: {totalSources}", metrics.TotalSources);
                _logger.LogInformation("Extracted records: {totalRecords}", metrics.TotalRecords);

                // Se busca el resultado del extractor CSV
                var csvResult = metrics.Results.FirstOrDefault(x =>
                    x.SourceName == "CSV Files" &&
                    x.WasSuccessful &&
                    !string.IsNullOrWhiteSpace(x.StagingFilePath));

                // Si no aparece el archivo staging del CSV, no se continua la carga
                if (csvResult == null)
                {
                    _logger.LogWarning("The CSV staging file was not found. The data warehouse load could not be executed.");
                    return;
                }

                _logger.LogInformation("Staging File found: {filePath}", csvResult.StagingFilePath);

                // Se ejecuta la carga de dimensiones
                var dimensionResult = await dataWarehouseLoadService.LoadDimensionsAsync(csvResult.StagingFilePath!, stoppingToken);

                _logger.LogInformation("Executed Process {processName}", dimensionResult.ProcessName);
                _logger.LogInformation("Upload result: {message}", dimensionResult.Message);
                _logger.LogInformation("DimCategory: {count}", dimensionResult.TotalCategories);
                _logger.LogInformation("DimProduct: {count}", dimensionResult.TotalProducts);
                _logger.LogInformation("DimLocation: {count}", dimensionResult.TotalLocations);
                _logger.LogInformation("DimCustomer: {count}", dimensionResult.TotalCustomers);
                _logger.LogInformation("DimOrderStatus: {count}", dimensionResult.TotalStatuses);
                _logger.LogInformation("DimDate: {count}", dimensionResult.TotalDates);

                if (!dimensionResult.WasSuccessful)
                {
                    _logger.LogWarning("The dimension load process did not complete successfully. The fact table load will not be executed.");
                    return;
                }


                // Se ejecuta la carga de hechos
                _logger.LogInformation("Starting FactSales load...");

                var factsResult = await dataWarehouseLoadService.LoadFactsAsync(
                    csvResult.StagingFilePath!,
                    stoppingToken);

                _logger.LogInformation("Executed Process {processName}", factsResult.ProcessName);
                _logger.LogInformation("Upload result: {message}", factsResult.Message);
                _logger.LogInformation("FactSales loaded: {count}", factsResult.TotalFactSales);

                if (!factsResult.WasSuccessful)
                {
                    _logger.LogWarning("The FactSales load encountered errors.");
                    return;
                }

                _logger.LogInformation("Process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the execution of the Worker.");
            }

            _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
        }
    }
}