using System.Diagnostics;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Models;
using SistemaVentas.Application.SourceModels.Api;

namespace SistemaVentas.Infrastructure.Extractors
{
    public class ApiExtractor : IExtractor
    {
        private readonly IApiClient<OrderTracking> _apiClient;
        private readonly IStagingService _stagingService;
        private readonly ILoggerService _loggerService;


        public string SourceName => "REST API";

        public ApiExtractor(
            IApiClient<OrderTracking> apiClient,
            IStagingService stagingService,
            ILoggerService loggerService)
        {
            _apiClient = apiClient;
            _stagingService = stagingService;
            _loggerService = loggerService;
        }


        // Ejecuta la extracción desde la API y guarda los datos en staging
        public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var startedAt = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _loggerService.LogInformation("Starting extraction from REST API...");

                // Se consumen los datos desde la API
                var data = await _apiClient.GetAsync(cancellationToken);

                // Se guarda el resultado en staging
                var stagingResult = await _stagingService.SaveAsync(data, "api_extraction.json", cancellationToken);

                stopwatch.Stop();

                _loggerService.LogInformation("API extraction completed successfully.");

                return new ExtractionResult
                {
                    SourceName = SourceName,
                    WasSuccessful = true,
                    RecordsExtracted = data.Count,
                    Message = "Extraction from REST API completed successfully.",
                    StagingFilePath = stagingResult.FilePath,
                    StartedAt = startedAt,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _loggerService.LogError("Error during extraction from REST API.", ex);

                return new ExtractionResult
                {
                    SourceName = SourceName,
                    WasSuccessful = false,
                    RecordsExtracted = 0,
                    Message = ex.Message,
                    StartedAt = startedAt,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }
        }
    }
}