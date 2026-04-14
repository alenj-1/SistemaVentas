using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Interfaces
{
    public interface IDataLoader
    {
        Task LoadAsync(CancellationToken cancellationToken = default);  
    }
}
