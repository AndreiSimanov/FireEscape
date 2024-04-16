using FireEscape.DBContext;
using FireEscape.Factories;
using SQLite;

namespace FireEscape.Repositories
{
    public class ProtocolRepository(SqliteContext context, ProtocolFactory protocolFactory) : IProtocolRepository
    {
        readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;

        public async Task<Protocol> CreateProtocolAsync(Order order)
        {
            var protocol = protocolFactory.CreateDefaultProtocol(order);
            await SaveProtocolAsync(protocol);
            return protocol;
        }

        public async Task<Protocol> CopyProtocolAsync(Protocol protocol)
        {
            var newProtocol = protocolFactory.CopyProtocol(protocol);
            await SaveProtocolAsync(newProtocol);
            return newProtocol;
        }

        public async Task<Protocol> SaveProtocolAsync(Protocol protocol)
        {
            if (protocol.Id != 0)
            {
                protocol.Updated = DateTime.Now;
                await (await connection).UpdateAsync(protocol);
            }
            else
                await (await connection).InsertAsync(protocol);
            return protocol;
        }

        public async Task DeleteProtocol(Protocol protocol)
        {
            if (protocol.Id != 0)
                await (await connection).DeleteAsync(protocol);
        }

        public async Task<Protocol[]> GetProtocolsAsync(int orderId)
        {
            var query = (await connection)
                .Table<Protocol>()
                .Where(protocol => protocol.OrderId == orderId)
                .OrderByDescending(item => item.Created);
            return await query.ToArrayAsync();
        }

        public async Task AddPhotoAsync(Protocol protocol, FileResult? photo)
        {
            if (photo != null)
            {
                using var photoStream = await photo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await photoStream.CopyToAsync(memoryStream);
                protocol.Image = memoryStream.ToArray();
                await SaveProtocolAsync(protocol);
            }
        }
    }
}
