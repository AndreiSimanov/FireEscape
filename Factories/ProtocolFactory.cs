using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

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
        if (AppUtils.TryDeserialize<Protocol>(JsonSerializer.Serialize(protocol), out var copy))
        {
            copy!.Id = 0;
            copy.Image = null;
            copy.ImageFilePath = null;
            copy.Stairs = new();
            copy.StairsId = 0;
            copy.FireEscapeNum = 0;
            copy.Created = DateTime.Now;
            copy.Updated = DateTime.Now;
            return copy;
        }
        throw new Exception(AppResources.CopyProtocolError);
    }
}