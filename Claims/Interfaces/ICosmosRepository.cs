public interface ICosmosRepository<T>
{
    Task<T> GetAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task DeleteAsync(string id);
}