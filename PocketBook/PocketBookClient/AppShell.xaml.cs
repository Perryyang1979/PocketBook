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
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(PocketOperationPage), typeof(PocketOperationPage));
        Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
        Routing.RegisterRoute(nameof(CategoryListPage), typeof(CategoryListPage));
        Routing.RegisterRoute(nameof(IdentifyListPage), typeof(IdentifyListPage));
        Routing.RegisterRoute(nameof(PasswordManagePage), typeof(PasswordManagePage));
        Routing.RegisterRoute(nameof(AdvancedReportViewModel), typeof(AdvancedReportViewModel));

    }

    private void logoutButton_Clicked(object sender, EventArgs e)
    {
          Settings.Current.LoggedIn = false;
          Task.Delay(1);
          Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }
}
