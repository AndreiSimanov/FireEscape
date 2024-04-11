using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Stairs), nameof(Stairs))]
    public partial class StairsViewModel(IOptions<StairsSettings> stairsSettings) : BaseViewModel
    {
        [ObservableProperty]
        Stairs? stairs;

        public StairsSettings StairsSettings { get; private set; } = stairsSettings.Value;

        [RelayCommand]
        async Task DeleteElement(BaseStairsElement element) =>
            await DoCommandAsync(async () =>
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteStairsElement,
                    AppResources.Cancel,
                    AppResources.Delete);
                if (string.Equals(action, AppResources.Cancel))
                    return;

                Stairs!.StairsElements.Remove(element);
            },
            element,
            AppResources.DeleteStairsElementError);
    }
}