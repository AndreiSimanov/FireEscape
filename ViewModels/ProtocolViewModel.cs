
namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Protocol), nameof(Protocol))]
    public partial class ProtocolViewModel: BaseViewModel
    {
        [ObservableProperty]
        Protocol? protocol;

    }
}
