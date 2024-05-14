namespace FireEscape.Views;

public partial class StairsPage : BaseStairsPage
{
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
        SetSelectedStairsElements();
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

    void EditorFocused(object sender, FocusEventArgs e) => SetSelectedStairsElements();

    void ScrollViewScrolled(object sender, ScrolledEventArgs e) => SetSelectedStairsElements();

    void StairsElementsScrolled(object sender, DevExpress.Maui.CollectionView.DXCollectionViewScrolledEventArgs e) => SetSelectedStairsElements();

    void SetSelectedStairsElements(BaseStairsElement? element = null)
    {
        if (ViewModel != null)
            ViewModel.SelectStairsElementCommand.Execute(element);
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