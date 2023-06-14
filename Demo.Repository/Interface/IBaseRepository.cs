using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface
{
    public interface IBaseRepository<TEntity>
    where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);

        //Task AddRangeAsync(TEntity[] entities);
        Task<TEntity> UpdateAsync(TEntity entity);

        //Task UpdateRangeAsync(TEntity[] entities);
        Task<TEntity?> ByIdAsync(long id);

        //Task<TEntity> RequireByIdAsync(Guid id);
        Task<bool> RemoveAsync(TEntity entity);


    }
}
