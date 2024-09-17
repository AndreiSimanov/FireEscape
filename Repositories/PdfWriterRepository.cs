using FireEscape.Reports.Interfaces;

namespace FireEscape.Repositories;

public class PdfWriterRepository(IProtocolPdfReportMaker protocolPdfReportMaker) : IReportRepository
{
    public Task CreateReportAsync(Order order, Protocol protocol, string outputPath) => 
        protocolPdfReportMaker.CreateReportAsync(order, protocol, outputPath);
}