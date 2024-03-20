using Claims;

public interface ICoverRepository
{
    Task<Cover?> GetAsync(string id);
    Task<IEnumerable<Cover>> GetAllAsync();
    Task<Cover> AddAsync(Cover entity);
    Task<bool> DeleteAsync(string id);
}