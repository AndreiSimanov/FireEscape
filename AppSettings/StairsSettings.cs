namespace FireEscape.AppSettings;

public class StairsSettings
{
    public required string DefaultUnit { get; set; }
    public int DefaultUnitMultiplier { get; set; }
    public string[] StairsMountTypes => EnumDescriptionTypeConverter.GetEnumDescriptions<StairsMountTypeEnum>().ToArray();
    public string[] StairsTypes => EnumDescriptionTypeConverter.GetEnumDescriptions<StairsTypeEnum>().ToArray();
    public string[] ServiceabilityTypes => EnumDescriptionTypeConverter.GetEnumDescriptions<ServiceabilityTypeEnum>().ToArray();
    public ServiceabilityLimits[]? ServiceabilityLimits { get; set; }
    public StairsElementSettings[]? StairsElementSettings { get; set; }
    public bool WeldSeamServiceability { get; set; }
    public bool ProtectiveServiceability { get; set; }
}
