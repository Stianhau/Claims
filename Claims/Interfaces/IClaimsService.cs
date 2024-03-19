using Claims;

public interface IClaimsService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim?> GetClaimAsync(string id);
        Task<Claim> AddClaimAsync(Claim claim);
        Task<Claim?> DeleteClaimAsync(string id);
    }