using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories;

public class ProtocolRepository(SqliteContext context, IOptions<ApplicationSettings> applicationSettings, IProtocolFactory factory)
    : BaseObjectRepository<Protocol, Order>(context, factory), IProtocolRepository
{
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;

    public override async Task DeleteAsync(Protocol protocol)
    {
        await base.DeleteAsync(protocol);
        if (protocol.HasImage)
            File.Delete(protocol.Image!);
    }

    public async Task<Protocol> CopyProtocolAsync(Protocol protocol)
    {
        var newProtocol = factory.CopyProtocol(protocol);
        await SaveAsync(newProtocol);
        return newProtocol;
    }

    public async Task<Protocol[]> GetProtocolsAsync(int orderId)
    {
        var query = (await connection)
            .Table<Protocol>()
            .Where(protocol => protocol.OrderId == orderId)
            .OrderByDescending(item => item.Id);
        return await query.ToArrayAsync();
    }

    public async Task AddImageAsync(Protocol protocol, FileResult? imageFile)
    {
        if (imageFile == null)
            return;

        var imageFileName = $"{protocol.OrderId}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.{ImageUtils.IMAGE_FILE_EXTENSION}";
        var imageFilePath = Path.Combine(applicationSettings.ContentFolder, imageFileName);

        var orientation = ImageUtils.GetImageOrientation(imageFile.FullPath);
        await ImageUtils.TransformImageAsync(imageFile, applicationSettings.MaxImageSize, imageFilePath);
        ImageUtils.SetImageOrientation(imageFilePath, orientation);

        if (protocol.HasImage)
            File.Delete(protocol.Image!);
        protocol.Image = imageFilePath;
        await SaveAsync(protocol);
    }
}
