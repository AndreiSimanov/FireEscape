namespace FireEscape.Models.StairsElements.BaseStairsElements;

public abstract partial class BaseSupportBeamsElement : BaseStairsElement
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Self))]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    int supportBeamsCount;
}
