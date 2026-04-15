using System.Diagnostics;
using Microsoft.Extensions.Options;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Models;
using SistemaVentas.Infrastructure.Settings;

namespace SistemaVentas.Infrastructure.Extractors
{
    public class DatabaseExtractor : IExtractor
    {
        private readonly IDatabaseReader<Dictionary<string, object?>> _databaseReader;
        private readonly IStagingService _stagingService;
        private readonly ILoggerService _loggerService;

        private readonly DatabaseSettings _databaseSettings;


        public string SourceName => "Data Warehouse";


        public DatabaseExtractor(
            IDatabaseReader<Dictionary<string, object?>> databaseReader,
            IStagingService stagingService,
            ILoggerService loggerService,
            IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseReader = databaseReader;
            _stagingService = stagingService;
            _loggerService = loggerService;
            _databaseSettings = databaseOptions.Value;
        }


        // Ejecuta la extracción y guarda el resultado en staging
        public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var startedAt = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _loggerService.LogInformation("Starting Extraction process...");

                // Se ejecuta el query
                var data = await _databaseReader.ReadAsync(_databaseSettings.ExtractionQuery, cancellationToken);

                // Se guarda el resultado en staging
                var stagingResult = await _stagingService.SaveAsync(data, "dw_extraction.json", cancellationToken);

                stopwatch.Stop();

                _loggerService.LogInformation("Extraction completed successfully.");

                return new ExtractionResult
                {
                    SourceName = SourceName,
                    WasSuccessful = true,
                    RecordsExtracted = data.Count,
                    Message = "Extraction completed successfully.",
                    StagingFilePath = stagingResult.FilePath,
                    StartedAt = startedAt,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _loggerService.LogError("Error during extraction process.", ex);

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