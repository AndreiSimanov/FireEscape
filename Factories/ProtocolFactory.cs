using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories;

public class ProtocolFactory(IOptions<ProtocolSettings> ProtocolSettings) : IProtocolFactory
{
    readonly ProtocolSettings ProtocolSettings = ProtocolSettings.Value;

    public Protocol CreateBrokenDataProtocol(int id) => new() { Id = id, FireEscapeObject = AppResources.BrokenData };

    public Protocol CreateDefault(Order? order) => new()
    {
        OrderId = (order == null) ? 0 : order.Id,
        ProtocolNum = ProtocolSettings.ProtocolNum,
        ProtocolDate = DateTime.Today,
        FireEscapeNum = ProtocolSettings.FireEscapeNum,
        Location = (order != null && string.IsNullOrWhiteSpace(order.Location)) ? ProtocolSettings.Location : string.Empty,
        Created = DateTime.Now,
        Updated = DateTime.Now
    };

    public Protocol CopyProtocol(Protocol protocol)
    {
        var newProtocol = (Protocol)protocol.Clone();
        newProtocol.Id = 0;
        newProtocol.Image = null;
        newProtocol.ImageFilePath = null;
        newProtocol.Stairs = new();
        newProtocol.StairsId = 0;
        newProtocol.FireEscapeNum = 0;
        newProtocol.Created = DateTime.Now;
        newProtocol.Updated = DateTime.Now;
        return newProtocol;
    }
}