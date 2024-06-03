using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class FenceP2 : BaseStairsElement
{
    
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty<float> fenceHeight = new();

    public override string Name => AppResources.StairsFence;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool Required => true;
    public override int PrintOrder => 20;
    public override int TestPointCount => 1;
}
