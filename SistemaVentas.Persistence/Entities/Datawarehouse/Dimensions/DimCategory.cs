using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Dimensions
{
    public class DimCategory
    {
        public int CategoryKey { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
