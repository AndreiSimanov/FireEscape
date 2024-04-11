namespace FireEscape.Views;

public partial class UserAccountPage : ContentPage
{
	public UserAccountPage(UserAccountViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    void ContentPage_Disappearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as UserAccountViewModel;
        viewModel?.SaveUserAccountCommand.Execute(null);
    }
}