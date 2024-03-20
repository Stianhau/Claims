using Claims;

public interface IClaimRepository
{
    Task<Claim?> GetAsync(string id);
    Task<IEnumerable<Claim>> GetAllAsync();
    Task<Claim> AddAsync(Claim entity);
    Task<bool> DeleteAsync(string id);
}