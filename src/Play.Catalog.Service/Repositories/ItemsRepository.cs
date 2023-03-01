using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository : IItemsRepository
{
    private const string CollectionName = "Items";
    private readonly IMongoCollection<Item> collection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemsRepository(IMongoDatabase database)
    {
        // var client = new MongoClient("mongodb://localhost:27017");
        // var database = client.GetDatabase("Catalog");
        collection = database.GetCollection<Item>(CollectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {

        var filter = filterBuilder.Empty;
        var items = await collection.Find(filter).ToListAsync();
        return items;
    }

    public async Task<Item> GetByIdAsync(Guid id)
    {
        var filter = filterBuilder.Eq(entity => entity.Id, id);
        var item = await collection.Find(filter).SingleOrDefaultAsync();
        return item;
    }

    public async Task CreateAsync(Item item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        await collection.InsertOneAsync(item);
    }

    public async Task UpdateAsync(Item item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var filter = filterBuilder.Eq(entity => entity.Id, item.Id);
        await collection.ReplaceOneAsync(filter, item);
    }

    public async Task DeleteAsync(Item item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var filter = filterBuilder.Eq(entity => entity.Id, item.Id);
        await collection.DeleteOneAsync(filter);
    }
}