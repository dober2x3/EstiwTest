using System;
using System.Collections.Generic;
using System.Linq;

namespace Lx.Data.Repository
{
    public interface IRepository<TEntity>
    {
        IReadOnlyCollection<TEntity> GetAll();
        bool TryAdd(TEntity entity);
        bool TryRemove(TEntity entity);
        bool ContainsItem(TEntity entity);
        void AddItem(TEntity entity);
        void RemoveItem(TEntity entity);
        void ClearItems();
        TEntity Find(Func<TEntity, bool> expression);
        IQueryable<TEntity> FindAll(Func<TEntity, bool> expression);
    }
}
