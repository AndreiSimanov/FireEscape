using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;


namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Models.FireEscape), nameof(Models.FireEscape))]
    public partial class FireEscapeViewModel : BaseViewModel
    {
        [ObservableProperty]
        Models.FireEscape? fireEscape;
        
        public FireEscapePropertiesSettings FireEscapePropertiesSettings { get; private set; }

        public FireEscapeViewModel(IOptions<FireEscapePropertiesSettings> fireEscapePropertiesSettings)
        {
            FireEscapePropertiesSettings = fireEscapePropertiesSettings.Value;
        }

    }
}
