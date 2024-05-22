using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories;

public class ProtocolFactory(IOptions<NewProtocolSettings> newProtocolSettings) : IProtocolFactory
{
    readonly NewProtocolSettings newProtocolSettings = newProtocolSettings.Value;

    public Protocol CreateBrokenDataProtocol(int id) => new() { Id = id, FireEscapeObject = AppResources.BrokenData };

    public Protocol CreateDefault(Order? order) => new()
    {
        OrderId = (order == null) ? 0 : order.Id,
        ProtocolNum = newProtocolSettings.ProtocolNum,
        ProtocolDate = DateTime.Today,
        FireEscapeNum = newProtocolSettings.FireEscapeNum,
        Location = (order != null && string.IsNullOrWhiteSpace(order.Location)) ? newProtocolSettings.Location : string.Empty,
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
        newProtocol.FireEscapeNum = newProtocol.FireEscapeNum + 1;
        newProtocol.Created = DateTime.Now;
        newProtocol.Updated = DateTime.Now;
        return newProtocol;
    }
}