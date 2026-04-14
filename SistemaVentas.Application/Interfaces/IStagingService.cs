using SistemaVentas.Application.Models;

namespace SistemaVentas.Application.Interfaces
{
    public interface IStagingService
    {
        Task<StagingFileResult> SaveAsync<T>(IEnumerable<T> data, string fileName, CancellationToken cancellationToken = default);
    }
}