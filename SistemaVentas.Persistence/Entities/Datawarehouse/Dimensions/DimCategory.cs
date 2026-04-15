namespace SistemaVentas.Persistence.Entities.Datawarehouse.Dimensions
{
    public class DimCategory
    {
        public int CategoryKey { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ICollection<DimProduct> Products { get; set; } = new List<DimProduct>();
    }
}