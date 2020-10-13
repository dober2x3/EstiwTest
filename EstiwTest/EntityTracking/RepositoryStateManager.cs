using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Lx.Data.Repository
{
    public sealed class RepositoryStateManager<TEntity> : IChangeTracker<TEntity>
    {
        private readonly object _syncRoot = new object();

        private void OnStateChanged()
        {
            lock (_syncRoot)
            {
                HasChanges = _deleted.Count > 0 || _changed.Count > 0 || _inserted.Count > 0;
            }
        }

        public event EventHandler HasChangesChanged;
        private void NotifyHasChangesChangedListeners()
        {
            EventHandler handler = HasChangesChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private bool _hasChanges;
        public bool HasChanges 
        { 
            get
            {
                return _hasChanges;
            }
            private set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    NotifyHasChangesChangedListeners();
                }
            }
        }

        public void RemoveItem(TEntity item)
        {
            lock (_syncRoot)
            {
                _inserted = _inserted.Remove(item);
                _changed = _changed.Remove(item);
                _deleted = _deleted.Remove(item);

                OnStateChanged();
            }
        }

        private IImmutableSet<TEntity> _deleted = ImmutableHashSet<TEntity>.Empty;
        public IReadOnlyCollection<TEntity> Deleted
        {
            get
            {
                return _deleted;
            }
        }
        public bool ContainsDeleted(TEntity entity)
        {
            return _deleted.Contains(entity);
        }
        public void RemoveDeleted(TEntity entity)
        {
            lock (_syncRoot)
            {
                _deleted = _deleted.Remove(entity);

                OnStateChanged();
            }
        }
        public void Delete(TEntity entity)
        {
            lock (_syncRoot)
            {
                _deleted = _deleted.Add(entity);

                OnStateChanged();
            }
        }

        private IImmutableSet<TEntity> _changed = ImmutableHashSet<TEntity>.Empty;
        public IReadOnlyCollection<TEntity> Changed
        {
            get
            {
                return _changed;
            }
        }
        public bool ContainsChanged(TEntity entity)
        {
            return _changed.Contains(entity);
        }
        public void RemoveChanged(TEntity entity)
        {
            lock (_syncRoot)
            {
                _changed = _changed.Remove(entity);

                OnStateChanged();
            }
        }
        public void Update(TEntity entity)
        {
            lock (_syncRoot)
            {
                _changed = _changed.Add(entity);

                OnStateChanged();
            }
        }

        private IImmutableSet<TEntity> _inserted = ImmutableHashSet<TEntity>.Empty;
        public IReadOnlyCollection<TEntity> Inserted
        {
            get
            {
                return _inserted;
            }
        }
        public bool ContainsInserted(TEntity entity)
        {
            return _inserted.Contains(entity);
        }
        public void RemoveInserted(TEntity entity)
        {
            lock (_syncRoot)
            {
                _inserted = _inserted.Remove(entity);

                OnStateChanged();
            }
        }
        public void Insert(TEntity entity)
        {
            lock (_syncRoot)
            {
                _inserted = _inserted.Add(entity);

                OnStateChanged();
            }
        }

        public void Reset()
        {
            lock (_syncRoot)
            {
                _changed = _changed.Clear();
                _inserted = _inserted.Clear();
                _deleted = _deleted.Clear();

                HasChanges = false;
            }
        }
    }
}
