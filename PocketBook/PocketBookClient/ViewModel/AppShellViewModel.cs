
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class AppShellViewModel : BaseViewModel
    {
        public AppShellViewModel()
        {
            Settings.Current.OnCurrentUserChangeEvent += Current_OnCurrentUserChangeEvent;
        }

        private void Current_OnCurrentUserChangeEvent(object sender, EventArgs e)
        {
            LoginUserName = Settings.Current.CurrentUserSetting == null ? string.Empty : Settings.Current.CurrentUserSetting.UserName;
            if (LoginUserName == string.Empty)
            {
                LogoutButtonVisible = false;
                LoginButtonVisible = true;
                HomeItemEnable = false;
            }
            else
            {
                LogoutButtonVisible = true;
                LoginButtonVisible = false;
                HomeItemEnable = true;

                if(LoginUserName == "Perry")
                {
                    LogoutButtonVisible = false;
                    UserButtonVisible = true;
                }
                else
                {
                    UserButtonVisible = false;
                }
            }
        }

        [ObservableProperty]
        public string loginUserName = string.Empty;
        [ObservableProperty]
        bool homeItemEnable = false;
        [ObservableProperty]
        bool logoutButtonVisible = false;
        [ObservableProperty]
        bool loginButtonVisible = true;
        [ObservableProperty]
        bool userButtonVisible = false;

        [RelayCommand]
        async void SignOut()
        {

            Settings.Current.LoggedIn = false;
            await Task.Delay(1);

            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }
    }
}

