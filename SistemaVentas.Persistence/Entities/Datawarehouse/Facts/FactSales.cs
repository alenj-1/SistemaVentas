using SistemaVentas.Persistence.Entities.Datawarehouse.Dimensions;

namespace SistemaVentas.Persistence.Entities.Datawarehouse.Facts
{
    public class FactSales
    {
        public long SalesKey { get; set; }
        public int DateKey { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int? LocationKey { get; set; }
        public int StatusKey { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesAmount { get; set; }
        public DimDate? DateNavigation { get; set; }
        public DimCustomer? CustomerNavigation { get; set; }
        public DimProduct? ProductNavigation { get; set; }
        public DimLocation? LocationNavigation { get; set; }
        public DimOrderStatus? StatusNavigation { get; set; }
    }
}