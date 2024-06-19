using FireEscape.Factories.Interfaces;
using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using Microsoft.Extensions.Options;

namespace FireEscape.Repositories;

public class PdfWriterRepository(IProtocolPdfReportMaker protocolPdfReportMaker, IStairsFactory stairsFactory, IOptions<StairsSettings> stairsSettings, IOptions<ReportSettings> reportSettings) : IReportRepository
{
    public async Task CreateReportAsync(Order order, Protocol protocol, string outputPath)
    {
        var protocolRdp = new ProtocolReportDataProvider(order, protocol, reportSettings.Value, stairsFactory, stairsSettings.Value.ServiceabilityLimits ?? []);
        await protocolPdfReportMaker.MakeReportAsync(protocolRdp, outputPath);
    }
}