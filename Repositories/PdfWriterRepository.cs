using FireEscape.Factories.Interfaces;
using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories;

public class PdfWriterRepository(IStairsFactory stairsFactory, IOptions<StairsSettings> stairsSettings) : IReportRepository
{
    readonly StairsSettings stairsSettings = stairsSettings.Value;

    public async Task CreateReportAsync(Order order, Protocol protocol, string outputPath)
    {
        var protocolRdp = new ProtocolReportDataProvider(order, protocol, protocol.Stairs, stairsFactory, stairsSettings);
        await ProtocolPdfReportMaker.MakeReportAsync(protocolRdp, outputPath);
    }
}
