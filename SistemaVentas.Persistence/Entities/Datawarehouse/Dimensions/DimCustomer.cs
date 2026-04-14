using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Dimensions
{
    public class DimCustomer
    {
        public int CustomerKey { get; set; }
        public int CustomerID_NaturalKey { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? LocationKey { get; set; }
    }
}
