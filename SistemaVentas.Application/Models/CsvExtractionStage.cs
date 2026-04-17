using SistemaVentas.Application.SourceModels.Csv;

namespace SistemaVentas.Application.Models
{
    public class CsvExtractionStage
    {
        public List<Customer> Customers { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<OrderDetails> OrderDetails { get; set; } = new();
    }
}