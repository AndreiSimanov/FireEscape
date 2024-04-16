using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories
{
    public class ProtocolFactory(IOptions<NewProtocolSettings> newProtocolSettings, StairsFactory stairsFactory)
    {
        readonly NewProtocolSettings newProtocolSettings = newProtocolSettings.Value;

        public Protocol CreateBrokenDataProtocol(int id) => 
            new Protocol() { Id = id, Image = Protocol.NO_PHOTO, FireEscapeObject = AppResources.BrokenData };

        public Protocol CreateDefaultProtocol(int orderId) => new Protocol()
        {
            OrderId = orderId,
            Image = Protocol.NO_PHOTO,
            ProtocolNum = newProtocolSettings.ProtocolNum,
            Location = newProtocolSettings.Location,
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
