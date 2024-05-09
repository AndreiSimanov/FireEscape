using DevExpress.Maui.Editors;

namespace FireEscape.Views;

public partial class StairsPage : BaseStairsPage
{
    public StairsPage(StairsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        stairsElements.HeightRequest = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density - 180;
    }

    void AddStairsElement(object sender, EventArgs e)
    {
        ViewModel?.AddStairsElementCommand.Execute(null);
        stairsElements.ScrollTo(0);
    }

    void StairsTypeChanged(object sender, EventArgs e)
    {
        var cbx = sender as ComboBoxEdit;
        if (cbx == null)
            return;
        var stairsType = cbx.SelectedItem as StairsType?;
        if (stairsType != null)
            stairsElements.FilterString = $"[BaseStairsType] == {(int)stairsType.Value.BaseStairsType}";
    }

    private void EditorFocused(object sender, FocusEventArgs e) // avoid set focus on invisible controls
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
}