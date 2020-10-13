using System;
using System.Collections.Generic;
using System.Linq;

namespace Lx.Data.Repository
{


    public class EntityRepository<TEntity> : RepositoryBase<TEntity> 
        where TEntity : class, IEntity
    {
        protected override void OnItemAdded(TEntity entity)
        {
            base.OnItemAdded(entity);

            if (entity != null)
            {
                entity.HasChangesChanged += new EventHandler(Entity_HasChangesChanged);

                if (entity.HasChanges)
                {
                    base.Update(entity);
                }
            }
        }

        protected override void OnItemRemoved(TEntity entity)
        {
            base.OnItemRemoved(entity);

            if (entity != null)
            {
                entity.HasChangesChanged -= Entity_HasChangesChanged;
            }
        }

        private void Entity_HasChangesChanged(object sender, EventArgs e)
        {
            TEntity entity = sender as TEntity;

            lock (SyncRoot)
            {
                if (entity.HasChanges)
                {
                    base.Update(entity);
                }
                else if (ChangeTracker.ContainsChanged(entity))
                {
                    ChangeTracker.RemoveChanged(entity);
                }

                NotifyHasChangesChangedListeners();
            }
        }

        public override void AcceptChanges()
        {
            IEnumerable<TEntity> modifiedItems = ChangeTracker.Inserted.Union(ChangeTracker.Changed);
            foreach (TEntity entity in modifiedItems)
            {
                entity.AcceptChanges();
            }

            base.AcceptChanges();
        }

        public override void RejectChanges()
        {
            IEnumerable<TEntity> modifiedItems = ChangeTracker.Deleted.Union(ChangeTracker.Changed);
            foreach (TEntity entity in modifiedItems)
            {
                entity.RejectChanges();
            }

            base.RejectChanges();
        }


    }
}
