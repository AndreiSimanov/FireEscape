namespace FireEscape.Repositories.Interfaces
{
    public interface IProtocolRepository
    {
        Task AddProtocolPhotoAsync(Protocol protocol, FileResult photo);
        Task<Protocol> CreateProtocolAsync();
        Task DeleteProtocol(Protocol protocol);
        Task<List<Protocol>> GetProtocolsAsync();
        Task SaveProtocolAsync(Protocol protocol);
    }
}
