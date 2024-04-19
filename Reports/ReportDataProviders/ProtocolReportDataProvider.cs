﻿namespace FireEscape.Reports.ReportDataProviders;

public class ProtocolReportDataProvider(Order order, Protocol protocol, ServiceabilityLimits? serviceabilityLimits, UserAccount userAccount)
{
    Order order = order;
    Protocol protocol = protocol;
    Stairs stairs = protocol.Stairs;
    ServiceabilityLimits? serviceabilityLimits = serviceabilityLimits;
    UserAccount userAccount = userAccount;

    public int ProtocolNum => protocol.ProtocolNum;
    public string StairsTypeDescription => stairs.IsEvacuation
        ? "испытания пожарной эвакуационной лестницы"
        : stairs.StairsType.Type == StairsTypeEnum.P2
            ? "испытания пожарной маршевой лестницы"
            : "испытания пожарной лестницы";

    public string Location => string.IsNullOrWhiteSpace(protocol.Location) ? order.Location : protocol.Location;
    public string Address => string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;

    public string ProtocolDate => string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate);

    public string StairsType => stairs.StairsType.Type == StairsTypeEnum.P2
        ? "маршевая лестница тип П2"
        : stairs.StairsType.Name;

    public string StairsMountType => stairs.StairsMountType;

    public string FireEscapeObject => protocol.FireEscapeObject;
    public string FullAddress => Location + ", " + Address;
    public int FireEscapeNum => protocol.FireEscapeNum;

    public float StairsHeight => stairs.StairsHeight.Value?? 0f;
    public int StairsWidth => stairs.StairsWidth.Value?? 0;

    public int StepsCount => stairs.StepsCount?? 0;

    public string TestEquipment => stairs.StairsType.Type == StairsTypeEnum.P2
        ? "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
        : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.";

    public string WeldSeamServiceability => stairs.WeldSeamServiceability ? "соответствует" : "не соответствует";
    public string ProtectiveServiceability => stairs.ProtectiveServiceability ? "соответствует" : "не соответствует";

    public bool HasImage => protocol.HasImage;
    public string Image => protocol.HasImage ? protocol.Image! : string.Empty;

    public string Customer => order.Customer;

    public string ExecutiveCompany => string.IsNullOrWhiteSpace(order.ExecutiveCompany) ? string.Empty : order.ExecutiveCompany;
    
    public string UserAccountSignature => string.IsNullOrWhiteSpace(userAccount.Signature) ? "___________" : userAccount.Signature;

    public List<string> GetSummary()
    {
        var summary = new List<string>();
        if (!stairs.WeldSeamServiceability)
            summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
        if (!stairs.ProtectiveServiceability)
            summary.Add("конструкция не окрашена (ГОСТ 9.032)");

        if (serviceabilityLimits == null)
            return summary;

        CheckServiceability(summary, stairs.StairsHeight,
            item => (item?? 0f) > serviceabilityLimits.MaxStairsHeight && serviceabilityLimits.MaxStairsHeight > 0,
            $"высота лестницы не более {serviceabilityLimits.MaxStairsHeight}м" + " ({0}м)",
            "высота лестницы не соответствует ГОСТ");

        CheckServiceability(summary, stairs.StairsWidth,
            item => (item ?? 0) < serviceabilityLimits.MinStairsWidth,
            $"ширина лестницы не менее {serviceabilityLimits.MinStairsWidth}мм" + " ({0}мм)",
            "ширина лестницы не соответствует ГОСТ");
        return summary;
    }

    static void CheckServiceability<T>(List<string> summary, ServiceabilityProperty<T> serviceabilityProperty,
        Predicate<T?> predicate, string rejectExplanation, string defaultExplanation)
    {
        var serviceabilityType = serviceabilityProperty.Serviceability.Type;

        if (serviceabilityType == ServiceabilityTypeEnum.Approve)
            return;

        if (serviceabilityType == ServiceabilityTypeEnum.Reject && !string.IsNullOrWhiteSpace(serviceabilityProperty.RejectExplanation))
        {
            summary.Add(serviceabilityProperty.RejectExplanation.Replace(Environment.NewLine, " "));
            return;
        }

        if (predicate != null && predicate(serviceabilityProperty.Value))
        {
            summary.Add(string.Format(rejectExplanation, serviceabilityProperty.Value == null ? "0" : serviceabilityProperty.Value));
            return;
        }

        if (serviceabilityType == ServiceabilityTypeEnum.Reject)
            summary.Add(defaultExplanation);
    }
}
