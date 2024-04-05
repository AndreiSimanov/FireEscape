namespace FireEscape.Repositories.Interfaces
{
    public interface IProtocolRepository
    {
        Task AddPhotoAsync(Protocol protocol, FileResult? photo);
        Task<Protocol> CreateProtocolAsync();
        Task<Protocol> CopyProtocolAsync(Protocol protocol);
        Task DeleteProtocol(Protocol protocol);
        Task SaveProtocolAsync(Protocol protocol);
        IAsyncEnumerable<Protocol> GetProtocolsAsync();
        Protocol[] GetProtocols();
    }
}
