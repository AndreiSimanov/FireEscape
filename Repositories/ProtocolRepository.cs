using FireEscape.DBContext;
using FireEscape.Factories;
using Microsoft.Extensions.Options;
using SQLite;

namespace FireEscape.Repositories
{
    public class ProtocolRepository(SqliteContext context, IOptions<ApplicationSettings> applicationSettings, ProtocolFactory protocolFactory) : IProtocolRepository
    {
        readonly AsyncLazy<SQLiteAsyncConnection> connection = context.Connection;
        readonly ApplicationSettings applicationSettings = applicationSettings.Value;

        public async Task<Protocol> CreateProtocolAsync(int orderId)
        {
            var protocol = protocolFactory.CreateDefaultProtocol(orderId);
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
            if (protocol.HasImage)
                File.Delete(protocol.Image!);
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
                var photoFilePath = Path.Combine(applicationSettings.ContentFolder, photo.FileName);
                using var photoStream = await photo.OpenReadAsync();
                using var outputFile = File.Create(photoFilePath);
                await photoStream.CopyToAsync(outputFile);
                if (protocol.HasImage)
                    File.Delete(protocol.Image!);
                protocol.Image = photoFilePath;
                await SaveProtocolAsync(protocol);
            }
        }
    }
}
