using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.Factories
{
    public class ProtocolFactory(IOptions<NewProtocolSettings> newProtocolSettings, StairsFactory stairsFactory)
    {
        readonly NewProtocolSettings newProtocolSettings = newProtocolSettings.Value;

        public Protocol CreateBrokenDataProtocol(string protocolId) => 
            new Protocol() { Id = protocolId, Image = Protocol.NO_PHOTO, FireEscapeObject = AppResources.BrokenData };

        public Protocol CreateDefaultProtocol() => new Protocol()
        {
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
            newProtocol.Id = null;
            newProtocol.Image = Protocol.NO_PHOTO;
            newProtocol.FireEscapeNum = newProtocol.FireEscapeNum + 1;
            newProtocol.Stairs = stairsFactory.CreateDefaultStairs();
            newProtocol.Created = DateTime.Now;
            return newProtocol;
        }
    }
}
