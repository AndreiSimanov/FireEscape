using FireEscape.Converters;
using FireEscape.Factories.Interfaces;
using FireEscape.Models.Attributes;
using System.Reflection;

namespace FireEscape.Reports.ReportDataProviders;

public class ProtocolReportDataProvider(Order order, Protocol protocol, ReportSettings reportSettings, IStairsFactory stairsFactory, ServiceabilityLimit[] allServiceabilityLimits)
{
    List<StairsElementResult>? stairsElementResults;

    ServiceabilityLimit[]? serviceabilityLimits;

    ServiceabilityLimit[] ServiceabilityLimits
    {
        get
        {
            serviceabilityLimits ??= allServiceabilityLimits.Where(item =>
                    item.BaseStairsType == Stairs.BaseStairsType &&
                    (!item.StairsType.HasValue || item.StairsType == Stairs.StairsType) &&
                    (!item.IsEvacuation.HasValue || item.IsEvacuation == Stairs.IsEvacuation)).ToArray();
            return serviceabilityLimits;
        }
    }

    Stairs Stairs => protocol.Stairs;
    public int ProtocolNum => protocol.ProtocolNum;
    public bool IsEvacuation => Stairs.IsEvacuation;
    public string Location => string.IsNullOrWhiteSpace(protocol.Location) ? order.Location : protocol.Location;
    public string Address => string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;
    public DateTime ProtocolDate => protocol.ProtocolDate;
    public StairsTypeEnum StairsType => Stairs.StairsType;
    public string StairsTypeStr => EnumDescriptionTypeConverter.GetEnumDescription(Stairs.StairsType);
    public string StairsMountType => EnumDescriptionTypeConverter.GetEnumDescription(Stairs.StairsMountType);
    public string FireEscapeObject => string.IsNullOrWhiteSpace(protocol.FireEscapeObject) ? order.FireEscapeObject : protocol.FireEscapeObject;
    public string FullAddress => Location + ", " + Address;
    public int FireEscapeNum => protocol.FireEscapeNum;
    public float StairsHeight => Stairs.StairsHeight.Value / 1000;
    public float StairsWidth => Stairs.StairsWidth.Value;
    public int StepsCount => Stairs.StepsCount;
    public bool WeldSeamServiceability => Stairs.WeldSeamServiceability;
    public bool ProtectiveServiceability => Stairs.ProtectiveServiceability;
    public bool HasImage => protocol.HasImage;
    public string ImageFilePath => HasImage ? protocol.ImageFilePath! : string.Empty;
    public string Customer => order.Customer;
    public string ExecutiveCompany => order.ExecutiveCompany;
    public string PrimaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.PrimaryExecutorSign) ? order.PrimaryExecutorSign : protocol.PrimaryExecutorSign);
    public string SecondaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.SecondaryExecutorSign) ? order.SecondaryExecutorSign : protocol.SecondaryExecutorSign);
    public string CustomerSign => GetExecutorSign(string.Empty);
    public string SupportBeamsP1Calc => GetStairsElementResultCalc<SupportBeamsP1>().FirstOrDefault().Item2;
    public string PlatformP1Calc => GetStairsElementResultCalc<PlatformP1>().FirstOrDefault().Item2;
    public IEnumerable<(string, string)> StairwayP2Lens => GetCalcParams<StairwayP2>();
    public IEnumerable<(string, string)> PlatformP2Sizes => GetCalcParams<PlatformP2>();
    public IEnumerable<(string, string)> StairwayP2Calc => GetStairsElementResultCalc<StairwayP2>();
    public IEnumerable<(string, string)> PlatformP2Calc => GetStairsElementResultCalc<PlatformP2>();

    public bool HasStairsFence
    {
        get
        {
            if (Stairs.BaseStairsType == BaseStairsTypeEnum.P1)
                return Stairs.GetBaseStairsTypeElements().FirstOrDefault(element => element.StairsElementType == typeof(FenceP1)) != null;

            var element = Stairs.GetBaseStairsTypeElements().FirstOrDefault(element => element.StairsElementType == typeof(FenceP2)) as FenceP2;
            if (element != null)
                return element.FenceHeight.Value > 0;

            return false;
        }
    }

    public List<StairsElementResult> StairsElementsResult
    {
        get
        {
            if (stairsElementResults != null)
                return stairsElementResults;

            stairsElementResults = Stairs.GetBaseStairsTypeElements().
                Where(stairsElement => stairsElement.BaseStairsType == Stairs.BaseStairsType).
                GroupBy(stairsElement => new { stairsElement.StairsElementType, WithstandLoadCalc = GetWithstandLoadCalc(stairsElement) }, (key, group) =>
                    new StairsElementResult([.. group.OrderBy(element => element.PrintOrder).ThenBy(element => element.ElementNumber)], false, [])).ToList();

            foreach (var (elementResult, element) in stairsElementResults.SelectMany(elementResult => elementResult.StairsElements.Select(element => (elementResult, element))))
            {
                foreach (var serviceabilityRecord in GetServiceabilityRecords(element).OrderBy(item => item.ServiceabilityLimit.PrintOrder))
                {
                    serviceabilityRecord.WithstandLoadCalcResult = element.WithstandLoadCalcResult;
                    serviceabilityRecord.ElementCaption = element.Caption;
                    elementResult.Summary.AddRange(GetServiceabilitySummary(serviceabilityRecord));
                }
            }

            stairsElementResults.AddRange(stairsFactory.GetAvailableStairsElements(Stairs)
                .Where(element => !stairsElementResults.Any(item => item.StairsElementType == element.StairsElementType))
                .Select(element => new StairsElementResult([element], true, [])));

            return stairsElementResults = [.. stairsElementResults.OrderBy(element => element.PrintOrder).ThenBy(element => element.MinElementNumber)];
        }
    }

    public List<string> ReportSummary
    {
        get
        {
            var summary = new List<string>();

            GetServiceabilityRecords(Stairs).
                OrderBy(item => item.ServiceabilityLimit.PrintOrder).
                ToList().
                ForEach(serviceabilityRecord => summary.AddRange(GetServiceabilitySummary(serviceabilityRecord)));

            StairsElementsResult.ForEach(elementResult => summary.AddRange(elementResult.Summary));
            summary.RemoveAll(string.IsNullOrWhiteSpace);
            return summary;
        }
    }

    public float? MaxStairsP1Height 
    { 
        get 
        {
            var stairsHeightlimit = allServiceabilityLimits.FirstOrDefault(limit => limit.StairsType == StairsTypeEnum.P1_1 &&
                limit.ServiceabilityName == $"{nameof(Stairs)}.{nameof(Stairs.StairsHeight)}");
            return stairsHeightlimit.MaxValue / stairsHeightlimit.Multiplier;
        }
    }

    IEnumerable<(string, string)> GetStairsElementResultCalc<T>()
    {
        return StairsElementsResult.Where(element => element.StairsElementType == typeof(T) && !element.IsAbsent).
            Select(elementsResult => (elementsResult.ElementNumber, GetWithstandLoadCalc(elementsResult?.StairsElements.FirstOrDefault())));
    }

    string GetWithstandLoadCalc(BaseStairsElement? element)
    {
        if (element is SupportBeamsP1)
            return $"({StairsHeight.ToString(reportSettings.FloatFormat)}*{BaseStairsElement.K2})/({BaseStairsElement.K1}*{element.TestPointCount})*{BaseStairsElement.K3} = {element.WithstandLoadCalcResult.ToString(reportSettings.FloatFormat)}";

        if (element is BasePlatformElement platformElement)
           return $"({platformElement.Size}*{BaseStairsElement.K2})/({BaseStairsElement.K4}*{platformElement.SupportBeamsCount})*{BaseStairsElement.K3} = {platformElement.WithstandLoadCalcResult.ToString(reportSettings.FloatFormat)}";

        if (element is StairwayP2 stairwayP2)
            return $"({(stairwayP2.StairwayLength / 1000).ToString(reportSettings.FloatFormat)}*{BaseStairsElement.K2})/({BaseStairsElement.K4}*{stairwayP2.SupportBeamsCount})*{BaseStairsElement.K3}*{BaseStairsElement.COS_ALPHA} = {stairwayP2.WithstandLoadCalcResult.ToString(reportSettings.FloatFormat)}";

        return string.Empty;
    }

    IEnumerable<(string, string)> GetCalcParams<T>() where T : BaseStairsElement
    {
        if (Stairs.BaseStairsType != BaseStairsTypeEnum.P2)
            yield break;

        foreach (var elementsResult in StairsElementsResult.Where(element => element.StairsElementType == typeof(T) && !element.IsAbsent))
        {
            var val = string.Empty;
            if (elementsResult.StairsElements.FirstOrDefault() is StairwayP2 stairwayP2)
                val = (stairwayP2.StairwayLength / 1000).ToString(reportSettings.FloatFormat);
            else if (elementsResult.StairsElements.FirstOrDefault() is PlatformP2 platformP2)
                val = platformP2.Size.ToString();

            yield return (elementsResult.ElementNumber, val);
        }
    }

    IEnumerable<ServiceabilityRecord> GetServiceabilityRecords(object obj)
    {
        foreach (var prop in obj.GetType().GetProperties())
        {
            if (prop.GetCustomAttribute(typeof(ServiceabilityAttribute)) is ServiceabilityAttribute attr)
            {
                var serviceabilityName = $"{obj.GetType().Name}.{(string.IsNullOrWhiteSpace(attr.ServiceabilityName) ? prop.Name : attr.ServiceabilityName)}";
                if (prop.GetValue(obj, null) is ServiceabilityProperty serviceabilityProperty)
                {
                    if (ServiceabilityLimits.Any(serviceabilityLimit => serviceabilityLimit.ServiceabilityName == serviceabilityName))
                    {
                        yield return new ServiceabilityRecord(serviceabilityProperty,
                            ServiceabilityLimits.First(serviceabilityLimit => serviceabilityLimit.ServiceabilityName == serviceabilityName));
                    }
                }
            }
        }
    }

    IEnumerable<string> GetServiceabilitySummary(ServiceabilityRecord serviceabilityRecord)
    {
        if (serviceabilityRecord.ServiceabilityType == ServiceabilityTypeEnum.Approve)
            yield break;

        var floatFormat = serviceabilityRecord.Multiplier > 1? reportSettings.FloatFormat : string.Empty;

        if (serviceabilityRecord.MaxValue.HasValue && serviceabilityRecord.Value > serviceabilityRecord.MaxValue)
        {
            yield return string.Format(serviceabilityRecord.LimitRejectExplanation,
                (serviceabilityRecord.MaxValue.Value / serviceabilityRecord.Multiplier).ToString(floatFormat),
                (serviceabilityRecord.Value / serviceabilityRecord.Multiplier).ToString(floatFormat),
                serviceabilityRecord.WithstandLoadCalcResult,
                serviceabilityRecord.ElementCaption);
        }

        if (serviceabilityRecord.MinValue.HasValue && serviceabilityRecord.Value < serviceabilityRecord.MinValue)
        {
            yield return string.Format(serviceabilityRecord.LimitRejectExplanation,
                (serviceabilityRecord.MinValue.Value / serviceabilityRecord.Multiplier).ToString(floatFormat),
                (serviceabilityRecord.Value / serviceabilityRecord.Multiplier).ToString(floatFormat),
                serviceabilityRecord.WithstandLoadCalcResult,
                serviceabilityRecord.ElementCaption);
        }

        if (serviceabilityRecord.ServiceabilityType == ServiceabilityTypeEnum.Reject)
        {
            if (string.IsNullOrWhiteSpace(serviceabilityRecord.RejectExplanationText))
                yield return string.Format(serviceabilityRecord.DefaultRejectExplanation, serviceabilityRecord.ElementCaption);
            else
            {
                foreach (var text in serviceabilityRecord.RejectExplanationText.Split(Environment.NewLine))
                    yield return text;
            }
        }
    }

    string GetExecutorSign(string executorSign)
    {
        if (string.IsNullOrWhiteSpace(executorSign))
            return reportSettings.EmptySignature;
        var empty = new string(' ', reportSettings.EmptySignature.Length - executorSign.Length);
        return empty + executorSign + empty;
    }
}
