using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels;

public partial class StairsViewModel(StairsService stairsService, IOptions<StairsSettings> stairsSettings, IStairsFactory stairsFactory) : BaseEditViewModel<Stairs>
{
    public StairsSettings StairsSettings { get; private set; } = stairsSettings.Value;

    [RelayCommand]
    async Task AddStairsElementAsync()
    {
        await DoCommandAsync(async () =>
        {
            if (EditObject == null)
                return;
            var availableStairsElements = stairsFactory.GetAvailableStairsElements(EditObject).ToList();
            if (availableStairsElements != null)
            {
                var elementNames = availableStairsElements.Select(item => item.ToString()).ToArray();
                var action = await Shell.Current.DisplayActionSheet("Выберете элемент", AppResources.Cancel, string.Empty, elementNames);
                var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                if (element != null)
                    EditObject.StairsElements.Add(element);
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

            EditObject!.StairsElements.Remove(element);
        },
        element,
        AppResources.DeleteStairsElementError);

    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await stairsService.SaveStairsAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveProtocolError);
}