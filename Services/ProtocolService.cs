namespace FireEscape.Services;

public class ProtocolService(IProtocolRepository protocolRepository, IStairsRepository stairsRepository, ISearchDataRepository searchDataRepository) : IProtocolService
{
    public async Task<Protocol> CreateAsync(Order order)
    {
        var protocol = await protocolRepository.CreateAsync(order);
        protocol.Stairs = await stairsRepository.CreateAsync(protocol);
        return await protocolRepository.SaveAsync(protocol);
    }

    public async Task<Protocol> CopyAsync(Protocol protocol)
    {
        var newProtocol = await protocolRepository.CopyAsync(protocol);
        newProtocol.Stairs = await stairsRepository.CreateAsync(newProtocol);
        newProtocol.FireEscapeNum = await protocolRepository.GetNextFireEscapeNum(protocol.OrderId);
        return await protocolRepository.SaveAsync(newProtocol);
    }

    public async Task<Protocol> CopyWithStairsAsync(Protocol protocol)
    {
        var newProtocol = await protocolRepository.CopyAsync(protocol);
        newProtocol.Stairs = await stairsRepository.CopyAsync(protocol.Stairs);
        newProtocol.FireEscapeNum = await protocolRepository.GetNextFireEscapeNum(protocol.OrderId);
        return await protocolRepository.SaveAsync(newProtocol);
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

    public Task<Protocol[]> GetProtocolsAsync(int orderId) => protocolRepository.GetProtocolsAsync(orderId);

    public async Task AddPhotoAsync(Protocol protocol)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var imageFile = await MediaPicker.Default.CapturePhotoAsync();
            await protocolRepository.AddImageAsync(protocol, imageFile);
        }
    }

    public async Task SelectPhotoAsync(Protocol protocol) =>
        await protocolRepository.AddImageAsync(protocol, await MediaPicker.Default.PickPhotoAsync());
}