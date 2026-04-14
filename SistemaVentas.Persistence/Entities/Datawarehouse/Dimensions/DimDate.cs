using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Entities.Datawarehouse.Dimensions
{
    public class DimDate
    {
        public int DateKey { get; set; }
        public DateTime Date { get; set; }
        public short Year { get; set; }
        public byte Quarter { get; set; }
        public byte Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public byte DayOfMonth { get; set; }
        public string DayName { get; set; } = string.Empty;
    }
}
