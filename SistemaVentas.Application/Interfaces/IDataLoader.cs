namespace SistemaVentas.Application.Interfaces
{
    public interface IDataLoader
    {
        Task LoadAsync<T>(IEnumerable<T> data, string destinationName, CancellationToken cancellationToken = default) where T : class;
    }
}