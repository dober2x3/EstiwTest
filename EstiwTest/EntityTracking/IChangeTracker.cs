using System;
using System.Collections.Generic;

namespace Lx.Data.Repository
{
    public interface IChangeTracker<TEntity>
    {
        IReadOnlyCollection<TEntity> Inserted { get; }
        void Insert(TEntity entity);
        bool ContainsInserted(TEntity entity);
        void RemoveInserted(TEntity entity);

        IReadOnlyCollection<TEntity> Changed { get; }
        void Update(TEntity entity);
        bool ContainsChanged(TEntity entity);
        void RemoveChanged(TEntity entity);

        IReadOnlyCollection<TEntity> Deleted { get; }
        void Delete(TEntity entity);
        bool ContainsDeleted(TEntity entity);
        void RemoveDeleted(TEntity entity);

        event EventHandler HasChangesChanged;
        bool HasChanges { get; }

        void RemoveItem(TEntity item);
        void Reset();
    }
}
