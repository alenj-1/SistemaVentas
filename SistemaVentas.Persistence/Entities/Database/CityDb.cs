namespace SistemaVentas.Persistence.Entities.Database
{
    public class CityDb
    {
        public int CityID { get; set; }
        public string City { get; set; } = string.Empty;
        public int CountryID { get; set; }
        public CountryDb? CountryNavigation { get; set; }
        public ICollection<CustomerDb> Customers { get; set; } = new List<CustomerDb>();
    }
}