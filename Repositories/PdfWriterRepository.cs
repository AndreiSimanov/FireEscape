using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories
{
    public class PdfWriterRepository : IReportRepository
    {
        readonly ApplicationSettings applicationSettings;
        readonly FireEscapeSettings fireEscapeSettings;
        public PdfWriterRepository(IOptions<ApplicationSettings> applicationSettings, IOptions<FireEscapeSettings> fireEscapeSettings) 
        {
            this.applicationSettings = applicationSettings.Value;
            this.fireEscapeSettings = fireEscapeSettings.Value;
        }

        public async Task<string> CreateReportAsync(Protocol protocol, UserAccount userAccount, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            fileName = fileName + ".pdf";

            var serviceabilityLimits = fireEscapeSettings.ServiceabilityLimits!.FirstOrDefault(item =>
                item.StairsType == protocol.FireEscape.FireEscapeType.StairsType &&
                item.IsEvacuation == protocol.FireEscape.IsEvacuation);

            var protocolRdp = new ProtocolReportDataProvider(protocol, serviceabilityLimits, userAccount);

            var filePath = Path.Combine(applicationSettings.ContentFolder, fileName);

            return await ProtocolPdfReportMaker.MakeReport(protocolRdp, filePath);
        }
    }
}
