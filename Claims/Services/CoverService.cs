using Claims;

public class CoverService : ICoverService
{
    private readonly ICosmosRepository<Cover> _repository;

    public CoverService(ICosmosRepository<Cover> repository)
    {
        _repository = repository;
    }

    public async Task AddCoverAsync(Cover cover)
    {


        DateTime currDateTime = DateTime.Now;
        TimeOnly currTime = TimeOnly.FromDateTime(currDateTime);
        var startDate = cover.StartDate.ToDateTime(currTime);
        var endDate = cover.EndDate.ToDateTime(currTime);
        if ((endDate - startDate).TotalDays > 365)
        {
            throw new ArgumentException("Cover duration cannot exceed one year");
        }

        if (startDate < currDateTime ){
            throw new ArgumentException("The startDate cannot be in the past");
        }

        cover.Premium = PremiumCalculator.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _repository.AddAsync(cover);
    }

    public async Task DeleteCoverAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<Cover> GetCoverAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetCoversAsync()
    {
        return await _repository.GetAllAsync();
    }
}