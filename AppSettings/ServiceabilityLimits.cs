namespace FireEscape.AppSettings
{
    public record ServiceabilityLimits(
        StairsTypeEnum StairsType, 
        bool IsEvacuation, 
        int MaxStairsHeight, 
        int MinStairsWidth);
}
