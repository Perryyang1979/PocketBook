using PocketBookClient.Helper;
using PocketBookClient.Services;
using System.Linq.Expressions;

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

        if (!Settings.Current.LoggedIn)
        {
            var vm = BindingContext as LoginPageViewModel;
            vm?.Init();
        }
    }
}