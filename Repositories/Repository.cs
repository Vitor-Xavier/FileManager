using Microsoft.EntityFrameworkCore;

namespace FileManager.Repositories
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        protected readonly TContext _context;

        public Repository(TContext context) => _context = context;

        public ValueTask<TEntity> GetById(int id, CancellationToken cancellationToken = default) =>
            _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);

        public async Task Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Detached;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Edit(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
