namespace PocketBookClient.View;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        this.BindingContext = new LoginPageViewModel();
    }
}