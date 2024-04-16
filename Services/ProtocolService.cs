namespace FireEscape.Services
{
    public class ProtocolService(IProtocolRepository protocolRepository, IReportRepository reportRepository)
    {
        public async Task<Protocol> CreateProtocolAsync(int orderId) => await protocolRepository.CreateProtocolAsync(orderId);

        public async Task<Protocol> CopyProtocolAsync(Protocol protocol) => await protocolRepository.CopyProtocolAsync(protocol);

        public async Task SaveProtocolAsync(Protocol protocol) => await protocolRepository.SaveProtocolAsync(protocol);

        public async Task DeleteProtocol(Protocol protocol) => await protocolRepository.DeleteProtocol(protocol);

        public async Task<Protocol[]> GetProtocolsAsync(int orderId) => await protocolRepository.GetProtocolsAsync(orderId);

        public async Task AddPhotoAsync(Protocol protocol)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                await protocolRepository.AddPhotoAsync(protocol, photo);
            }
        }

        public async Task SelectPhotoAsync(Protocol protocol) => 
            await protocolRepository.AddPhotoAsync(protocol, await MediaPicker.PickPhotoAsync());

        public async Task CreateReportAsync(Protocol protocol, UserAccount userAccount)
        {
            var fileName = "protocol"; //todo: change file name to some protocol attribute 
            var filePath = await reportRepository.CreateReportAsync(protocol,  userAccount, fileName);
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
