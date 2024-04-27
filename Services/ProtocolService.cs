namespace FireEscape.Services;

public class ProtocolService(IProtocolRepository protocolRepository, IStairsRepository stairsRepository, ISearchDataRepository searchDataRepository, IReportRepository reportRepository)
{
    public async Task<Protocol> CreateAsync(Order order)
    {
        var protocol = await protocolRepository.CreateAsync(order);
        protocol.Stairs = await stairsRepository.CreateAsync(protocol);
        await protocolRepository.SaveAsync(protocol);
        return protocol;
    }

    public async Task<Protocol> CopyAsync(Protocol protocol)
    {
        var newProtocol = await protocolRepository.CopyAsync(protocol);
        newProtocol.Stairs = await stairsRepository.CreateAsync(newProtocol);
        await protocolRepository.SaveAsync(newProtocol);
        return newProtocol;
    }

    public async Task SaveAsync(Protocol protocol)
    {
        await protocolRepository.SaveAsync(protocol);
        await searchDataRepository.SetSearchDataAsync(protocol.OrderId);
    }

    public async Task DeleteAsync(Protocol protocol)
    {
        await protocolRepository.DeleteAsync(protocol);
        await searchDataRepository.SetSearchDataAsync(protocol.OrderId);
    }

    public async Task<Protocol[]> GetProtocolsAsync(int orderId) => await protocolRepository.GetProtocolsAsync(orderId);

    public async Task AddPhotoAsync(Protocol protocol)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var imageFile = await MediaPicker.Default.CapturePhotoAsync();
            await protocolRepository.AddImageAsync(protocol, imageFile);
        }
    }

    public async Task SelectPhotoAsync(Protocol protocol) =>
        await protocolRepository.AddImageAsync(protocol, await MediaPicker.PickPhotoAsync());

    public async Task CreateReportAsync(Order order, Protocol protocol, UserAccount userAccount)
    {
        var fileName = "protocol"; //todo: change file name to some protocol attribute 
        if (protocol.Stairs == null)
            return;
        var filePath = await reportRepository.CreateReportAsync(order, protocol, userAccount, fileName);
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }
}
