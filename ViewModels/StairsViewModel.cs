using FireEscape.Factories;
using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Stairs), nameof(Stairs))]
public partial class StairsViewModel(IOptions<StairsSettings> stairsSettings, StairsFactory stairsFactory) : BaseViewModel
{
    [ObservableProperty]
    Stairs? stairs;

    public StairsSettings StairsSettings { get; private set; } = stairsSettings.Value;

    [RelayCommand]
    async Task AddStairsElementAsync()
    {
        await DoCommandAsync(async () =>
        {
            if (Stairs == null)
                return;
            var availableStairsElements = stairsFactory.GetAvailableStairsElements(Stairs).ToList();
            if (availableStairsElements != null)
            {
                var elementNames = availableStairsElements.Select(item => item.ToString()).ToArray();
                var action = await Shell.Current.DisplayActionSheet("Выберете элемент", AppResources.Cancel, string.Empty, elementNames);
                var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                if (element != null)
                    Stairs.StairsElements.Add(element);
            }
        },
        AppResources.AddProtocolError);
    }

    [RelayCommand]
    async Task DeleteElementAsync(BaseStairsElement element) =>
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