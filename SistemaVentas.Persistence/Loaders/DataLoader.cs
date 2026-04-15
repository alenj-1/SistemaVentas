using SistemaVentas.Application.Interfaces;
using SistemaVentas.Persistence.Context;

namespace SistemaVentas.Persistence.Loaders
{
    public class DataLoader : IDataLoader
    {
        private readonly DataWarehouseDbContext _dataWarehouseContext;

        public DataLoader(DataWarehouseDbContext dataWarehouseContext)
        {
            _dataWarehouseContext = dataWarehouseContext;
        }

        // Carga una colección de datos en el Data Warehouse
        public async Task LoadAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class
        {
            if (data == null || !data.Any())
            {
                return;
            }

            // Se insertan los datos en el DW
            await _dataWarehouseContext.Set<T>().AddRangeAsync(data, cancellationToken);

            // Se guardan los cambios
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);
        }
    }
}