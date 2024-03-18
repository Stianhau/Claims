using Claims;

public interface IClaimsService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> GetClaimAsync(string id);
        Task AddClaimAsync(Claim claim);
        Task DeleteClaimAsync(string id);
    }