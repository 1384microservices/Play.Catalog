using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task CreateAsync(T item);
    Task DeleteAsync(T item);
    Task UpdateAsync(T item);
}
