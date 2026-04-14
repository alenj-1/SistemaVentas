using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Database
{
    public class ProductDb
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
