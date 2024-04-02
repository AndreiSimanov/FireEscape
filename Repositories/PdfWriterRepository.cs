using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;

namespace FireEscape.Repositories
{
    public class PdfWriterRepository : IReportRepository
    {
        public async Task<string> CreateReportAsync(Protocol protocol, UserAccount userAccount, string filePath)
        {
            return await ProtocolPdfReportMaker.MakeReport(new ProtocolReportDataProvider(protocol, userAccount), filePath);
        }
    }
}


