using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Database
{
    public class CityDb
    {
        public int CityID { get; set; }
        public string City { get; set; } = string.Empty;
        public int CountryID { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}
