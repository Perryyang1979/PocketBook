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
            return;
        }

        //var vm = BindingContext as LoginPageViewModel;
        //await vm?.GotoPocketOperation();

    }

    private async void Signin_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as LoginPageViewModel;
        var result =await  vm?.LoginAsync();

        if( result) {
            // await Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}");
            var navPage = new NavigationPage( (ServiceHelper.GetService<AddTransactionPage>()));
            navPage.Title = "Home";
            try
            {
                await Navigation.PushAsync(navPage);
            }catch(Exception ex) { 
                Console.Write(ex); }
        }
    }
}