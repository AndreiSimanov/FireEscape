using Microsoft.Extensions.Options;

namespace FireEscape.DBContext;

public class SqliteContext : IAsyncDisposable
{
    readonly AsyncLazy<SQLiteAsyncConnection> connection;

    public SqliteContext(IOptions<ApplicationSettings> applicationSettings)
    {
        connection = new AsyncLazy<SQLiteAsyncConnection>(async () =>
        {
            var conn = new SQLiteAsyncConnection(Path.Combine(await ApplicationSettings.GetDefaultContentFolderAsync(), applicationSettings.Value.DbName));
            //await conn.DropTableAsync<Order>();
            //await conn.DropTableAsync<Protocol>();
            //await conn.DropTableAsync<Stairs>();
            await conn.CreateTablesAsync<Order, Protocol, Stairs>();
            return conn;
        });
    }
    public AsyncLazy<SQLiteAsyncConnection> Connection => connection;

    public async ValueTask DisposeAsync()
    {
        await (await connection).CloseAsync();
    }
}
