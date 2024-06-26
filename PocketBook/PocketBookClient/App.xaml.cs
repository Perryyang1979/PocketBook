using PocketBookClient.View;
using PocketBookClient.Services;
using PocketBookClient.Helper;
using PocketBookClient.Resources.Languages;
using System.Globalization;

namespace PocketBookClient;


public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Settings.Current.LoggedIn = false;
      //  MainPage = new LoginPage(ServiceHelper.GetService<LoginPageViewModel>());
        MainPage = new AppShell();
      //  CheckLoginStatusAsync();
    }

    public LocalizationResourceManager LocalizationResourceManager
  => LocalizationResourceManager.Instance;


    public async void CheckLoginStatusAsync()
    {
        if (Settings.Current.LoggedIn == false)
        {
            await GoToLogin();
        }
        else
        {
            await GoToHome();

        }
    }

    public static System.Threading.Tasks.Task GoToLogin()
    {
        return Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }
    public static System.Threading.Tasks.Task GoToHome()
    {
        return Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}");
    }

    protected override void OnStart()
    {
        OnResume();
    }

    protected override void OnSleep()
    {
        ThemeHelper.SetTheme();
        RequestedThemeChanged -= App_RequestedThemeChanged;
    }

    protected override void OnResume()
    {
        ThemeHelper.SetTheme();
        RequestedThemeChanged += App_RequestedThemeChanged;
    }

    private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ThemeHelper.SetTheme();
        });
    }
}
