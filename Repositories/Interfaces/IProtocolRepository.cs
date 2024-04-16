namespace FireEscape.Repositories.Interfaces
{
    public interface IProtocolRepository
    {
        Task AddPhotoAsync(Protocol protocol, FileResult? photo);
        Task<Protocol> CopyProtocolAsync(Protocol protocol);
        Task<Protocol> CreateProtocolAsync(Order order);
        Task DeleteProtocol(Protocol protocol);
        Task<Protocol[]> GetProtocolsAsync(int orderId);
        Task<Protocol> SaveProtocolAsync(Protocol protocol);
    }
}
