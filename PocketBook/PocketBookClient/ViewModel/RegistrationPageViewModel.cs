
using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel;

public partial class RegistrationPageViewModel : BaseViewModel
{
    public RegistrationPageViewModel()
    {
        
    }
    [ObservableProperty]
    private bool isRunning;
    [ObservableProperty]
    private string userName;
    [ObservableProperty]
    public string password;
    [ObservableProperty]
    public string repeatPassword;
    [ObservableProperty]
    public string email;
    [ObservableProperty]
    public string mobileNumber;

    [RelayCommand]
    public async Task SignUpAsync()
    {

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

        if (RepeatPassword != Password)
        {
            await Shell.Current.DisplayAlert("Password is not Same", "Please make sure password is same", "OK");
            IsRunning = false;
            return;
        }

        if (string.IsNullOrEmpty(Email))
        {
            await Shell.Current.DisplayAlert("Missing Email Address", "Please provide valid Email Address", "OK");
            IsRunning = false;
            return;
        }

        if (string.IsNullOrEmpty(MobileNumber))
        {
            await Shell.Current.DisplayAlert("Missing MobileNumber", "Please provide valid Mobile", "OK");
            IsRunning = false;
            return;
        }

        var user = new User()
        {
            Name = UserName.Trim(),
            Password = Password,
            Email = Email,
            PhoneNumber = MobileNumber,
        };

        var response = await Communication.Instance.RegisterUserAsync(user);

        if (response == null)
        {
            await Shell.Current.DisplayAlert("Register failed", "Unknow error", "OK");
            IsRunning = false;
            Settings.Current.LoggedIn = false;
            return;
        }

        if (!response.Success)
        {
            await Shell.Current.DisplayAlert("Register failed", response.Message, "OK");
            IsRunning = false;
            return;
        }

        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
