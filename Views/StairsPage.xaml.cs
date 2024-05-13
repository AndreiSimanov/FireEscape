using DevExpress.Maui.Controls;

namespace FireEscape.Views;

public partial class StairsPage : BaseStairsPage
{
    const double MAX_BOTTOM_SHEET_HEIGHT = .5;
    const double MIN_BOTTOM_SHEET_HEIGHT = 0;

    public StairsPage(StairsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        stairsElements.HeightRequest = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density - 200;
    }

    void AddStairsElement(object sender, EventArgs e)
    {
        ViewModel?.AddStairsElementCommand.Execute(null);
        stairsElements.ScrollTo(0);
    }

    void StairsTypeChanged(object sender, EventArgs e)
    {
        SetBottomSheetHeight(0);
        var baseStairsType = ViewModel?.EditObject?.BaseStairsType;
        if (!baseStairsType.HasValue)
            baseStairsType = BaseStairsTypeEnum.P1;
        stairsElements.FilterString = $"[BaseStairsType] == {(int)baseStairsType.Value}";
    }

    void EditorFocused(object sender, FocusEventArgs e) => SetBottomSheetHeight(0);

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

    protected override void ContentPageDisappearing(object sender, EventArgs e)
    {
        SetBottomSheetHeight(0);
        base.ContentPageDisappearing(sender, e);
    }

    void StairsElementsSelectionChanged(object sender, DevExpress.Maui.CollectionView.CollectionViewSelectionChangedEventArgs e) => UpdateBottomSheet();

    void StairsElementsTap(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        if (bottomSheet.State == BottomSheetState.Hidden)
            UpdateBottomSheet();
    }

    void ScrollViewScrolled(object sender, ScrolledEventArgs e) => SetBottomSheetHeight(MIN_BOTTOM_SHEET_HEIGHT);

    void StairsElementsScrolled(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e) => SetBottomSheetHeight(MIN_BOTTOM_SHEET_HEIGHT);


    void UpdateBottomSheet()
    {
        bottomSheet.State = ViewModel?.SelectedStairsElement == null ? BottomSheetState.Hidden : BottomSheetState.HalfExpanded;
        if (bottomSheet.State == BottomSheetState.HalfExpanded)
            SetBottomSheetHeight(MAX_BOTTOM_SHEET_HEIGHT);
    }

    void SetBottomSheetHeight(double height)
    {
        if (bottomSheet.State == BottomSheetState.HalfExpanded && bottomSheet.HalfExpandedRatio != height)
        {
            bottomSheet.Animate(nameof(bottomSheet), x => bottomSheet.HalfExpandedRatio = x, bottomSheet.HalfExpandedRatio, height);
            if (height == 0)
                bottomSheet.State = BottomSheetState.Hidden;
        }
    }
}