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
        Routing.RegisterRoute(nameof(SettingPage), typeof(SettingPage));
        Routing.RegisterRoute(nameof(CategoryListPage), typeof(CategoryListPage));
        Routing.RegisterRoute(nameof(IdentifyListPage), typeof(IdentifyListPage));
        Routing.RegisterRoute(nameof(PasswordManagePage), typeof(PasswordManagePage));
        Routing.RegisterRoute(nameof(AdvancedReportPage), typeof(AdvancedReportPage));
        Routing.RegisterRoute(nameof(UserListPage), typeof(UserListPage));
    }

    private void logoutButton_Clicked(object sender, EventArgs e)
    {
          Settings.Current.LoggedIn = false;
          Settings.Current.CurrentUserSetting = null;
        try
        {
            SecureStorage.Remove("perry_token");
            var exist = SecureStorage.GetAsync("perry_token").Result;

            if(!string.IsNullOrEmpty(exist))
            {
                SecureStorage.SetAsync("perry_token",null);
            }
        }
        catch(Exception ex)
        {

        }
          Task.Delay(1);
          Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }

    private void UserButton_Clicked(object sender, EventArgs e)
    {
        Task.Delay(1);
        Shell.Current.GoToAsync($"{nameof(UserListPage)}");
    }
}
