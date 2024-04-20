namespace FireEscape.Repositories.Interfaces;

public interface IProtocolRepository : IBaseObjectRepository<Protocol, Order>
{
    Task AddImageAsync(Protocol protocol, FileResult? imageFile);
    Task<Protocol> CopyProtocolAsync(Protocol protocol);
    Task<Protocol[]> GetProtocolsAsync(int orderId);
}
