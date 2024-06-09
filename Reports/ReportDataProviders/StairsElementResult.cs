using System.Text;

namespace FireEscape.Reports.ReportDataProviders;

public record StairsElementResult(BaseStairsElement[] StairsElements, bool IsAbsent, List<string> Summary)
{
    string name = string.Empty;
    public string Name
    {
        get
        {
            if (string.IsNullOrWhiteSpace(name))
                name = GetStairsElementResultName();
            return name;
        }
    }

    public Type StairsElementType => StairsElements.First().StairsElementType;
    public int TestPointCount => StairsElements.Sum(element => element.TestPointCount);
    public float WithstandLoadCalcResult => StairsElements.First().WithstandLoadCalcResult;
    public int PrintOrder => StairsElements.First().PrintOrder;
    public int ElementNumber => StairsElements.Min(element => element.ElementNumber);


    string GetStairsElementResultName()
    {
        var elementNumber = string.Empty;
        if (StairsElementType == typeof(PlatformP2) || StairsElementType == typeof(StairwayP2))
            elementNumber = ToRangeString(StairsElements.Select(element => element.ElementNumber));
        return $"{StairsElements.First().Name} {elementNumber}";
    }

    static string ToRangeString(IEnumerable<int> nums)
    {
        var sb = new StringBuilder();
        int distance = 0;
        int? currentNum = null;
        foreach (var num in nums.OrderBy(num => num))
        {
            if (currentNum == null)
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
