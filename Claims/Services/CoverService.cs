using Claims;

public class CoverService : ICoverService
{
    private readonly ICoverRepository _repository;

    public CoverService(ICoverRepository repository)
    {
        _repository = repository;
    }

    public async Task<Cover> AddCoverAsync(Cover cover)
    {
        DateOnly currDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var startDate = cover.StartDate.DayNumber;
        var endDate = cover.EndDate.DayNumber;
        
        // Expects that when the startDate and endDate is the same, insuranceLength will be 1 day
        var insuranceLength = endDate - startDate + 1;
        
        if(endDate < startDate) {
            throw new ArgumentException("The endDate cannot be before the startDate");
        }

        // TODO: Cover leap years
        if (insuranceLength > 365)
        {
            throw new ArgumentException("Cover duration cannot exceed one year");
        }

        if (startDate < currDate.DayNumber) {
            throw new ArgumentException("The startDate cannot be in the past");
        }

        cover.Premium = PremiumCalculator.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        return await _repository.AddAsync(cover);
    }

    public async Task<bool> DeleteCoverAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetCoversAsync()
    {
        return await _repository.GetAllAsync();
    }
}