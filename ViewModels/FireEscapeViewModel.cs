using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Models.FireEscape), nameof(Models.FireEscape))]
    public partial class FireEscapeViewModel : BaseViewModel
    {
        [ObservableProperty]
        Models.FireEscape? fireEscape;
        
        public FireEscapeSettings FireEscapeSettings { get; private set; }

        public FireEscapeViewModel(IOptions<FireEscapeSettings> fireEscapeSettings)
        {
            FireEscapeSettings = fireEscapeSettings.Value;
        }
    }
}
