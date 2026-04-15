using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.SourceModels.Api;
using SistemaVentas.Infrastructure.Settings;
using System.Net.Http.Json;

namespace SistemaVentas.Infrastructure.ApiClients
{
    public class OrderTrackingApiClient : IApiClient<OrderTracking>
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public OrderTrackingApiClient(HttpClient httpClient, ApiSettings apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings;
        }


        // Obtiene la lista de seguimiento de pedidos desde la API
        public async Task<List<OrderTracking>> GetAsync(CancellationToken cancellationToken = default)
        {
            // Se crea la ruta del endpoint
            var endpoint = $"{_apiSettings.BaseUrl.TrimEnd('/')}/{_apiSettings.OrderTrackingEndpoint.TrimStart('/')}";

            // Petición GET a la API
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Se convierten los datos JSON en una lista del modelo OrderTracking
            var data = await response.Content.ReadFromJsonAsync<List<OrderTracking>>(cancellationToken: cancellationToken);
            return data ?? new List<OrderTracking>();
        }
    }
}