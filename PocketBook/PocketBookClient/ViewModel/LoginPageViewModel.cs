
using PocketBookClient.View;
using PocketBookClient.Helper;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel;

public partial class LoginPageViewModel : BaseViewModel
{
    public LoginPageViewModel()
    {
        
    }

    public void Init()
    {
        IsRunning = false;
        UserName = string.Empty;
        Password = string.Empty;
    }

    [ObservableProperty]
    private bool isRunning;

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    public string password;
    [RelayCommand]
    public async Task<bool> LoginAsync()
    {
        if (IsRunning)
            return false;
        IsRunning = true;


        if (string.IsNullOrEmpty(UserName))
        {
             await Shell.Current.DisplayAlert("Missing UserName", "Please provide valid UserName", "OK");
            IsRunning = false;
            return false;
        }

        if (string.IsNullOrEmpty(Password))
        {
             await Shell.Current.DisplayAlert("Missing Password", "Please provide valid Password", "OK");
            IsRunning = false;
            return false;
        }

         var result =  Communication.Instance.LoginUser(UserName.Trim(), Password);

        //UserLoginResponse result = await Communication.Instance.UserLoginAsync(UserName.Trim(), Password);

        if (result == null)
        {
            await Shell.Current.DisplayAlert("Missing Password", "Please provide valid User Name or Password", "OK");
            IsRunning = false;
            Settings.Current.LoggedIn = false;
            return false;
        }


        Settings.Current.LoggedIn = true;
        Settings.Current.CurrentUserSetting =
            new Settings.CurrentUserLoginSetting()
            {
                UserName = UserName.Trim(),
                UserGroupId = result.GroupId,

            };
        await Shell.Current.GoToAsync($"//{nameof(AddTransactionPage)}");
        return true;
        // await Shell.Current.DisplayAlert("Lgin in OK", result.Message, "OK");
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
    }

    [RelayCommand]
    public async Task ScannerAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(ScannerPage)}");
    }

    [RelayCommand]
    public async Task ForgotPasswordAsync()
    {
        await Shell.Current.GoToAsync(nameof(PasswordManagePage), true, new Dictionary<string, object>
        {
            {"PasswordOperation", 0}
        });
    }

    public async Task GotoPocketOperation()
    {
        await Shell.Current.GoToAsync($"{nameof(AddTransactionPage)}");
    }
}
