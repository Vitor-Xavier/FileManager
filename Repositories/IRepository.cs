namespace FileManager.Repositories
{
    public interface IRepository<TEntity>
    {
        ValueTask<TEntity> GetById(int id, CancellationToken cancellationToken = default);

        Task Add(TEntity entity, CancellationToken cancellationToken = default);

        Task Delete(TEntity Entity, CancellationToken cancellationToken = default);

        Task Edit(TEntity entity, CancellationToken cancellationToken = default);
    }
}
