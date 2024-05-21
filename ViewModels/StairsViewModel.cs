using DevExpress.Data.Extensions;
using DevExpress.Maui.Controls;
using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Linq;

namespace FireEscape.ViewModels;

public partial class StairsViewModel : BaseEditViewModel<Stairs>
{
    const int MAX_EXPAND_PLATFORM_SIZES = 30;
    readonly StairsService stairsService;
    readonly IStairsFactory stairsFactory;
    public StairsSettings StairsSettings { get; private set; }

    public StairsViewModel(StairsService stairsService, IOptions<StairsSettings> stairsSettings, IStairsFactory stairsFactory)
    {
        this.stairsService = stairsService;
        this.stairsFactory = stairsFactory;
        StairsSettings = stairsSettings.Value;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BottomSheetState))]
    BaseStairsElement? selectedStairsElement;

    [ObservableProperty]
    PlatformSize[] selectedPlatformSizes = [];

    [ObservableProperty]
    BottomSheetState bottomSheetState;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(BottomSheetState))
        {
            if (BottomSheetState == BottomSheetState.FullExpanded)
            {
                SetPlatformSizes(SelectedStairsElement, true);
            }
            else
            {
                SetPlatformSizes(SelectedStairsElement, false);
                SelectedStairsElement?.UpdateCalcWithstandLoad();
            }
        }
    }

    [RelayCommand]
    void UpdatStairsElements() => EditObject?.UpdateStairsElements();

    [RelayCommand]
    void UpdateStepsCount() => EditObject?.UpdateStepsCount();

    [RelayCommand]
    void SelectStairsElement(BaseStairsElement? element)
    {
        BottomSheetState = element == null ? BottomSheetState.Hidden : BottomSheetState.HalfExpanded;
        SelectedStairsElement = element;
    }

    bool expanded = false;

    void SetPlatformSizes(BaseStairsElement? element, bool expand)
    {
        var platformElement = element as BasePlatformElement;
        if (platformElement == null || expanded == expand)
            return;

        expanded = expand;

        if (expanded)
        {
            var platformSizeStubs = Enumerable.Range(1, MAX_EXPAND_PLATFORM_SIZES - platformElement.PlatformSizes.Length).Select(o => new PlatformSize());
            SelectedPlatformSizes = platformElement.PlatformSizes.
                Select(item => new PlatformSize
                {
                    Length = item.Length / UnitOfMeasureSettings.PrimaryUnitOfMeasure.Multiplier,
                    Width = item.Width / UnitOfMeasureSettings.PrimaryUnitOfMeasure.Multiplier
                }).
                Concat(platformSizeStubs).
                ToArray();
        }
        else
        {
            platformElement.PlatformSizes = SelectedPlatformSizes.
                Where(platformSize => platformSize.Length > 0 || platformSize.Width > 0).
                Select(item => new PlatformSize
                {
                    Length = item.Length * UnitOfMeasureSettings.PrimaryUnitOfMeasure.Multiplier,
                    Width = item.Width * UnitOfMeasureSettings.PrimaryUnitOfMeasure.Multiplier
                }).
                ToArray();
            SelectedPlatformSizes = [];
        }
    }

    [RelayCommand]
    async Task AddStairsElementAsync()
    {
        await DoCommandAsync(async () =>
        {
            if (EditObject == null)
                return;
            var availableStairsElements = stairsFactory.GetAvailableStairsElements(EditObject);
            if (EditObject.BaseStairsType == BaseStairsTypeEnum.P2)
            {
                var platformIndex = EditObject.StairsElements.FindIndex(element => element.StairsElementType == StairsElementTypeEnum.PlatformP2);
                var stairwayIndex = EditObject.StairsElements.FindIndex(element => element.StairsElementType == StairsElementTypeEnum.StairwayP2);
                if ((stairwayIndex > platformIndex || stairwayIndex == -1) && platformIndex != -1)
                    availableStairsElements = availableStairsElements.OrderBy(item => item.Name);
                else
                    availableStairsElements = availableStairsElements.OrderByDescending(item => item.Name);
            }

            var elementNames = availableStairsElements.Select(item => item.ToString()).ToArray();
            if (elementNames.Any())
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.SelectStairsElement, AppResources.Cancel, string.Empty, elementNames);
                var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                if (element != null)
                {
                    EditObject.UpdateStairsElement(element);
                    EditObject.StairsElements.Insert(0, element);
                    SelectStairsElement(element);
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
            if (SelectedStairsElement == element)
                SelectStairsElement(null);
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

    public string PlatformLength => AddUnitOfMeasure(AppResources.PlatformLength, UnitOfMeasureSettings.PrimaryUnitOfMeasure);
    public string PlatformWidth => AddUnitOfMeasure(AppResources.PlatformWidth, UnitOfMeasureSettings.PrimaryUnitOfMeasure);
    string AddUnitOfMeasure(string val, UnitOfMeasure unitOfMeasure ) => string.IsNullOrWhiteSpace(val) ? string.Empty : string.Format(val + " ({0})", unitOfMeasure.Symbol);
}