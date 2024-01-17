namespace PocketBookClient.View;

public partial class UserListPage : ContentPage
{
	public UserListPage(UserListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}