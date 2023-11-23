
namespace PocketBookClient.View;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }
}