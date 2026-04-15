namespace SistemaVentas.Infrastructure.Settings
{
    public class CsvSettings
    {
        public string BasePath { get; set; } = string.Empty;
        public string CustomersFile { get; set; } = string.Empty;
        public string ProductsFile { get; set; } = string.Empty;
        public string OrdersFile { get; set; } = string.Empty;
        public string OrderDetailsFile { get; set; } = string.Empty;
    }
}