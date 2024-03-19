using Claims;


public class CoverRepositoryMock : ICoverRepository
{
    private readonly Dictionary<string, Cover> _cover;

    public CoverRepositoryMock(Dictionary<string, Cover> cover)
    {
        _cover = cover;
    }

    public Task<Cover> AddAsync(Cover entity)
    {
        _cover.Add(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task<Cover?> DeleteAsync(string id)
    {
        return Task.FromResult<Cover?>(_cover[id]);
    }

    public Task<IEnumerable<Cover>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Cover>>(_cover.Values.ToList());
    }

    public Task<Cover?> GetAsync(string id)
    {
        var hasClaim = _cover.ContainsKey(id);
        if(hasClaim) return Task.FromResult<Cover?>(_cover[id]);
        return Task.FromResult<Cover?>(null);
    }
}

