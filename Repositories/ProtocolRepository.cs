using FireEscape.DBContext;
using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using SQLiteNetExtensionsAsync.Extensions;

namespace FireEscape.Repositories;

public class ProtocolRepository(SqliteContext context, IOptions<ApplicationSettings> applicationSettings, IProtocolFactory factory)
    : BaseObjectRepository<Protocol, Order>(context, factory), IProtocolRepository
{
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;

    public override async Task DeleteAsync(Protocol protocol)
    {
        await base.DeleteAsync(protocol);
        if (protocol.HasImage)
            File.Delete(protocol.ImageFilePath!);
    }

    public async Task<Protocol> CopyAsync(Protocol protocol)
    {
        var newProtocol = factory.CopyProtocol(protocol);
        await SaveAsync(newProtocol);
        return newProtocol;
    }

    public async Task<Protocol[]> GetProtocolsAsync(int orderId)
    {
        var protocols = await (await connection).GetAllWithChildrenAsync<Protocol>(protocol => protocol.OrderId == orderId, true);
        protocols.Where(protocol => !string.IsNullOrWhiteSpace(protocol.Image)).ToList().
            ForEach(protocol => protocol.ImageFilePath = Path.Combine(ApplicationSettings.ImagesFolder, protocol.Image!));
        return [.. protocols.OrderByDescending(item => item.Id)];
    }

    public async Task AddImageAsync(Protocol protocol, FileResult? imageFile)
    {
        if (imageFile == null)
            return;

        var imageFileName = $"{protocol.OrderId}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.{ImageUtils.IMAGE_FILE_EXTENSION}";
        var imageFilePath = Path.Combine(ApplicationSettings.ImagesFolder, imageFileName);

        var orientation = ImageUtils.GetImageOrientation(imageFile.FullPath);
        await ImageUtils.TransformImageAsync(imageFile, imageFilePath, applicationSettings.MaxImageSize, applicationSettings.ImageQuality / 100f);
        ImageUtils.SetImageOrientation(imageFilePath, orientation);

        if (protocol.HasImage)
            File.Delete(protocol.ImageFilePath!);
        protocol.Image = imageFileName;
        protocol.ImageFilePath = imageFilePath;

        await SaveAsync(protocol);
    }
}
