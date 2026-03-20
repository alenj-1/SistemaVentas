using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Interfaces
{
    public interface IStagingService
    {
        Task SaveAsync<T>(
            string sourceName,
            IEnumerable<T> data,
            CancellationToken cancellationToken = default
            );
    }
}
