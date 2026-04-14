using System.Diagnostics;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Models;

namespace SistemaVentas.Application.Services
{
    public class EtlOrchestrator
    {
        private readonly IEnumerable<IExtractor> _extractors;
        private readonly ILoggerService _loggerService;


        public EtlOrchestrator(IEnumerable<IExtractor> extractors, ILoggerService loggerService)
        {
            _extractors = extractors;
            _loggerService = loggerService;
        }


        public async Task<ProcessMetrics> RunAsync(CancellationToken cancellationToken = default)
        {
            var metrics = new ProcessMetrics
            {
                StartedAt = DateTime.Now
            };

            // Stopwatch para poder medir el tiempo total del proceso
            var stopwatch = Stopwatch.StartNew();

            _loggerService.LogInformation("Starting Extraction...");

            // Se recorren todos los extractores registrados
            foreach (var extractor in _extractors)
            {
                try
                {
                    _loggerService.LogInformation($"Executing Extraction: {extractor.SourceName}");

                    var result = await extractor.ExtractAsync(cancellationToken);

                    metrics.Results.Add(result);

                    metrics.TotalSources++;

                    metrics.TotalRecords += result.RecordsExtracted;

                    _loggerService.LogInformation($"Extraction completed: {extractor.SourceName}");
                }
                catch (Exception ex)
                {
                    // Si un extractor falla, se registra el error pero el proceso no se cae en ese momento
                    _loggerService.LogError($"Error running the Extraction: {extractor.SourceName}", ex);

                    // Se agrega el resultado fallido para dejar evidencia en métricas
                    metrics.Results.Add(new ExtractionResult
                    {
                        SourceName = extractor.SourceName,
                        WasSuccessful = false,
                        RecordsExtracted = 0,
                        Message = ex.Message,
                        StartedAt = DateTime.Now,
                        EndedAt = DateTime.Now,
                        DurationInMilliseconds = 0
                    });

                    metrics.TotalSources++;
                }
            }

            stopwatch.Stop();

            metrics.EndedAt = DateTime.Now;
            metrics.TotalDurationInMilliseconds = stopwatch.ElapsedMilliseconds;

            _loggerService.LogInformation("Extraction completed.");

            return metrics;
        }
    }
}