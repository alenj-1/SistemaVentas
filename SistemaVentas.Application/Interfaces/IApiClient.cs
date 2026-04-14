namespace SistemaVentas.Application.Interfaces
{
    public interface IApiClient<T>
    {
        Task<List<T>> GetAsync(CancellationToken cancellationToken = default);
    }
}