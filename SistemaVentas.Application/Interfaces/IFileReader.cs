namespace SistemaVentas.Application.Interfaces
{
    public interface IFileReader<T>
    {
        Task<List<T>> ReadAsync(string filePath, CancellationToken cancellationToken = default);
    }
}