using DevExpress.Maui.CollectionView;

namespace FireEscape.Views;

public partial class StairsPage : BaseStairsPage
{
    const int STAIRS_ELEMENTS_HEADER_HEIGHT = 83;
    public StairsPage(StairsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        Content.SizeChanged += ContentSizeChanged;
    }

    void ContentSizeChanged(object? sender, EventArgs e) => 
        stairsElements.HeightRequest = Content.Height - STAIRS_ELEMENTS_HEADER_HEIGHT;

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

    void StairsElementsScrolled(object sender, DXCollectionViewScrolledEventArgs e) => SetSelectedStairsElements();

    void SetSelectedStairsElements(BaseStairsElement? element = null)
    {
        ViewModel?.SelectStairsElementCommand.Execute(element);
    }

    private void PlatformSizesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (platformSizes == null)
            return;
        if (e.PropertyName == nameof(platformSizes.ItemsSource))
        {
            var itemsSource = platformSizes.ItemsSource as PlatformSize[];
            if (itemsSource != null && itemsSource.Any())
                platformSizes.ScrollToRow(0);
        }

        if (e.PropertyName == nameof(platformSizes.Y) && platformSizes.IsVisible)
            platformSizes.HeightRequest = Height - platformSizes.Y - platformSizesStackLayout.Y;
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