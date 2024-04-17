using Microsoft.Extensions.Options;
using SQLite;

namespace FireEscape.DBContext
{
    public class SqliteContext : IAsyncDisposable
    {
        readonly AsyncLazy<SQLiteAsyncConnection> connection;

        public SqliteContext(IOptions<ApplicationSettings> applicationSettings)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, applicationSettings.Value.DbName);
            connection = new AsyncLazy<SQLiteAsyncConnection>(async () =>
            {
                var conn = new SQLiteAsyncConnection(dbPath);
                //await conn.DropTableAsync<Order>();
                //await conn.DropTableAsync<Protocol>();
                await conn.CreateTableAsync<Order>();
                await conn.CreateTableAsync<Protocol>();
                return conn;
            });
        }
        public AsyncLazy<SQLiteAsyncConnection> Connection => connection;

        public async ValueTask DisposeAsync()
        {
           await(await connection).CloseAsync();
        }
    }
}
