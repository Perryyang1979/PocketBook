using PocketBookClient.Services;

namespace PocketBookClient.View;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(!Settings.Current.LoggedIn)
        {
            return;
        }

        var vm = BindingContext as LoginPageViewModel;
        await  vm?.GotoPocketOperation();

    }
}