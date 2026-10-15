using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities.Common;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete
{
    public class GenericRepository<TEntity>(AppDbContext _context) : IRepository<TEntity> where TEntity : BaseEntity
    {

        private readonly DbSet<TEntity> _table = _context.Set<TEntity>();

        public async Task CreateAsync(TEntity entity)
        {
   
            await _context.AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Remove(entity);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _table.AsNoTracking().ToListAsync();  //ChangeTracking mekanizmasý durssun
        }

        public async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table.AsNoTracking();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();

            //var blogs = await _repository.GetAllAsync(x => x.Category, x => x.AppUser); kullanýmý
        }

        public async Task<List<TEntity>> GetAllFilterAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _table.AsNoTracking().Where(filter).ToListAsync();

        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<List<TEntity>> GetLatestAsync(int count)
        {
            return await _table.AsNoTracking()
                               .OrderByDescending(x=>x.CreatedAt)
                               .Take(count)
                               .ToListAsync();


        }

        public async Task<List<TEntity>> GetLatestAsync(Expression<Func<TEntity, bool>> filter, int count)
        {
            return await _table.AsNoTracking()
                               .Where(filter)
                               .OrderByDescending(x => x.CreatedAt)
                               .Take(count)
                               .ToListAsync();
        }


        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }
    
        
    
    }
}
