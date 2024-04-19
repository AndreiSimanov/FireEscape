namespace FireEscape.Services;

public class ProtocolService(IProtocolRepository protocolRepository, ISearchDataRepository searchDataRepository, IReportRepository reportRepository)
{
    public async Task<Protocol> CreateProtocolAsync(Order order) => await protocolRepository.CreateProtocolAsync(order);

    public async Task<Protocol> CopyProtocolAsync(Protocol protocol) => await protocolRepository.CopyProtocolAsync(protocol);

    public async Task SaveProtocolAsync(Protocol protocol)
    {
        await protocolRepository.SaveProtocolAsync(protocol);
        await searchDataRepository.SetSearchDataAsync(protocol.OrderId);
    }

    public async Task DeleteProtocolAsync(Protocol protocol)
    {
        await protocolRepository.DeleteProtocolAsync(protocol);
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
        var filePath = await reportRepository.CreateReportAsync(order, protocol,  userAccount, fileName);
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }
}
