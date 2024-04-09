using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories
{
    public class PdfWriterRepository : IReportRepository
    {
        readonly ApplicationSettings applicationSettings;
        readonly StairsSettings stairsSettings;
        public PdfWriterRepository(IOptions<ApplicationSettings> applicationSettings, IOptions<StairsSettings> stairsSettings) 
        {
            this.applicationSettings = applicationSettings.Value;
            this.stairsSettings = stairsSettings.Value;
        }

        public async Task<string> CreateReportAsync(Protocol protocol, UserAccount userAccount, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            fileName = fileName + ".pdf";

            var serviceabilityLimits = stairsSettings.ServiceabilityLimits!.FirstOrDefault(item =>
                item.StairsType == protocol.Stairs.StairsType.Type &&
                item.IsEvacuation == protocol.Stairs.IsEvacuation);

            var protocolRdp = new ProtocolReportDataProvider(protocol, serviceabilityLimits, userAccount);

            var filePath = Path.Combine(applicationSettings.ContentFolder, fileName);

            return await ProtocolPdfReportMaker.MakeReport(protocolRdp, filePath);
        }
    }
}
