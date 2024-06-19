namespace FireEscape.Factories.Interfaces;

public interface IProtocolFactory : IBaseObjectFactory<Protocol, Order>
{
    Protocol CopyProtocol(Protocol protocol);
    Protocol CreateBrokenDataProtocol(int id);
}