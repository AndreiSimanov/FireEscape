namespace FireEscape.AppSettings;

public readonly record struct MeasureLimits(
    int StairsElementMaxSize,
    int DeformationMaxSize,
    int SupportBeamsMaxCount,
    int StepsMaxCount,
    int StairwayMaxLength,
    int StairwayStepsMaxCount)
{
    public decimal StairsElementMaxSizeInUnits => ApplicationSettings.PrimaryUnitOfMeasure.ConvertToUnit(StairsElementMaxSize);
    public decimal DeformationMaxSizeInUnits => ApplicationSettings.PrimaryUnitOfMeasure.ConvertToUnit(DeformationMaxSize);
    public decimal StairwayMaxLengthInUnits => ApplicationSettings.PrimaryUnitOfMeasure.ConvertToUnit(StairwayMaxLength);
    public decimal StairsHeightUnits => ApplicationSettings.SecondaryUnitOfMeasure.MaxValue;
}