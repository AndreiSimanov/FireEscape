namespace FireEscape.AppSettings;

public class StairsSettings
{
    public required string DefaultUnit { get; set; }
    public int DefaultUnitMultiplier { get; set; }
    public string[]? StairsMountTypes { get; set; }
    public StairsType[]? StairsTypes { get; set; }
    public ServiceabilityType[]? ServiceabilityTypes { get; set; }
    public ServiceabilityLimits[]? ServiceabilityLimits { get; set; }
    public StairsElementSettings[]? StairsElementSettings { get; set; }
    public bool WeldSeamServiceability { get; set; }
    public bool ProtectiveServiceability { get; set; }
}
