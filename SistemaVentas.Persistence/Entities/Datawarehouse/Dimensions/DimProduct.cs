using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Dimensions
{
    public class DimProduct
    {
        public int ProductKey { get; set; }
        public int ProductID_NaturalKey { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryKey { get; set; }
        public decimal ListPrice { get; set; }
        public int Stock { get; set; }
    }
}
