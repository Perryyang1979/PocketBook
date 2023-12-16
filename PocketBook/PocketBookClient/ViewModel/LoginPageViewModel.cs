
using PocketBookClient.View;
using PocketBookClient.Helper;
using PocketBookClient.Services;
using GrpcShared;

namespace PocketBookClient.ViewModel;

public partial class LoginPageViewModel : BaseViewModel
{
    public LoginPageViewModel()
    {
    }

    [ObservableProperty]
    private bool isRunning;

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    public string password;

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsRunning)
            return;
        IsRunning = true;


        if (string.IsNullOrEmpty(UserName))
        {
            await Shell.Current.DisplayAlert("Missing UserName", "Please provide valid UserName", "OK");
            IsRunning = false;
            return;
        }

        if (string.IsNullOrEmpty(Password))
        {
            await Shell.Current.DisplayAlert("Missing Password", "Please provide valid Password", "OK");
            IsRunning = false;
            return;
        }

        UserLoginResponse result = await Communication.Instance.UserLoginAsync(UserName.Trim(), RSAService.EncryptData(Password));

        if (result == null)
        {
            await Shell.Current.DisplayAlert("Missing Password", "Please provide valid Password", "OK");
            IsRunning = false;
            Settings.Current.LoggedIn = false;
            return;
        }

        if (!result.Success)
        {
            await Shell.Current.DisplayAlert("Lgin in failed", result.Message, "OK");
            IsRunning = false;
            return;
        }

        Settings.Current.LoggedIn = true;
        Settings.Current.CurrentUserSetting =
            new Settings.CurrentUserLoginSetting()
            {
                UserName = UserName.Trim(),
                UserGroupId = result.UsergroupId,

            };

       // await Shell.Current.DisplayAlert("Lgin in OK", result.Message, "OK");
         await Shell.Current.GoToAsync($"{nameof(PocketOperationPage)}");
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
    }
}
