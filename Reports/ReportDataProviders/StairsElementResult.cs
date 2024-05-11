namespace FireEscape.Reports.ReportDataProviders;

public record StairsElementResult(string Name, int ElementNumber, StairsElementTypeEnum StairsElementType, int PrintOrder, int TestPointCount, float CalcWithstandLoad, List<string> Summary);
