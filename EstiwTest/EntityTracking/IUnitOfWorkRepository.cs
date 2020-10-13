
namespace Lx.Data.Repository
{
    public interface IUnitOfWorkRepository<TEntity> : IRepository<TEntity>, IUnitOfWork<TEntity>
    { }
}
