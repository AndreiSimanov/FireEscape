namespace FireEscape.Services.Interfaces;

public interface IProtocolService
{
    Task AddPhotoAsync(Protocol protocol);
    Task<Protocol> CopyAsync(Protocol protocol);
    Task<Protocol> CopyWithStairsAsync(Protocol protocol);
    Task<Protocol> CreateAsync(Order order);
    Task DeleteAsync(Protocol protocol);
    Task<Protocol[]> GetProtocolsAsync(int orderId);
    Task SaveAsync(Protocol protocol);
    Task SelectPhotoAsync(Protocol protocol);
}