using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;

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
                if (elementNames.Any())
                {
                    var action = await Shell.Current.DisplayActionSheet("Выберете элемент", AppResources.Cancel, string.Empty, elementNames);
                    var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                    if (element != null)
                    {
                        element.StairsHeight = EditObject.StairsHeight.Value;
                        EditObject.StairsElements.Insert(0, element);
                    }
                }
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EditObject))
            EditObject?.UpdatStairsElementsCommand.Execute(null);
        base.OnPropertyChanged(e);
    }

    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await stairsService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveProtocolError);

    public string StairsWidth => string.Format(AppResources.StairsWidth, StairsSettings.DefaultUnit);
    public string StairsWidthHint => string.Format(AppResources.StairsWidthHint, StairsSettings.DefaultUnit);

    public string Deformation => string.Format(AppResources.Deformation, StairsSettings.DefaultUnit);
    public string DeformationHint => string.Format(AppResources.DeformationHint, StairsSettings.DefaultUnit);
}