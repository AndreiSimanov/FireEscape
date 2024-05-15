using System.Collections.ObjectModel;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

public abstract partial class BasePlatformElement : BaseSupportBeamsElement
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlatformSize))]
    ObservableCollection<PlatformSize> platformSizes = new();

    [ObservableProperty]
    PlatformSize platformSize =new();
}