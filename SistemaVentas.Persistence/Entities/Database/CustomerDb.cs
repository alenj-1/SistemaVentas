namespace SistemaVentas.Persistence.Entities.Database
{
    public class CustomerDb
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? CityID { get; set; }
        public CityDb? CityNavigation { get; set; }
        public ICollection<OrderDb> Orders { get; set; } = new List<OrderDb>();
    }
}