using System.Linq.Expressions;
using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Application.Contracts.Persistence;

public interface IRepository<TEntity> where TEntity : BaseEntity
{

    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(Guid id);
    Task CreateAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);

    Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> GetAllFilterAsync(Expression<Func<TEntity, bool>> filter);

    Task<List<TEntity>> GetLatestAsync(int count);
    Task<List<TEntity>> GetLatestAsync(Expression<Func<TEntity, bool>> filter, int count);

}
