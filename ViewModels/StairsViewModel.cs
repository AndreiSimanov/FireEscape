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

    public string StairsWidth => AddDefaultUnit(AppResources.StairsWidth);
    public string StairsWidthHint => AddDefaultUnit(AppResources.StairsWidthHint);

    public string Deformation => AddDefaultUnit(AppResources.Deformation);
    public string DeformationHint => AddDefaultUnit(AppResources.DeformationHint);

    public  string StairsFenceHeight => AddDefaultUnit(AppResources.StairsFenceHeight);
    public  string StairsFenceHeightHint => AddDefaultUnit(AppResources.StairsFenceHeightHint);

    public string StairwayWidth => AddDefaultUnit(AppResources.StairwayWidth);
    public string StairwayWidthHint => AddDefaultUnit(AppResources.StairwayWidthHint);

    public string StepsHeight => AddDefaultUnit(AppResources.StepsHeight);
    public string StepsHeightHint => AddDefaultUnit(AppResources.StepsHeightHint);

    public string StepsWidth => AddDefaultUnit(AppResources.StepsWidth);
    public string StepsWidthHint => AddDefaultUnit(AppResources.StepsWidthHint);

    public string PlatformLength => AddDefaultUnit(AppResources.PlatformLength);
    public string PlatformLengthHint => AddDefaultUnit(AppResources.PlatformLengthHint);

    public string PlatformWidth => AddDefaultUnit(AppResources.PlatformWidth);
    public string PlatformWidthHint => AddDefaultUnit(AppResources.PlatformWidthHint);

    string AddDefaultUnit(string val) => string.IsNullOrWhiteSpace(val) ? string.Empty : string.Format(val + " ({0})", StairsSettings.DefaultUnit);
}