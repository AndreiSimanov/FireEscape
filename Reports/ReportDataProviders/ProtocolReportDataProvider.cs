using FireEscape.Factories.Interfaces;

namespace FireEscape.Reports.ReportDataProviders;

public class ProtocolReportDataProvider(Order order, Protocol protocol, Stairs stairs, IStairsFactory stairsFactory, StairsSettings stairsSettings)
{
    const string EMPTY_SIGNATURE = "___________________";

    List<StairsElementResult>? stairsElementResults;

    ServiceabilityLimits serviceabilityLimits = stairsSettings.ServiceabilityLimits!.FirstOrDefault(item =>
           item.StairsType == stairs.StairsType &&
           item.IsEvacuation == stairs.IsEvacuation);
    public int ProtocolNum => protocol.ProtocolNum;
    public string StairsTypeDescription => stairs.IsEvacuation
        ? "испытания пожарной эвакуационной лестницы"
        : stairs.StairsType == StairsTypeEnum.P2
            ? "испытания пожарной маршевой лестницы"
            : "испытания пожарной лестницы";

    public string Location => string.IsNullOrWhiteSpace(protocol.Location) ? order.Location : protocol.Location;
    public string Address => string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;

    public string ProtocolDate => string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate);

    public string StairsType => EnumDescriptionTypeConverter.GetEnumDescription(stairs.StairsType);

    public string StairsMountType => EnumDescriptionTypeConverter.GetEnumDescription(stairs.StairsMountType);

    public string FireEscapeObject => string.IsNullOrWhiteSpace(protocol.FireEscapeObject) ? order.FireEscapeObject : protocol.FireEscapeObject;
    public string FullAddress => Location + ", " + Address;
    public int FireEscapeNum => protocol.FireEscapeNum;

    public float StairsHeight => stairs.StairsHeight.Value/1000;
    public float StairsWidth => stairs.StairsWidth.Value;

    public int StepsCount => stairs.StepsCount;

    public string TestEquipment => stairs.StairsType == StairsTypeEnum.P2
        ? "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
        : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.";

    public string WeldSeamServiceability => stairs.WeldSeamServiceability ? "соответствует" : "не соответствует";
    public string ProtectiveServiceability => stairs.ProtectiveServiceability ? "соответствует" : "не соответствует";

    public bool HasImage => protocol.HasImage;
    public string ImageFilePath => HasImage ? protocol.ImageFilePath! : string.Empty;

    public string Customer => order.Customer;

    public string ExecutiveCompany => order.ExecutiveCompany;

    public string PrimaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.PrimaryExecutorSign) ? order.PrimaryExecutorSign : protocol.PrimaryExecutorSign);

    public string SecondaryExecutorSign => GetExecutorSign(string.IsNullOrWhiteSpace(protocol.SecondaryExecutorSign) ? order.SecondaryExecutorSign : protocol.SecondaryExecutorSign);

    public List<string> GetSummary()
    {
        var summary = new List<string>();
        if (!stairs.WeldSeamServiceability)
            summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
        if (!stairs.ProtectiveServiceability)
            summary.Add("конструкция не окрашена (ГОСТ 9.032)");

        CheckServiceability(summary, stairs.StairsHeight,
            _ => StairsHeight > serviceabilityLimits.MaxStairsHeight && serviceabilityLimits.MaxStairsHeight > 0,
            $"высота лестницы не более {serviceabilityLimits.MaxStairsHeight}м ({StairsHeight}м)",
            "высота лестницы не соответствует ГОСТ");

        CheckServiceability(summary, stairs.StairsWidth,
            _ => StairsWidth < serviceabilityLimits.MinStairsWidth,
            $"ширина лестницы не менее {serviceabilityLimits.MinStairsWidth}мм ({StairsWidth}мм)",
            "ширина лестницы не соответствует ГОСТ");
        return summary;
    }



    public List<StairsElementResult> GetStairsElements() // todo: group elements by StairsElementType & WithstandLoad
    {
        if (stairsElementResults != null)
            return stairsElementResults;
        stairsElementResults = stairs.StairsElements
            .Where(stairsElement => stairsElement.BaseStairsType == stairs.BaseStairsType)
            .Select(element => new StairsElementResult(element.Name, element.ElementNumber, element.StairsElementType, element.PrintOrder, element.TestPointCount, element.CalcWithstandLoad, []))
            .ToList();

        stairsElementResults.AddRange(stairsFactory.GetAvailableStairsElements(stairs)
            .Where(element => !stairsElementResults.Any(item => string.Equals(item.StairsElementType, element.StairsElementType)))
            .Select(element => new StairsElementResult(element.Name, element.ElementNumber, element.StairsElementType, element.PrintOrder, 0, 0, [])));

        return stairsElementResults = stairsElementResults.OrderBy(stairsElement => stairsElement.PrintOrder).ThenBy(stairsElement => stairsElement.Name).ToList();
    }

    static void CheckServiceability<T>(List<string> summary, ServiceabilityProperty<T> serviceabilityProperty,
        Predicate<T> predicate, string rejectExplanation, string defaultExplanation) where T : struct
    {
        if (serviceabilityProperty.ServiceabilityType == ServiceabilityTypeEnum.Approve)
            return;

        if (serviceabilityProperty.ServiceabilityType == ServiceabilityTypeEnum.Reject && !string.IsNullOrWhiteSpace(serviceabilityProperty.RejectExplanation))
        {
            summary.Add(serviceabilityProperty.RejectExplanation.Replace(Environment.NewLine, " "));
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

    static string GetExecutorSign(string executorSign)
    {
            if (string.IsNullOrWhiteSpace(executorSign))
                return EMPTY_SIGNATURE;
            var empty = new string(' ', (EMPTY_SIGNATURE.Length - executorSign.Length));
            return empty + executorSign + empty;
    }
}
