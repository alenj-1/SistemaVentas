using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.Application.Interfaces
{
    public interface IDatabaseReader
    {
        Task<IEnumerable<T>> QueryAsync<T> (
            string sql,
            Func<System.Data.IDataReader, T> map,
            CancellationToken cancellationToken = default
            );
    }
}
