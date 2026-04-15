namespace SistemaVentas.Application.Interfaces
{
    public interface IDataLoader
    {
        Task LoadAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class;
    }
}