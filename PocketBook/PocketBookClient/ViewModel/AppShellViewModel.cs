using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        [ObservableProperty]
        public string loginUserName = string.Empty;


        [RelayCommand]
            async void SignOut()
            {
                //if (Preferences.ContainsKey(nameof(App.UserDetails)))
                //{
                //    Preferences.Remove(nameof(App.UserDetails));
                //}
                await Task.Delay(1);
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            }
        }
}
