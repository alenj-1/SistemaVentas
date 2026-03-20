using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Data.Interfaces
{
    public interface IFileReader
    {
        Task<IEnumerable<T>> ReadAsync<T>(string filePath, CancellationToken cancellationToken = default);
    }
}
