using FireEscape.Reports.ReportDataProviders;

namespace FireEscape.Reports.Interfaces;

public interface IProtocolReportDataProvider
{
    void Init(Order order, Protocol protocol);
    string Address { get; }
    string Customer { get; }
    string CustomerSign { get; }
    string ExecutiveCompany { get; }
    int FireEscapeNum { get; }
    string FireEscapeObject { get; }
    string FullAddress { get; }
    bool HasImage { get; }
    bool HasStairsFence { get; }
    string ImageFilePath { get; }
    bool IsEvacuation { get; }
    string Location { get; }
    float? MaxStairsP1Height { get; }
    string PlatformP1Calc { get; }
    IEnumerable<(string, string)> PlatformP2Calc { get; }
    IEnumerable<(string, string)> PlatformP2Sizes { get; }
    string PrimaryExecutorSign { get; }
    bool ProtectiveServiceability { get; }
    DateTime ProtocolDate { get; }
    int ProtocolNum { get; }
    List<string> ReportSummary { get; }
    string SecondaryExecutorSign { get; }
    List<StairsElementResult> StairsElementsResult { get; }
    float StairsHeight { get; }
    StairsMountTypeEnum StairsMountType { get; }
    StairsTypeEnum StairsType { get; }
    float StairsWidth { get; }
    IEnumerable<(string, string)> StairwayP2Calc { get; }
    IEnumerable<(string, string)> StairwayP2Lens { get; }
    int StepsCount { get; }
    string SupportBeamsP1Calc { get; }
    bool WeldSeamServiceability { get; }
    string FontName { get; }
    float FontSize { get; }
    string DateFormat { get; }
    string FloatFormat { get; }
    float ImageScale { get; }
}