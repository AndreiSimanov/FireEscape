using DevExpress.Maui.CollectionView;

namespace FireEscape.Views;

public partial class StairsPage : BaseStairsPage
{
    const int STAIRS_ELEMENTS_HEADER_HEIGHT = 103;
    public StairsPage(StairsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        Content.SizeChanged += ContentSizeChanged;
        stairsElements.Scrolled += StairsElementsScrolled;
    }

    void ContentSizeChanged(object? sender, EventArgs e) =>
        stairsElements.HeightRequest = Height - STAIRS_ELEMENTS_HEADER_HEIGHT;

    async void AddStairsElement(object sender, EventArgs e)
    {
        if (ViewModel != null)
        {
            await ViewModel.AddStairsElementCommand.ExecuteAsync(null);
            await ScrollUpStairsElementsAsync();
        }
    }

    async void CopyStairsElement(object sender, SwipeItemTapEventArgs e)
    {
        if (e.Item is not BaseStairsElement stairsElement)
            return;
        ViewModel?.CopyStairsElementCommand.Execute(stairsElement);
        await ScrollUpStairsElementsAsync();
    }

    async Task ScrollUpStairsElementsAsync()
    {
        stairsElements.Scrolled -= StairsElementsScrolled;
        stairsElements.BeginUpdate();
        stairsElements.ScrollTo(0);
        stairsElements.EndUpdate();
        await Task.Delay(500); // preventing call HideBottomSheet while stairsElements scrolling
        stairsElements.Scrolled -= StairsElementsScrolled;
        stairsElements.Scrolled += StairsElementsScrolled;
    }

    void StairsTypeChanged(object sender, EventArgs e)
    {
        SetSelectedStairsElements();
        ViewModel?.SetPlatformP1WidthCommand.Execute(null);
        var baseStairsType = ViewModel?.EditObject?.BaseStairsType;
        if (!baseStairsType.HasValue)
            baseStairsType = BaseStairsTypeEnum.P1;
        stairsElements.FilterString = $"[BaseStairsType] == {(int)baseStairsType.Value}";
    }

    protected override void ContentPageDisappearing(object sender, EventArgs e)
    {
        SetSelectedStairsElements();
        base.ContentPageDisappearing(sender, e);
    }

    void EditorFocused(object sender, FocusEventArgs e) => HideBottomSheet();

    void ScrollViewScrolled(object sender, ScrolledEventArgs e) => HideBottomSheet();

    void StairsElementsScrolled(object? sender, DXCollectionViewScrolledEventArgs e) => HideBottomSheet();

    void SetSelectedStairsElements(BaseStairsElement? element = null) => ViewModel?.SelectStairsElementCommand.Execute(element);

    void HideBottomSheet() => ViewModel?.HideBottomSheetCommand.Execute(null);

    void StairsWidthEditorUnfocused(object sender, FocusEventArgs e) => ViewModel?.SetPlatformP1WidthCommand.Execute(null);

    void PlatformSizesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (platformSizes == null)
            return;
        if (e.PropertyName == nameof(platformSizes.ItemsSource))
        {
            if (platformSizes.ItemsSource is PlatformSize[] itemsSource && itemsSource.Length != 0)
                platformSizes.ScrollToRow(0);
        }

        if (e.PropertyName == nameof(platformSizes.Y) && platformSizes.IsVisible)
            platformSizes.HeightRequest = Height - bottomSheet.GrabberOffset;
    }

    /*
        void EditorFocused(object sender, FocusEventArgs e) // avoid set focus on invisible controls
        {
            var editor = sender as NumericEdit;
            if (editor == null) 
                return;
            if (e.IsFocused && !editor.IsVisible)
            {
                var parent = editor.Parent as Layout;
                if (parent != null)
                {
                    var editorIdx = parent.Children.IndexOf(editor);
                    parent.Children[editorIdx +1].Focus();
                }
            }
        }
    */
}