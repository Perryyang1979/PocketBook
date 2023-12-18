
using PocketBookClient.Services;


namespace PocketBookClient.ViewModel
{
    public partial class UserInfoPageViewModel:BaseViewModel
    {
        public UserInfoPageViewModel() { }

        [ObservableProperty]
        string? userName  = Settings.Current.CurrentUserSetting==null?string.Empty:Settings.Current.CurrentUserSetting.UserName;
        [ObservableProperty]
        string? userGroupId = Settings.Current.CurrentUserSetting == null ? string.Empty:Settings.Current.CurrentUserSetting.UserGroupId;
        [ObservableProperty]
        bool panelVisble = Settings.Current.CurrentUserSetting!=null && !string.IsNullOrEmpty(Settings.Current.CurrentUserSetting.UserName);

        [RelayCommand]
        public async Task ChangePasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(PasswordManagePage), true, new Dictionary<string, object>
        {
            {"PasswordOperation", 1}
        });
        }
    }
}
