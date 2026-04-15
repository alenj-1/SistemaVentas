using Microsoft.Data.SqlClient;
using SistemaVentas.Application.Interfaces;

namespace SistemaVentas.Infrastructure.Readers
{
    public class DatabaseReader : IDatabaseReader<Dictionary<string, object?>>
    {
        private readonly string _connectionString;

        public DatabaseReader(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Ejecuta un query y devuelve los resultados como lista de diccionarios
        public async Task<List<Dictionary<string, object?>>> ReadAsync(string query, CancellationToken cancellationToken = default)
        {
            var results = new List<Dictionary<string, object?>>();

            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(query, connection);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Se recorren todas las filas
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Se guarda el nombre de la columna y su valor
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }

                results.Add(row);
            }

            return results;
        }
    }
}