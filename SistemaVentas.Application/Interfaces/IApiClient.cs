using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Application.Interfaces
{
    public interface IApiClient
    {
        Task<IEnumerable<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    }
}
