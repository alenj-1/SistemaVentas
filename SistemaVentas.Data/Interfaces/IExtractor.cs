using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Interfaces
{
    public interface IExtractor<T>
    {
        Task<IEnumerable<T>> Extract(CancellationToken cancellationToken = default);
    }
}
