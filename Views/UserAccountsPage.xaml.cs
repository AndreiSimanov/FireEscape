namespace FireEscape.Views;

public partial class UserAccountsPage : ContentPage
{
	public UserAccountsPage(UserAccountsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private UserAccountsViewModel? UserAccountsViewModel
    {
        get
        {
            return BindingContext as UserAccountsViewModel;
        }
    }


    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (UserAccountsViewModel != null)
        {
            /*
            MainViewModel.GetProtocolsCommand.Execute(null);
            if (MainViewModel.SelectedProtocol != null)
            {
                var index = MainViewModel.Protocols.IndexOf(MainViewModel.SelectedProtocol);
                protocols.ScrollTo(index);
                MainViewModel.SelectedProtocol = null;
            }
            */
        }
    }
}