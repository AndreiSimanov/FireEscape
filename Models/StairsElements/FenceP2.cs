using FireEscape.Models.Attributes;
using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public partial class FenceP2 : BaseStairsElement
{
    
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty fenceHeight = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Self))]
    [property: JsonIgnore]
    int fenceElementsCount;

    public override string Name => AppResources.StairsFence;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool IsRequired => true;
    public override int PrintOrder => 20;
    public override int TestPointCount => FenceElementsCount;
    public FenceP2() : base() => FenceHeight.PropertyChanged += ServiceabilityPropertyChanged;
}
