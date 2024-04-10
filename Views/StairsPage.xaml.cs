using DevExpress.Maui.Editors;

namespace FireEscape.Views;

public partial class StairsPage : ContentPage
{
    public StairsPage(StairsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        stairsElements.HeightRequest = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density - 180;
    }

    void AddStairsElement(object sender, EventArgs e)
    {

    }

    void StairsTypeChanged(object sender, EventArgs e)
    {
        var cbx = sender as ComboBoxEdit;
        if (cbx == null)
            return;
        var stairsType = cbx.SelectedItem as StairsType?;
        if (stairsType != null )
            stairsElements.FilterString = stairsType.Value.Type == StairsTypeEnum.P2 ?  "[StairsType] == 2" : "[StairsType] == 1";
    }
}