using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using SQLiteNetExtensionsAsync.Extensions;

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
            await (await connection).UpdateWithChildrenAsync(obj);
        }
        else
            await (await connection).InsertWithChildrenAsync(obj, true);
        return obj;
    }

    public virtual async Task DeleteAsync(T obj)
    {
        if (obj.Id != 0)
            await (await connection).DeleteAsync(obj, true);
    }

    public virtual async Task<T> GetAsync(int id) =>
        await (await connection).GetWithChildrenAsync<T>(id, true);
}