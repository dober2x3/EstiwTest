using System;
using System.ComponentModel;

namespace Lx.Data.Repository
{
    public interface IUnitOfWork<TEntity>
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        event EventHandler HasChangesChanged;
        bool HasChanges { get; }
        void AcceptChanges();
        void RejectChanges();
        IChangeTracker<TEntity> ChangeTracker { get; }

        event EventHandler<SaveCompletedEventArgs> SaveChangesCompleted;
        //void SaveChangesAsync();
        //void SaveChanges();
    }

    public class SaveCompletedEventArgs : AsyncCompletedEventArgs
    {
        public SaveCompletedEventArgs(bool result, Exception error, bool canceled, object userState)
            : base(error, canceled, userState)
        {
            Result = result;
        }

        public bool Result { get; private set; }
    }
}
