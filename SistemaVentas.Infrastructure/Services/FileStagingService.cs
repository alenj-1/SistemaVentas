using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Models;
using SistemaVentas.Infrastructure.Settings;
using System.Collections;
using System.Text.Json;

namespace SistemaVentas.Infrastructure.Services
{
    public class FileStagingService : IStagingService
    {
        private readonly StagingSettings _stagingSettings;

        public FileStagingService(StagingSettings stagingSettings)
        {
            _stagingSettings = stagingSettings;
        }


        // Guarda datos en un archivo JSON dentro de la carpeta staging
        public async Task<StagingFileResult> SaveAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(_stagingSettings.FolderPath))
            {
                Directory.CreateDirectory(_stagingSettings.FolderPath);
            }

            // Se genera el nombre final con fecha y hora
            var finalFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            // Ruta completa del archivo
            var fullPath = Path.Combine(_stagingSettings.FolderPath, finalFileName);

            // Se convierten los datos a JSON
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(fullPath, json, cancellationToken);

            // Se calcula la cantidad de registros si el objeto es una colección
            int recordsSaved = 1;

            if (data is IEnumerable collection && data is not string)
            {
                recordsSaved = 0;

                foreach (var item in collection)
                {
                    recordsSaved++;
                }
            }

            // Se devuelve el resultado
            return new StagingFileResult
            {
                WasCreated = true,
                FilePath = fullPath,
                RecordsSaved = recordsSaved,
                Message = "Staging file created successfully."
            };
        }
    }
}