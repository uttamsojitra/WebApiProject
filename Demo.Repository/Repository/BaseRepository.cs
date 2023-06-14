using Dapper;
using Demo.Business.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
     where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {

            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        //public async Task AddRangeAsync(TEntity[] entities)
        //{
        //    await _dbSet.AddRangeAsync(entities);
        //    await _dbContext.SaveChangesAsync();
        //}

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;

        }

        //public async Task UpdateRangeAsync(TEntity[] entities)
        //{
        //    foreach (var entity in entities)
        //    {
        //        _dbContext.Entry(entity).State = EntityState.Modified;
        //    }
        //    await _dbContext.SaveChangesAsync();
        //}

        public async Task<TEntity?> ByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

    }
}
