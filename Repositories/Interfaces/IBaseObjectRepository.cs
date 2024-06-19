namespace FireEscape.Repositories.Interfaces;

public interface IBaseObjectRepository<T, P> where T : BaseObject, new() where P : BaseObject
{
    Task<T> CreateAsync(P? parent);
    Task DeleteAsync(T obj);
    Task<T> GetAsync(int id);
    Task<T> SaveAsync(T obj);
}
