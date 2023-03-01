using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository
{
    private const string CollectionName = "Items";
    private readonly IMongoCollection<Item> collection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemsRepository()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("Catalog");
        var collection = database.GetCollection<Item>(CollectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAll()
    {
        return await collection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<Item> GetAsync(Guid id)
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