using System.Text;

namespace FireEscape.Reports.ReportDataProviders;

public record StairsElementResult(BaseStairsElement[] StairsElements, bool IsAbsent, List<string> Summary)
{
    public string Name
    {
        get
        {
            var elementNumber = string.Empty;
            if (StairsElementType == StairsElementTypeEnum.PlatformP2 || StairsElementType == StairsElementTypeEnum.StairwayP2)
                elementNumber = ToRangeString(StairsElements.Select(element => element.ElementNumber));
            return $"{StairsElements.First().Name} {elementNumber}";
        }
    }

    public StairsElementTypeEnum StairsElementType => StairsElements.First().StairsElementType;
    public int TestPointCount => StairsElements.First().TestPointCount;
    public float WithstandLoadCalcResult => StairsElements.First().WithstandLoadCalcResult;
    public int PrintOrder => StairsElements.First().PrintOrder;

    static string ToRangeString(IEnumerable<int> nums)
    {
        var sb = new StringBuilder();
        int distance = 0;
        int currentNum = int.MinValue;
        foreach (var num in nums.OrderBy(num => num))
        {
            if (currentNum == int.MinValue)
            {
                sb.Append(num);
                currentNum = num;
                continue;
            }

            if (currentNum == num)
                continue;

            if (currentNum == num - ++distance)
                continue;

            if (currentNum != currentNum + distance - 1)
            {
                sb.Append(distance == 2 ? ',' : '-');
                sb.Append(currentNum + distance - 1);
            }

            sb.Append(',');
            sb.Append(num);
            currentNum = num;
            distance = 0;
        }

        if (distance > 0)
        {
            sb.Append(distance == 1 ? ',' : '-');
            sb.Append(currentNum + distance);
        }
        return sb.ToString();
    }
}

