using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Database
{
    public class OrderStatusDb
    {
        public int StatusID { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
