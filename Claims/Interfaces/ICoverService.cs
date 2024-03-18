using Claims;

public interface ICoverService
    {
        Task<IEnumerable<Cover>> GetCoversAsync();
        Task<Cover> GetCoverAsync(string id);
        Task AddCoverAsync(Cover cover);
        Task DeleteCoverAsync(string id);
    }