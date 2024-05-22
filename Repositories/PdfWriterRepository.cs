using FireEscape.Factories.Interfaces;
using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories;

public class PdfWriterRepository(IStairsFactory stairsFactory, IOptions<StairsSettings> stairsSettings) : IReportRepository
{
    readonly StairsSettings stairsSettings = stairsSettings.Value;

    public async Task<string> CreateReportAsync(Order order, Protocol protocol, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentNullException(nameof(fileName));
        fileName = fileName + ".pdf";
        var protocolRdp = new ProtocolReportDataProvider(order, protocol, protocol.Stairs!, stairsFactory, stairsSettings);
        var filePath = Path.Combine(ApplicationSettings.DocumentsFolder, fileName);
        return await ProtocolPdfReportMaker.MakeReportAsync(protocolRdp, filePath);
    }
}
