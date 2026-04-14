using SistemaVentas.Application.Models;

namespace SistemaVentas.Application.Interfaces
{
    public interface IExtractor
    {
        string SourceName { get; }
        Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default);
    }
}