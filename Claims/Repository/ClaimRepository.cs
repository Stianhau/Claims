using Claims;
using Microsoft.Azure.Cosmos;

public class ClaimRepository : IClaimRepository
    {
    private readonly Container _container;

    public ClaimRepository(Container container)
    {
        _container = container;
    }

    public async Task<Claim> AddAsync(Claim entity)
    {
        return await _container.CreateItemAsync(entity, new PartitionKey(entity.Id));
    }

    public async Task<bool> DeleteAsync(string id)
    {   
        try
        {
            await _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<IEnumerable<Claim>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Claim>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<Claim?> GetAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}