using FireEscape.Factories.Interfaces;

namespace FireEscape.Reports.ReportDataProviders;

public class ProtocolReportDataProvider(Order order, Protocol protocol, ReportSettings reportSettings, IStairsFactory stairsFactory, ServiceabilityLimits[] serviceabilityLimits)
{
    List<StairsElementResult>? stairsElementResults;

    ServiceabilityLimits serviceabilityLimits = serviceabilityLimits.FirstOrDefault(item =>
           item.StairsType == protocol.Stairs.StairsType &&
           item.IsEvacuation == protocol.Stairs.IsEvacuation);

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

    public List<string> GetSummary()
    {
        var summary = new List<string>();
        if (!Stairs.WeldSeamServiceability)
            summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
        if (!Stairs.ProtectiveServiceability)
            summary.Add("конструкция не окрашена (ГОСТ 9.032)");

        CheckServiceability(summary, Stairs.StairsHeight,
            _ => StairsHeight > serviceabilityLimits.MaxStairsHeight && serviceabilityLimits.MaxStairsHeight > 0,
            $"высота лестницы не более {serviceabilityLimits.MaxStairsHeight}м ({StairsHeight}м)",
            "высота лестницы не соответствует ГОСТ");

        CheckServiceability(summary, Stairs.StairsWidth,
            _ => StairsWidth < serviceabilityLimits.MinStairsWidth,
            $"ширина лестницы не менее {serviceabilityLimits.MinStairsWidth}мм ({StairsWidth}мм)",
            "ширина лестницы не соответствует ГОСТ");
        return summary;
    }

    public List<StairsElementResult> GetStairsElements() // todo: group elements by StairsElementType & WithstandLoad
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

    static void CheckServiceability<T>(List<string> summary, ServiceabilityProperty<T> serviceabilityProperty,
        Predicate<T> predicate, string rejectExplanation, string defaultExplanation) where T : struct
    {
        if (serviceabilityProperty.ServiceabilityType == ServiceabilityTypeEnum.Approve)
            return;

        if (serviceabilityProperty.ServiceabilityType == ServiceabilityTypeEnum.Reject && !string.IsNullOrWhiteSpace(serviceabilityProperty.RejectExplanation))
        {
            summary.AddRange(serviceabilityProperty.RejectExplanation.Split(Environment.NewLine));
            return;
        }

        if (predicate != null && predicate(serviceabilityProperty.Value))
        {
            summary.Add(string.Format(rejectExplanation, serviceabilityProperty.Value));
            return;
        }

        if (serviceabilityProperty.ServiceabilityType == ServiceabilityTypeEnum.Reject)
            summary.Add(defaultExplanation);
    }

    string GetExecutorSign(string executorSign)
    {
        if (string.IsNullOrWhiteSpace(executorSign))
            return reportSettings.EmptySignature;
        var empty = new string(' ', reportSettings.EmptySignature.Length - executorSign.Length);
        return empty + executorSign + empty;
    }
}
