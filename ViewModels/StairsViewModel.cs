using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Stairs), nameof(Stairs))]
    public partial class StairsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Stairs? stairs;
        
        public StairsSettings StairsSettings { get; private set; }

        public StairsViewModel(IOptions<StairsSettings> stairsSettings)
        {
            StairsSettings = stairsSettings.Value;
        }
    }
}
