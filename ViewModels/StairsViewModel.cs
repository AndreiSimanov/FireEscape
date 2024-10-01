using DevExpress.Maui.Controls;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace FireEscape.ViewModels;

public partial class StairsViewModel(IStairsService stairsService, IOptions<StairsSettings> stairsSettings, ILogger<StairsViewModel> logger) : BaseEditViewModel<Stairs>(logger)
{
    const int MAX_EXPAND_PLATFORM_SIZES = 30;
    bool platformSizesExpanded = false;
    public StairsSettings StairsSettings { get; private set; } = stairsSettings.Value;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BottomSheetState))]
    BaseStairsElement? selectedStairsElement;

    [ObservableProperty]
    PlatformSize[] selectedPlatformSizes = [];

    [ObservableProperty]
    BottomSheetState bottomSheetState;

    [ObservableProperty]
    SupportBeamsP1? supportBeams;

    [ObservableProperty]
    StepsP1? stepsP1;

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

    void SetPlatformSizes(BaseStairsElement? element, bool expand)
    {
        if (element is not BasePlatformElement platformElement || platformSizesExpanded == expand)
            return;

        platformSizesExpanded = expand;

        if (platformSizesExpanded)
        {
            var platformSizeStubs = Enumerable.Range(1, MAX_EXPAND_PLATFORM_SIZES - platformElement.PlatformSizes.Length).Select(o => new PlatformSize());
            SelectedPlatformSizes = platformElement.PlatformSizes.
                Select(item => new PlatformSize
                {
                    Length = ApplicationSettings.PrimaryUnitOfMeasure.ConvertToUnit(item.Length),
                    Width = ApplicationSettings.PrimaryUnitOfMeasure.ConvertToUnit(item.Width)
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
                    Length = ApplicationSettings.PrimaryUnitOfMeasure.ConvertFromUnit(item.Length),
                    Width = ApplicationSettings.PrimaryUnitOfMeasure.ConvertFromUnit(item.Width)
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
            var availableStairsElements = stairsService.GetAvailableStairsElements(EditObject);

            var elementNames = availableStairsElements.Select(item => item.ToString()).ToArray();
            if (elementNames.Length != 0)
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.SelectStairsElement, AppResources.Cancel, string.Empty, elementNames);
                AddStairsElement(availableStairsElements.FirstOrDefault(item => string.Equals(item.ToString(), action)));
            }
        },
        AppResources.AddStairsElementError);

    [RelayCommand]
    void CopyStairsElement(BaseStairsElement stairsElement) =>
     DoBusyCommand(() =>
     {
         if (EditObject == null || stairsElement == null || stairsElement.IsSingleElement)
             return;
         AddStairsElement(stairsService.CopyStairsElement(EditObject, stairsElement));
     },
     AppResources.CopyStairsElementError);

    [RelayCommand]
    Task DeleteElementAsync(BaseStairsElement element) =>
        DoBusyCommandAsync(async () =>
        {
            SelectedStairsElement = element;
            if (EditObject == null || element.IsRequired)
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

        if (e.PropertyName == nameof(EditObject))
        {
            SupportBeams = EditObject!.StairsElements.FirstOrDefault(element => element.StairsElementType == typeof(SupportBeamsP1)) as SupportBeamsP1;
            StepsP1 = EditObject!.StairsElements.FirstOrDefault(element => element.StairsElementType == typeof(StepsP1)) as StepsP1;
        }
    }

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

    void AddStairsElement(BaseStairsElement? stairsElement)
    {
        if (EditObject != null && stairsElement != null)
        {
            EditObject.StairsElements.Insert(0, stairsElement);
            UpdateStepsCount();
            UpdatStairsElements();
            SetPlatformP1Width();
            SelectStairsElement(stairsElement);
        }
    }

    public static string PlatformLength => AddUnitOfMeasure(AppResources.PlatformLength, ApplicationSettings.PrimaryUnitOfMeasure);
    public static string PlatformWidth => AddUnitOfMeasure(AppResources.PlatformWidth, ApplicationSettings.PrimaryUnitOfMeasure);
    static string AddUnitOfMeasure(string val, UnitOfMeasure unitOfMeasure) => string.IsNullOrWhiteSpace(val) ? string.Empty : string.Format(val + " ({0})", unitOfMeasure.Symbol);
}