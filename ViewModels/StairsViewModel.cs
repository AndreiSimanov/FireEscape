using CommunityToolkit.Maui.Core.Extensions;
using DevExpress.Data.Extensions;
using DevExpress.Maui.Controls;
using FireEscape.Factories.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;

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
        BaseStairsElement.DefaultUnit = StairsSettings.DefaultUnit;
        BaseStairsElement.DefaultUnitMultiplier = StairsSettings.DefaultUnitMultiplier;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BottomSheetState))]
    BaseStairsElement? selectedStairsElement;

    [ObservableProperty]
    BottomSheetState bottomSheetState;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(BottomSheetState) && BottomSheetState != BottomSheetState.FullExpanded)
            SelectedStairsElement?.UpdateCalcWithstandLoad();
    }

    [RelayCommand]
    void UpdatStairsElements() => EditObject?.UpdateStairsElements();

    [RelayCommand]
    void UpdateStepsCount() => EditObject?.UpdateStepsCount();

    [RelayCommand]
    void SelectStairsElement(BaseStairsElement? element)
    {
        UpdatePlatformSizes(SelectedStairsElement, false);
        BottomSheetState = element == null ? BottomSheetState.Hidden : BottomSheetState.HalfExpanded;
        SelectedStairsElement = element;
        UpdatePlatformSizes(SelectedStairsElement, true);
    }

    void UpdatePlatformSizes(BaseStairsElement? element, bool expand)
    {
        var platformElement = element as BasePlatformElement;
        if (platformElement == null)
            return;

        if (expand)
        {
            var platformSizeStubs = Enumerable.Range(1, MAX_EXPAND_PLATFORM_SIZES - platformElement.PlatformSizes.Count).Select(o => new PlatformSize());
            platformElement.PlatformSizes = platformElement.PlatformSizes.Concat(platformSizeStubs).ToObservableCollection();
        }
        else
        {
            platformElement.PlatformSizes = platformElement.PlatformSizes
                .Where(platformSize => platformSize.Length > 0 || platformSize.Width > 0).ToObservableCollection();
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

    public string StairsWidth => string.Format(AppResources.StairsWidth, StairsSettings.DefaultUnit);
    public string StairsWidthHint => string.Format(AppResources.StairsWidthHint, StairsSettings.DefaultUnit);

    public string Deformation => string.Format(AppResources.Deformation, StairsSettings.DefaultUnit);
    public string DeformationHint => string.Format(AppResources.DeformationHint, StairsSettings.DefaultUnit);

    public  string StairsFenceHeight => string.Format(AppResources.StairsFenceHeight, StairsSettings.DefaultUnit);
    public  string StairsFenceHeightHint => string.Format(AppResources.StairsFenceHeightHint, StairsSettings.DefaultUnit);

    public string StairwayWidth => string.Format(AppResources.StairwayWidth, StairsSettings.DefaultUnit);
    public string StairwayWidthHint => string.Format(AppResources.StairwayWidthHint, StairsSettings.DefaultUnit);

    public string StepsHeight => string.Format(AppResources.StepsHeight, StairsSettings.DefaultUnit);
    public string StepsHeightHint => string.Format(AppResources.StepsHeightHint, StairsSettings.DefaultUnit);

    public string StepsWidth => string.Format(AppResources.StepsWidth, StairsSettings.DefaultUnit);
    public string StepsWidthHint => string.Format(AppResources.StepsWidthHint, StairsSettings.DefaultUnit);

    public string PlatformLength => string.Format(AppResources.PlatformLength, StairsSettings.DefaultUnit);
    public string PlatformLengthHint => string.Format(AppResources.PlatformLengthHint, StairsSettings.DefaultUnit);

    public string PlatformWidth => string.Format(AppResources.PlatformWidth, StairsSettings.DefaultUnit);
    public string PlatformWidthHint => string.Format(AppResources.PlatformWidthHint, StairsSettings.DefaultUnit);
}