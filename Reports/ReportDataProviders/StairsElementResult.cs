namespace FireEscape.Reports.ReportDataProviders;

public record StairsElementResult(BaseStairsElement[] StairsElements, bool IsAbsent, List<string> Summary)
{
    public string Name
    {
        get
        {
            var elementNumber = string.Empty;
            if (StairsElementType == StairsElementTypeEnum.PlatformP2 || StairsElementType == StairsElementTypeEnum.StairwayP2)
                elementNumber = GetElementNumber(StairsElements.Select(element => element.ElementNumber));
            return $"{StairsElements.First().Name} {elementNumber}";
        }
    }

    public StairsElementTypeEnum StairsElementType => StairsElements.First().StairsElementType;
    public int TestPointCount => StairsElements.First().TestPointCount;
    public float WithstandLoadCalcResult => StairsElements.First().WithstandLoadCalcResult;
    public int PrintOrder => StairsElements.First().PrintOrder;

    static string GetElementNumber(IEnumerable<int> numbers)
    {
        return string.Join(",", numbers.OrderBy(num => num));
    }

}

