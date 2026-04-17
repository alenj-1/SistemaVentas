using SistemaVentas.Application.Models;

namespace SistemaVentas.Application.Interfaces
{
    public interface IDataWarehouseLoadService
    {
        Task<LoadProcessResult> LoadDimensionsAsync(string stagingFilePath, CancellationToken cancellationToken = default);
        Task<LoadProcessResult> LoadFactsAsync(string stagingFilePath, CancellationToken cancellationToken = default);
    }
}