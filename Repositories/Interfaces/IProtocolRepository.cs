namespace FireEscape.Repositories.Interfaces;

public interface IProtocolRepository : IBaseObjectRepository<Protocol, Order>
{
    Task AddImageAsync(Protocol protocol, FileResult? imageFile);
    Task<Protocol> CopyAsync(Protocol protocol);
    Task<Protocol[]> GetProtocolsAsync(int orderId);
    Task<int> GetNextFireEscapeNum(int orderId);
}
