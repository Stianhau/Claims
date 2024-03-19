using Claims;

public class ClaimsService : IClaimsService
{
    private readonly IClaimRepository _repository;

    private readonly ICoverService _coverService;

    public ClaimsService(IClaimRepository repository, ICoverService coverService)
    {
        _coverService = coverService;
        _repository = repository;
    }

    public async Task<Claim> AddClaimAsync(Claim claim)
    {
        if (claim.DamageCost > 100000)
        {
            throw new ArgumentException("DamageCost cannot exceed 100,000");
        }

        var cover = await _coverService.GetCoverAsync(claim.CoverId);
        if (cover is null) throw new ArgumentException("Invalid coverId");

        DateOnly claimCreated = DateOnly.FromDateTime(claim.Created);

        if ((claimCreated < cover.StartDate) || (claimCreated > cover.EndDate))
        {
            throw new ArgumentException("Created date must be within the period of the related Cover");
        }
        return await _repository.AddAsync(claim);
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Claim?> DeleteClaimAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }
}