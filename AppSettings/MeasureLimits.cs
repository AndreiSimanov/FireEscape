namespace FireEscape.AppSettings;

public readonly record struct MeasureLimits(
    int StairsElementMaxSize,
    int DeformationMaxSize,
    int SupportBeamsMaxCount,
    int StepsMaxCount,
    int StairwayStepsMaxCount)
{
    public decimal StairsElementMaxSizeInUnits => UnitOfMeasureSettings.PrimaryUnitOfMeasure.ConvertToUnit(StairsElementMaxSize);
    public decimal DeformationMaxSizeInUnits => UnitOfMeasureSettings.PrimaryUnitOfMeasure.ConvertToUnit(DeformationMaxSize);
    public decimal StairsHeightUnits => UnitOfMeasureSettings.SecondaryUnitOfMeasure.MaxValue;
}