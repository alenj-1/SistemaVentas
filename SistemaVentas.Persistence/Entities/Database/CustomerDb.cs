using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Database
{
    public class CustomerDb
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? CityID { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
