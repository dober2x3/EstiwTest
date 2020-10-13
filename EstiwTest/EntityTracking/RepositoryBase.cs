using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Lx.Data.Repository
{
    public abstract class RepositoryBase<TEntity> : IUnitOfWorkRepository<TEntity>
    {
        #region Constructors

        protected RepositoryBase()
        {
            ChangeTracker.HasChangesChanged += new EventHandler(ChangeTracker_HasChangesChanged);
        }

        private void ChangeTracker_HasChangesChanged(object sender, EventArgs e)
        {
            NotifyHasChangesChangedListeners();
        }

        #endregion

        #region Properties

        private readonly RepositoryStateManager<TEntity> _changeTracker = new RepositoryStateManager<TEntity>();
        public IChangeTracker<TEntity> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

        #endregion

        #region Protected Members

        private readonly object _syncRoot = new object();
        protected object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        private IImmutableSet<TEntity> _items = ImmutableHashSet<TEntity>.Empty;
        protected IImmutableSet<TEntity> InternalItems
        {
            get
            {
                return _items;
            }
        }

        #endregion

        #region IRepository<TEntity> Members

        public IReadOnlyCollection<TEntity> GetAll()
        {
            return _items;
        }

        public bool TryAdd(TEntity entity)
        {
            bool result = false;
            lock (_syncRoot)
            {
                if (!ContainsItem(entity))
                {
                    AddItem(entity);
                    result = true;
                }
            }

            return !result;
        }

        public bool TryRemove(TEntity entity)
        {
            bool result = false;
            lock (_syncRoot)
            {
                if (ContainsItem(entity))
                {
                    RemoveItem(entity);
                    result = true;
                }
            }

            return result;
        }
        
        public bool ContainsItem(TEntity entity)
        {
            return InternalItems.Contains(entity);
        }

        public void AddItem(TEntity entity)
        {
            lock (_syncRoot)
            {
                if (!InternalItems.Contains(entity))
                {
                    AddItemInternal(entity);
                }
                else
                {
                    throw new InvalidOperationException("Entity has already been added to repository");
                }
            }
        }
        private void AddItemInternal(TEntity entity)
        {
            _items = _items.Add(entity);

            OnItemAdded(entity);
        }
        protected virtual void OnItemAdded(TEntity entity)
        { }

        public void RemoveItem(TEntity entity)
        {
            lock (_syncRoot)
            {
                if (InternalItems.Contains(entity) || ChangeTracker.ContainsDeleted(entity))
                {
                    RemoveItemInternal(entity);

                    ChangeTracker.RemoveItem(entity);
                }
            }
        }
        private void RemoveItemInternal(TEntity entity)
        {
            _items = _items.Remove(entity);

            OnItemRemoved(entity);
        }
        protected virtual void OnItemRemoved(TEntity entity)
        { }

        public void ClearItems()
        {
            lock (_syncRoot)
            {
                List<TEntity> itemsCopy = new List<TEntity>(InternalItems.Union(ChangeTracker.Deleted));
                foreach (TEntity item in itemsCopy)
                {
                    RemoveItem(item);
                }
            }
        }

        public TEntity Find(Func<TEntity, bool> expression)
        {
            return InternalItems.Where(expression).SingleOrDefault();
        }

        public IQueryable<TEntity> FindAll(Func<TEntity, bool> expression)
        {
            return InternalItems.Where(expression).AsQueryable();
        }

        #endregion

        #region IUnitOfWork<TEntity> Members

        public virtual void RejectChanges()
        {
            foreach (TEntity entity in ChangeTracker.Deleted)
            {
                AddItem(entity);
            }

            foreach (TEntity entity in ChangeTracker.Inserted)
            {
                RemoveItem(entity);
            }

            ChangeTracker.Reset();
        }

        public virtual void AcceptChanges()
        {
            ChangeTracker.Reset();
        }

        public event EventHandler HasChangesChanged;
        protected void NotifyHasChangesChangedListeners()
        {
            EventHandler handler = HasChangesChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
        public bool HasChanges
        {
            get
            {
                return ChangeTracker.HasChanges;
            }
        }

        public virtual void Insert(TEntity entity)
        {
            lock (_syncRoot)
            {
                if (!InternalItems.Contains(entity))
                {
                    ChangeTracker.Insert(entity);
                    AddItemInternal(entity);
                }
                else
                {
                    throw new InvalidOperationException("Entity has not yet been added to repository");
                }
            }
        }

        public virtual void Update(TEntity entity)
        {
            lock (_syncRoot)
            {
                if (InternalItems.Contains(entity))
                {
                    if (ChangeTracker.ContainsDeleted(entity))
                    {
                        ChangeTracker.RemoveDeleted(entity);
                    }

                    if (!ChangeTracker.ContainsInserted(entity) && !ChangeTracker.ContainsChanged(entity))
                    {
                        ChangeTracker.Update(entity);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Entity has not yet been added to repository");
                }
            }
        }

        public virtual void Delete(TEntity entity)
        {
            lock (_syncRoot)
            {
                if (InternalItems.Contains(entity))
                {
                    bool doDelete = true;

                    if (ChangeTracker.ContainsInserted(entity))
                    {
                        ChangeTracker.RemoveInserted(entity);
                        doDelete = false;
                    }

                    if (ChangeTracker.ContainsChanged(entity))
                    {
                        ChangeTracker.RemoveChanged(entity);
                    }

                    if (doDelete)
                    {
                        ChangeTracker.Delete(entity);
                    }

                    RemoveItemInternal(entity);
                }
                else
                {
                    throw new InvalidOperationException("Entity has not yet been added to repository");
                }
            }
        }

        public event EventHandler<SaveCompletedEventArgs> SaveChangesCompleted;
        //public abstract void SaveChangesAsync();
        //public abstract void SaveChanges();

        #endregion
    }
}
