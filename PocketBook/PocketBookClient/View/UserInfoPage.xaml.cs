namespace PocketBookClient.View;

public partial class UserInfoPage : ContentPage
{
	public UserInfoPage(UserInfoPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}