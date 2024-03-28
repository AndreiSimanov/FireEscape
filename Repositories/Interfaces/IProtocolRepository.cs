namespace FireEscape.Repositories.Interfaces
{
    public interface IProtocolRepository
    {
        Task AddProtocolPhotoAsync(Protocol protocol, FileResult? photo);
        Task<Protocol> CreateProtocolAsync();
        Task DeleteProtocol(Protocol protocol);
        Task SaveProtocolAsync(Protocol protocol);
        IAsyncEnumerable<Protocol> GetProtocolsAsync();
        Protocol[] GetProtocols();
    }
}
