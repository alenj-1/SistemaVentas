using SistemaVentas.Application.Interfaces;
using SistemaVentas.Persistence.Context;

namespace SistemaVentas.Persistence.Loaders
{
    public class DataLoader : IDataLoader
    {
        private readonly SistemaVentasDbContext _databaseContext;
        private readonly DataWarehouseDbContext _dataWarehouseContext;

        public DataLoader(SistemaVentasDbContext databaseContext, DataWarehouseDbContext dataWarehouseContext)
        {
            _databaseContext = databaseContext;
            _dataWarehouseContext = dataWarehouseContext;
        }


        // Carga una colección de datos en el destino indicado
        public async Task LoadAsync<T>(IEnumerable<T> data, string destinationName, CancellationToken cancellationToken = default) where T : class
        {
            if (data == null || !data.Any())
            {
                return;
            }

            // Se decide en cuál base se insertarán los datos
            if (destinationName.Equals("Database", StringComparison.OrdinalIgnoreCase))
            {
                await _databaseContext.Set<T>().AddRangeAsync(data, cancellationToken);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }
            else if (destinationName.Equals("DataWarehouse", StringComparison.OrdinalIgnoreCase))
            {
                await _dataWarehouseContext.Set<T>().AddRangeAsync(data, cancellationToken);
                await _dataWarehouseContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                // Si el destino no coincide con ninguno, se lanza un error
                throw new ArgumentException("The specified destination is invalid. Use 'Database' or 'DataWarehouse'.");
            }
        }
    }
}