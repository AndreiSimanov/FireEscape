using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories
{
    public class ProtocolFactory(IOptions<NewProtocolSettings> newProtocolSettings, StairsFactory stairsFactory)
    {
        readonly NewProtocolSettings newProtocolSettings = newProtocolSettings.Value;

        public Protocol CreateBrokenDataProtocol(int id) => 
            new Protocol() { Id = id, Image = Protocol.NO_PHOTO, FireEscapeObject = AppResources.BrokenData };

        public Protocol CreateDefaultProtocol(Order order) => new Protocol()
        {
            OrderId = order.Id,
            Image = Protocol.NO_PHOTO,
            ProtocolNum = newProtocolSettings.ProtocolNum,
            Location = string.IsNullOrWhiteSpace(order.Location) ? newProtocolSettings.Location : string.Empty,
            ProtocolDate = DateTime.Today,
            FireEscapeNum = newProtocolSettings.FireEscapeNum,
            Stairs = stairsFactory.CreateDefaultStairs(),
            Created = DateTime.Now
        };

        public Protocol CopyProtocol(Protocol protocol)
        {
            var newProtocol = (Protocol)protocol.Clone();
            newProtocol.Id = 0;
            newProtocol.Image = Protocol.NO_PHOTO;
            newProtocol.FireEscapeNum = newProtocol.FireEscapeNum + 1;
            newProtocol.Stairs = stairsFactory.CreateDefaultStairs();
            newProtocol.Created = DateTime.Now;
            return newProtocol;
        }
    }
}
