using Newtonsoft.Json;
using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

public abstract partial class BasePlatformElement : BaseSupportBeamsElement
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Self))]
    PlatformSize[] platformSizes = [];

    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty platformLength = new();

    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty platformWidth = new();

    public override float WithstandLoadCalcResult // Рплощ = ((S*К2)/(К4*Х))*К3,
    {
        get
        {
            var s = Size;
            if (SupportBeamsCount == 0 || s == 0)
                return base.WithstandLoadCalcResult;
            return (float)Math.Round(s * K2 / (K4 * SupportBeamsCount) * K3);
        }
    }

    [JsonIgnore]
    public float Size => (float)Math.Round(GetAllPlatformSizes().Sum(item => ConvertToMeter(item.Length) * ConvertToMeter(item.Width)), 2);

    protected BasePlatformElement() : base()
    {
        PlatformLength.PropertyChanged += ServiceabilityPropertyChanged;
        PlatformWidth.PropertyChanged += ServiceabilityPropertyChanged;
    }

    IEnumerable<PlatformSize> GetAllPlatformSizes()
    {
        yield return new() { Length = PlatformLength.Value, Width = PlatformWidth.Value };
        foreach (var platformSize in PlatformSizes)
            yield return platformSize;
    }
}