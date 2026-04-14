using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Database
{
    public class CategoryDb
    {
        public int CategoryID { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
