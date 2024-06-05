namespace FireEscape.Reports.ReportDataProviders;

public record ServiceabilityRecord(ServiceabilityProperty ServiceabilityProperty, ServiceabilityLimit ServiceabilityLimit)
{
    public ServiceabilityTypeEnum ServiceabilityType => ServiceabilityProperty.ServiceabilityType;
    public float Value => ServiceabilityProperty.Value;
    public string RejectExplanationText => ServiceabilityProperty.RejectExplanationText;
    public float? MaxValue => ServiceabilityLimit.MaxValue;
    public float? MinValue => ServiceabilityLimit.MinValue;
    public int PrintOrder => ServiceabilityLimit.PrintOrder;
    public float Multiplier => ServiceabilityLimit.Multiplier;
    public string LimitRejectExplanation => ServiceabilityLimit.RejectExplanation;
    public string DefaultRejectExplanation => ServiceabilityLimit.DefaultRejectExplanation;
    public float WithstandLoadCalcResult { get; set; }
    public string? ElementCaption { get; set; }
}