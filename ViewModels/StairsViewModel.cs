using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace FireEscape.ViewModels;

public partial class StairsViewModel : BaseEditViewModel<Stairs>
{
    readonly StairsService stairsService;
    readonly IStairsFactory stairsFactory;
    public StairsSettings StairsSettings { get; private set; }

    public StairsViewModel(StairsService stairsService, IOptions<StairsSettings> stairsSettings, IStairsFactory stairsFactory)
    {
        this.stairsService = stairsService;
        this.stairsFactory = stairsFactory;
        StairsSettings = stairsSettings.Value;
        BaseStairsElement.DefaultUnit = StairsSettings.DefaultUnit;
        BaseStairsElement.DefaultUnitMultiplier = StairsSettings.DefaultUnitMultiplier;
    }

    [ObservableProperty]
    BaseStairsElement? selectedStairsElement;

    [RelayCommand]
    void UpdatStairsElements() => EditObject?.UpdatStairsElements();

    [RelayCommand]
    void UpdateStepsCount() => EditObject?.UpdateStepsCount();

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
                    var action = await Shell.Current.DisplayActionSheet(AppResources.SelectStairsElement, AppResources.Cancel, string.Empty, elementNames);
                    var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                    if (element != null)
                    {
                        EditObject.UpdatStairsElement(element);
                        EditObject.StairsElements.Insert(0, element);
                        SelectedStairsElement = element;
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
            if (EditObject == null || element.Required)
                return;
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteStairsElement,
                AppResources.Cancel,
                AppResources.Delete);
            if (string.Equals(action, AppResources.Cancel))
                return;

            EditObject.StairsElements.Remove(element);
            EditObject.UpdateStepsCount();
        },
        element,
        AppResources.DeleteStairsElementError);

    protected override async Task SaveEditObjectAsync() =>
       await DoCommandAsync(async () =>
       {
           await stairsService.SaveAsync(EditObject!);
       },
       EditObject,
       AppResources.SaveProtocolError);

    public string StairsWidth => string.Format(AppResources.StairsWidth, BaseStairsElement.DefaultUnit);
    public string StairsWidthHint => string.Format(AppResources.StairsWidthHint, BaseStairsElement.DefaultUnit);

    public string Deformation => string.Format(AppResources.Deformation, BaseStairsElement.DefaultUnit);
    public string DeformationHint => string.Format(AppResources.DeformationHint, BaseStairsElement.DefaultUnit);
}