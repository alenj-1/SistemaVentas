namespace SistemaVentas.Application.Interfaces
{
    public interface IDatabaseReader<T>
    {
        Task<List<T>> ReadAsync(string query, CancellationToken cancellationToken = default);
    }
}