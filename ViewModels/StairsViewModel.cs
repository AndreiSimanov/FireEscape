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

    public StairsViewModel(StairsService stairsService, IOptions<StairsSettings> stairsSettings, IStairsFactory stairsFactory, ILogger<StairsViewModel> logger) : base(logger)
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
    void UpdateStepsCount()
    {
        if (EditObject != null && EditObject.BaseStairsType == BaseStairsTypeEnum.P2)
            EditObject.StepsCount = EditObject.StairsElements.Where(element => element.StairsElementType == typeof(StairwayP2)).Sum(item => ((StairwayP2)item).StepsCount);
    }
    
    [RelayCommand]
    void SetPlatformP1Width()
    {
        if (EditObject != null && EditObject.BaseStairsType == BaseStairsTypeEnum.P1)
        {
            if (EditObject.StairsElements.FirstOrDefault(element => element.StairsElementType == typeof(PlatformP1)) is PlatformP1 platformP1)
            {
                if (platformP1.PlatformWidth.Value == 0)
                    platformP1.PlatformWidth.Value = EditObject.StairsWidth.Value;
            }
        }
    }

    [RelayCommand]
    void SelectStairsElement(BaseStairsElement? element)
    {
        BottomSheetState = element == null ? BottomSheetState.Hidden : BottomSheetState.HalfExpanded;
        SelectedStairsElement = element;
    }

    [RelayCommand]
    void HideBottomSheet() => BottomSheetState = BottomSheetState.Hidden;

    bool expanded = false;

    void SetPlatformSizes(BaseStairsElement? element, bool expand)
    {
        if (element is not BasePlatformElement platformElement || expanded == expand)
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
    Task AddStairsElementAsync() =>
        DoBusyCommandAsync(async () =>
        {
            if (EditObject == null)
                return;
            var availableStairsElements = stairsFactory.GetAvailableStairsElements(EditObject);
            if (EditObject.BaseStairsType == BaseStairsTypeEnum.P2)
            {
                var platformIndex = EditObject.StairsElements.FindIndex(element => element.StairsElementType == typeof(PlatformP2));
                var stairwayIndex = EditObject.StairsElements.FindIndex(element => element.StairsElementType == typeof(StairwayP2));
                if ((stairwayIndex > platformIndex || stairwayIndex == -1) && platformIndex != -1)
                    availableStairsElements = availableStairsElements.OrderBy(item => item.Name);
                else
                    availableStairsElements = availableStairsElements.OrderByDescending(item => item.Name);
            }

            var elementNames = availableStairsElements.Select(item => item.ToString()).ToArray();
            if (elementNames.Length != 0)
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.SelectStairsElement, AppResources.Cancel, string.Empty, elementNames);
                var element = availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action));
                if (element != null)
                {
                    EditObject.StairsElements.Insert(0, element);
                    UpdatStairsElements();
                    SetPlatformP1Width();
                    SelectStairsElement(element);
                }
            }
        },
        AppResources.AddProtocolError);

    [RelayCommand]
    Task DeleteElementAsync(BaseStairsElement element) =>
        DoBusyCommandAsync(async () =>
        {
            SelectedStairsElement = element;
            if (EditObject == null || element.Required)
                return;
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteStairsElement, AppResources.Cancel, AppResources.Delete);
            if (string.Equals(action, AppResources.Delete))
            {
                EditObject.StairsElements.Remove(element);
                UpdateStepsCount();
                UpdatStairsElements();
                SelectStairsElement(null);
            }
        },
        element,
        AppResources.DeleteStairsElementError);

    protected override List<string> GetEditObjectValidationResult()
    {
        var result = base.GetEditObjectValidationResult();
        result.AddRange(stairsService.Validate(EditObject!).Errors.Select(error => error.ErrorMessage).Distinct());
        return result;
    }

    protected override Task SaveEditObjectAsync() => 
        DoCommandAsync(() => stairsService.SaveAsync(EditObject!),
            EditObject,
            AppResources.SaveProtocolError);

    public static string PlatformLength => AddUnitOfMeasure(AppResources.PlatformLength, UnitOfMeasureSettings.PrimaryUnitOfMeasure);
    public static string PlatformWidth => AddUnitOfMeasure(AppResources.PlatformWidth, UnitOfMeasureSettings.PrimaryUnitOfMeasure);
    static string AddUnitOfMeasure(string val, UnitOfMeasure unitOfMeasure) => string.IsNullOrWhiteSpace(val) ? string.Empty : string.Format(val + " ({0})", unitOfMeasure.Symbol);
}