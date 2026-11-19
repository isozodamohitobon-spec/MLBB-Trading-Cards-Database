
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrisProject.ORM.Interfaces
{
    public interface IOrmContext
    {
        Task<List<T>> GetAllAsync<T>(
            string tableName,
            CancellationToken cancellationToken = default
        ) where T : class, new();

        Task InsertAsync<T>(
            string tableName,
            T entity,
            CancellationToken cancellationToken = default
        ) where T : class;

        Task UpdateAsync<T>(
            string tableName,
            int id,
            T entity,
            CancellationToken cancellationToken = default
        ) where T : class;

        Task DeleteAsync(
            string tableName,
            int id,
            CancellationToken cancellationToken = default
        );
    }
}
