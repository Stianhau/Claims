using Claims;


public class ClaimRepositoryMock : IClaimRepository
{
    private readonly Dictionary<string, Claim> _claims;

    public ClaimRepositoryMock(Dictionary<string, Claim> claims)
    {
        _claims = claims;
    }

    public Task<Claim> AddAsync(Claim entity)
    {
        _claims.Add(entity.Id, entity);
        return Task.FromResult(entity);
    }

    public Task<Claim?> DeleteAsync(string id)
    {
        var hasClaim = _claims.ContainsKey(id);
        if(hasClaim) return Task.FromResult<Claim?>(_claims[id]);
        return Task.FromResult<Claim?>(null);
    }

    public Task<IEnumerable<Claim>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Claim>>(_claims.Values.ToList());
    }

    public Task<Claim?> GetAsync(string id)
    {
        var hasClaim = _claims.ContainsKey(id);
        if(hasClaim) return Task.FromResult<Claim?>(_claims[id]);
        return Task.FromResult<Claim?>(null);
    }
}

