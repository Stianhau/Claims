using Claims;

public interface ICoverService
    {
        Task<IEnumerable<Cover>> GetCoversAsync();
        Task<Cover?> GetCoverAsync(string id);
        Task<Cover> AddCoverAsync(Cover cover);
        Task<Cover?> DeleteCoverAsync(string id);
    }