using CsvHelper.Configuration;
using SistemaVentas.Application.Interfaces;
using System.Globalization;

namespace SistemaVentas.Infrastructure.Readers
{
    public class CsvReader<T> : IFileReader<T>
    {
        // Lee un archivo CSV y lo convierte en una lista del tipo indicado
        public async Task<List<T>> ReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file was not found: {filePath}");
            }

            var records = new List<T>();

            using var reader = new StreamReader(filePath);


            // Configuración básica del lector CSV
            using var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });


            // Se leen los registros y se convierten al tipo T
            await foreach (var record in csv.GetRecordsAsync<T>(cancellationToken))
            {
                records.Add(record);
            }

            return records;
        }
    }
}