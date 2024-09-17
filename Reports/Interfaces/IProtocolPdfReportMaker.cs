namespace FireEscape.Reports.Interfaces;

public interface IProtocolPdfReportMaker
{
    Task CreateReportAsync(Order order, Protocol protocol, string outputPath);
}