namespace FireEscape.Reports.ReportDataProviders
{
    public record StairsElementResult(string Name, string TypeName, int PrintOrder, int TestPointCount, float CalcWithstandLoad, List<string> Summary);
}
