using Microsoft.Extensions.Options;

namespace FireEscape.DBContext;

public class SqliteContext : IAsyncDisposable
{
    readonly AsyncLazy<SQLiteAsyncConnection> connection;

    public SqliteContext(IOptions<ApplicationSettings> applicationSettings)
    {
        var dbPath = Path.Combine(AppUtils.DefaultContentFolder, applicationSettings.Value.DbName);
        connection = new AsyncLazy<SQLiteAsyncConnection>(async () =>
        {
            var conn = new SQLiteAsyncConnection(dbPath);
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
