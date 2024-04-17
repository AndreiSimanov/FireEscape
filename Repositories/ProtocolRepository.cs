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
            if (protocol.HasImage)
                File.Delete(protocol.Image!);
        }

        public async Task<Protocol[]> GetProtocolsAsync(int orderId)
        {
            var query = (await connection)
                .Table<Protocol>()
                .Where(protocol => protocol.OrderId == orderId)
                .OrderByDescending(item => item.Created);
            return await query.ToArrayAsync();
        }

        public async Task AddImageAsync(Protocol protocol, FileResult? imageFile)
        {
            if (imageFile == null)
                return;

            var imageFileName = $"{protocol.OrderId}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.{ImageUtils.IMAGE_FILE_EXTENSION}";
            var imageFilePath = Path.Combine(applicationSettings.ContentFolder, imageFileName);

            var orientation = ImageUtils.GetImageOrientation(imageFile.FullPath);
            await ImageUtils.TransformImage(imageFile, applicationSettings.MaxImageSize, imageFilePath);
            ImageUtils.SetImageOrientation(imageFilePath, orientation);

            if (protocol.HasImage)
                File.Delete(protocol.Image!);
            protocol.Image = imageFilePath;
            await SaveProtocolAsync(protocol);
        }
    }
}
