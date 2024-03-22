namespace FireEscape.Services
{
    public class ProtocolService
    {
        List<Protocol> protocolList = new();
        readonly IProtocolRepository protocolRepository;
        readonly IReportRepository reportRepository;

        public ProtocolService(IProtocolRepository protocolRepository, IReportRepository reportRepository) 
        {
            this.protocolRepository = protocolRepository;
            this.reportRepository = reportRepository;
        }

        public async Task<Protocol> CreateProtocolAsync() 
        {
            var protocol = await protocolRepository.CreateProtocolAsync();
            protocolList.Insert(0, protocol);
            return protocol;
        } 
 
        public async Task SaveProtocolAsync(Protocol protocol)
        {
            await protocolRepository.SaveProtocolAsync(protocol);
            if (!protocolList.Contains(protocol))
                protocolList.Insert(0, protocol);
        }

        public async Task DeleteProtocol(Protocol protocol)
        {
            await protocolRepository.DeleteProtocol(protocol);
            protocolList.Remove(protocol);
        }

        public async Task<List<Protocol>> GetProtocolsAsync()
        {
            if (!protocolList.Any())
            {
                protocolList = await protocolRepository.GetProtocolsAsync();
                protocolList = protocolList.OrderByDescending(item => item.Created).ToList();
            }    
            return protocolList;
        }

        public async Task AddProtocolPhotoAsync(Protocol protocol)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                await protocolRepository.AddProtocolPhotoAsync(protocol, photo);
            }
        }

        public async Task CreateReportAsync(Protocol protocol, UserAccount? userAccount)
        {
            var fileName = "protocol"; //todo: change file name to some protocol attribute 
            var filePath = Path.Combine(AppSettingsExtension.ContentFolder, fileName);

            filePath = await reportRepository.CreateReportAsync(protocol, userAccount, filePath);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
