using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories;

public class PdfWriterRepository(IOptions<ApplicationSettings> applicationSettings, IOptions<StairsSettings> stairsSettings) : IReportRepository
{
    readonly ApplicationSettings applicationSettings = applicationSettings.Value;
    readonly StairsSettings stairsSettings = stairsSettings.Value;

    public async Task<string> CreateReportAsync(Order order, Protocol protocol, UserAccount userAccount, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentNullException(nameof(fileName));
        fileName = fileName + ".pdf";

        var protocolRdp = new ProtocolReportDataProvider(order, protocol, protocol.Stairs!, stairsSettings, userAccount);

        var filePath = Path.Combine(applicationSettings.ContentFolder, fileName);

        return await ProtocolPdfReportMaker.MakeReportAsync(protocolRdp, filePath);
    }
}
