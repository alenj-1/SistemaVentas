using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Facts
{
    public class FactSales
    {
        public long SalesKey { get; set; }
        public int DateKey { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int? LocationKey { get; set; }
        public int StatusKey { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesAmount { get; set; }
    }
}
