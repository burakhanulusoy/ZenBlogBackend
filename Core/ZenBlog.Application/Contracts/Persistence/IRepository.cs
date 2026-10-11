using System.Linq.Expressions;
using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Application.Contracts.Persistence;

public interface IRepository<TEntity> where TEntity : BaseEntity
{

    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(Guid id);
    Task CreateAsync(TEntity entity);
    void UpdateAsync(TEntity entity);
    void DeleteAsync(TEntity entity);
    Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> filter);

}
