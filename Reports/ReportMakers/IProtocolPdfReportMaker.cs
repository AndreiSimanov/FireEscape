using FireEscape.Reports.ReportDataProviders;

namespace FireEscape.Reports.ReportMakers
{
    public interface IProtocolPdfReportMaker
    {
        Task MakeReportAsync(ProtocolReportDataProvider protocolRdp, string outputPath);
    }
}