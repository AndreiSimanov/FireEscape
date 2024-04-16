using FireEscape.DBContext;
using FireEscape.Factories;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Graphics.Platform;
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
            if (imageFile != null)
            {
                var imageFileName = $"{protocol.OrderId}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.{AppUtils.IMAGE_FILE_EXTENSION}";
                var imageFilePath = Path.Combine(applicationSettings.ContentFolder, imageFileName);

                using var imageStream = await imageFile.OpenReadAsync();
                using var image = PlatformImage.FromStream(imageStream);
                var scale = (image.Height > image.Width ? image.Height : image.Width) / applicationSettings.MaxImageSize;
                using var resizedImage = image.Resize(image.Width / scale, image.Height / scale, ResizeMode.Stretch, false);
                using var outputFile = File.Create(imageFilePath);
                await resizedImage.SaveAsync(outputFile, ImageFormat.Jpeg);

                if (protocol.HasImage)
                    File.Delete(protocol.Image!);
                protocol.Image = imageFilePath;
                await SaveProtocolAsync(protocol);
            }
        }
    }
}
