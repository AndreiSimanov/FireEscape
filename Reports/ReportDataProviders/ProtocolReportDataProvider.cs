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
            if (serviceabilityLimits == null )
                serviceabilityLimits = allServiceabilityLimits.Where(item =>
                    item.StairsType == protocol.Stairs.StairsType &&
                    (!item.IsEvacuation.HasValue || item.IsEvacuation == protocol.Stairs.IsEvacuation)).ToArray();
            return serviceabilityLimits;
        }
    }

    Stairs Stairs => protocol.Stairs;

    public int ProtocolNum => protocol.ProtocolNum;

    public string StairsTypeDescription => Stairs.IsEvacuation
        ? "испытания пожарной эвакуационной лестницы"
        : Stairs.StairsType == StairsTypeEnum.P2
            ? "испытания пожарной маршевой лестницы"
            : "испытания пожарной лестницы";

    public string Location => string.IsNullOrWhiteSpace(protocol.Location) ? order.Location : protocol.Location;
    public string Address => string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;

    public string ProtocolDate => string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate);

    public string StairsType => EnumDescriptionTypeConverter.GetEnumDescription(Stairs.StairsType);

    public string StairsMountType => EnumDescriptionTypeConverter.GetEnumDescription(Stairs.StairsMountType);

    public string FireEscapeObject => string.IsNullOrWhiteSpace(protocol.FireEscapeObject) ? order.FireEscapeObject : protocol.FireEscapeObject;
    public string FullAddress => Location + ", " + Address;
    public int FireEscapeNum => protocol.FireEscapeNum;

    public float StairsHeight => Stairs.StairsHeight.Value / 1000;
    public float StairsWidth => Stairs.StairsWidth.Value;

    public int StepsCount => Stairs.StepsCount;

    public string TestEquipment => Stairs.StairsType == StairsTypeEnum.P2
        ? "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
        : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.";

    public string WeldSeamServiceability => Stairs.WeldSeamServiceability ? "соответствует" : "не соответствует";
    public string ProtectiveServiceability => Stairs.ProtectiveServiceability ? "соответствует" : "не соответствует";

    public bool HasImage => protocol.HasImage;
    public string ImageFilePath => HasImage ? protocol.ImageFilePath! : string.Empty;

    public string Customer => order.Customer;

    public string ExecutiveCompany => order.ExecutiveCompany;

    public string PrimaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.PrimaryExecutorSign) ? order.PrimaryExecutorSign : protocol.PrimaryExecutorSign);

    public string SecondaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.SecondaryExecutorSign) ? order.SecondaryExecutorSign : protocol.SecondaryExecutorSign);

    public List<StairsElementResult> GetStairsElements()
    {
        if (stairsElementResults != null)
            return stairsElementResults;

        stairsElementResults = Stairs.StairsElements.
            Where(stairsElement => stairsElement.BaseStairsType == Stairs.BaseStairsType).
            GroupBy(stairsElement => new { stairsElement.StairsElementType, stairsElement.WithstandLoadCalc }, (key, group) =>
                new StairsElementResult(group.ToArray(), false, [])).ToList();

        stairsElementResults.AddRange(stairsFactory.GetAvailableStairsElements(Stairs)
            .Where(element => !stairsElementResults.Any(item => item.StairsElementType == element.StairsElementType))
            .Select(element => new StairsElementResult([element], true, [])));

        return stairsElementResults = [.. stairsElementResults.OrderBy(element => element.PrintOrder).ThenBy(element => element.Name)];
    }

    public List<string> GetSummary()
    {
        var summary = new List<string>();
        if (!Stairs.WeldSeamServiceability)
            summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
        if (!Stairs.ProtectiveServiceability)
            summary.Add("конструкция не окрашена (ГОСТ 9.032)");

        GetServiceabilityRecords(Stairs).
            OrderBy(item => item.ServiceabilityLimit.PrintOrder).
            ToList().
            ForEach(serviceabilityRecord => CheckServiceability(summary, serviceabilityRecord));
        return summary;
    }

    IEnumerable<ServiceabilityRecord> GetServiceabilityRecords(object obj)
    {
        foreach (var prop in obj.GetType().GetProperties())
        {
            var attr = prop.GetCustomAttribute(typeof(ServiceabilityAttribute)) as ServiceabilityAttribute;
            if (attr != null)
            {
                var serviceabilityName = $"{obj.GetType().Name}.{
                    (string.IsNullOrWhiteSpace(attr.ServiceabilityName) ? prop.Name : attr.ServiceabilityName)}";
                var serviceabilityProperty = prop.GetValue(obj, null) as ServiceabilityProperty;
                if (serviceabilityProperty != null)
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

    static void CheckServiceability(List<string> summary, ServiceabilityRecord serviceabilityRecord)
    {
        if (serviceabilityRecord.ServiceabilityType == ServiceabilityTypeEnum.Approve)
            return;
    
        if (serviceabilityRecord.MaxValue.HasValue && serviceabilityRecord.Value > serviceabilityRecord.MaxValue)
        {
            summary.Add(string.Format(serviceabilityRecord.LimitRejectExplanation,
                serviceabilityRecord.MaxValue / serviceabilityRecord.Multiplier, serviceabilityRecord.Value / serviceabilityRecord.Multiplier));
        }

        if (serviceabilityRecord.MinValue.HasValue && serviceabilityRecord.Value < serviceabilityRecord.MinValue)
        {
            summary.Add(string.Format(serviceabilityRecord.LimitRejectExplanation,
                serviceabilityRecord.MinValue / serviceabilityRecord.Multiplier, serviceabilityRecord.Value / serviceabilityRecord.Multiplier));
        }

        if (serviceabilityRecord.ServiceabilityType == ServiceabilityTypeEnum.Reject)
        {
            if (string.IsNullOrWhiteSpace(serviceabilityRecord.RejectExplanationText))
                summary.Add(serviceabilityRecord.DefaultRejectExplanation);
            else
                summary.AddRange(serviceabilityRecord.RejectExplanationText.Split(Environment.NewLine));
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
