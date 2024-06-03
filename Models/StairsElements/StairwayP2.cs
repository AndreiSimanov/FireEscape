namespace FireEscape.Models.StairsElements;

public partial class StairwayP2 : BaseSupportBeamsElement
{
    public override string Name => AppResources.Stairway;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int PrintOrder => 30;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;

    public override float WithstandLoadCalcResult // Рмарш = ((L*К2)/(К4*Х))*К3*cos a,
    {
        get
        {
            if (StairwayWidth == 0 || SupportBeamsCount == 0)
                return base.WithstandLoadCalcResult;
            return (float)Math.Round(ConvertToMeter(StairwayWidth) * K2 / (K4 * SupportBeamsCount) * K3 * COS_ALPHA);
        }
    }

    public override string WithstandLoadCalc => $"(({ConvertToMeter(StairwayWidth)}*{K2})/({K4}*{SupportBeamsCount}))*{K3}*{COS_ALPHA} = {WithstandLoadCalcResult}";

    [ObservableProperty]
    int stepsCount;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    float stairwayWidth;
}
