namespace FireEscape.Services;

public class ProtocolService(IProtocolRepository protocolRepository, IStairsRepository stairsRepository, ISearchDataRepository searchDataRepository, IReportRepository reportRepository)
{
    public async Task<Protocol> CreateProtocolAsync(Order order)
    {
        var protocol = await protocolRepository.CreateAsync(order);
        protocol.Stairs = await stairsRepository.CreateAsync(protocol);
        return protocol;
    }

    public async Task<Protocol> CopyProtocolAsync(Protocol protocol)
    {
        var newProtocol = await protocolRepository.CopyProtocolAsync(protocol);
        newProtocol.Stairs = await stairsRepository.CreateAsync(newProtocol);
        return protocol;
    }

    public async Task SaveProtocolAsync(Protocol protocol)
    {
        await protocolRepository.SaveAsync(protocol);
        await searchDataRepository.SetSearchDataAsync(protocol.OrderId);
    }

    public async Task DeleteProtocolAsync(Protocol protocol)
    {
        await protocolRepository.DeleteAsync(protocol);
        await searchDataRepository.SetSearchDataAsync(protocol.OrderId);
    }

    public async Task<Protocol[]> GetProtocolsAsync(int orderId)
    {
        var protocols  =  await protocolRepository.GetProtocolsAsync(orderId);
        var stairses = await stairsRepository.GetStairsesAsync(orderId);

        foreach(var protocol in protocols)
        {
            if (stairses.TryGetValue(protocol.Id, out Stairs? stairs))
                protocol.Stairs = stairs;
            else
                protocol.Stairs = await stairsRepository.CreateAsync(protocol);
        }
        return protocols;
    }

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
