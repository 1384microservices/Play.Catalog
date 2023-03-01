using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public interface IItemsRepository
{
    Task CreateAsync(Item item);
    Task DeleteAsync(Item item);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item> GetByIdAsync(Guid id);
    Task UpdateAsync(Item item);
}
