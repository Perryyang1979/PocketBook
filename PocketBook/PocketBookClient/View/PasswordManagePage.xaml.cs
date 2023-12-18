namespace PocketBookClient.View;

public partial class PasswordManagePage : ContentPage
{
	public PasswordManagePage(PasswordManageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}