using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using SQLite;

namespace FireEscape.Repositories;

public class BaseObjectRepository<T, P>(SqliteContext context, IBaseObjectFactory<T, P> factory) : IBaseObjectRepository<T, P> 
    where T : BaseObject, new() 
    where P : BaseObject
{
    protected readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;

    public virtual async Task<T> CreateAsync(P? parent)
    {
        var obj = factory.CreateDefault(parent);
        obj = await SaveAsync(obj);
        return obj;
    }

    public virtual async Task<T> SaveAsync(T obj)
    {
        if (obj.Id != 0)
        {
            obj.Updated = DateTime.Now;
            await (await connection).UpdateAsync(obj);
        }
        else
            await (await connection).InsertAsync(obj);
        return obj;
    }

    public virtual async Task DeleteAsync(T obj)
    {
        if (obj.Id != 0)
            await (await connection).DeleteAsync(obj);
    }

    public virtual async Task<T> GetAsync(int id) => 
        await (await connection).Table<T>().Where(obj => obj.Id == id).FirstOrDefaultAsync();
}