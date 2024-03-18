
using Claims;
using Microsoft.Azure.Cosmos;

public class CoverRepository : ICosmosRepository<Cover>
    {
    private readonly Container _container;

    public CoverRepository(Container container)
    {
        _container = container;
    }

    public async Task<Cover> AddAsync(Cover entity)
    {
        return await _container.CreateItemAsync(entity, new PartitionKey(entity.Id));
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Cover>(id, new PartitionKey(id));
    }

    public async Task<IEnumerable<Cover>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<Cover> GetAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Cover>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}