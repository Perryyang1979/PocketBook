using PocketBookClient.View;
using PocketBookClient.Services;

namespace PocketBookClient;

public partial class AppShell : Shell
{
    public string UserName;
	public AppShell()
	{
        AppShell.InitializeRouting();
        InitializeComponent();
        BindingContext = new AppShellViewModel();
    }

    private static void InitializeRouting()
    {
        Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
        Routing.RegisterRoute(nameof(ScannerPage), typeof(ScannerPage));
        Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(CategoryListPage), typeof(CategoryListPage));
        Routing.RegisterRoute(nameof(IdentifyListPage), typeof(IdentifyListPage));
        Routing.RegisterRoute(nameof(PasswordManagePage), typeof(PasswordManagePage));
        Routing.RegisterRoute(nameof(AdvancedReportPage), typeof(AdvancedReportPage));
    }

    private void logoutButton_Clicked(object sender, EventArgs e)
    {
          Settings.Current.LoggedIn = false;
           Settings.Current.CurrentUserSetting = null;
          Task.Delay(1);
          Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }
}
