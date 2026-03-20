using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Dimensions
{
    public class DimOrderStatus
    {
        public int StatusKey { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
