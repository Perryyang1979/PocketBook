

using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    [QueryProperty(nameof(PasswordOperation), "PasswordOperation")]
    public partial class PasswordManageViewModel:BaseViewModel
    {
        public PasswordManageViewModel() {
            Initialize();
        }

        private void Initialize()
        {
            PasswordOperation = -1;
            ResetPanelVisible = false;
            ChangePanelVisible = false;
            IsConfirmOK = false;
            IsConfirmNotOK = false;
        }

        [ObservableProperty]
        int passwordOperation = 0;// 0 reset 1 change

        [ObservableProperty]
        public bool resetPanelVisible;

        [ObservableProperty]
        public bool changePanelVisible;
        [ObservableProperty]
        public string email;
        [ObservableProperty]
        public string oldPassword;
        [ObservableProperty]
        public string newPassword;
        [ObservableProperty]
        public string repeatNewPassword;
        [ObservableProperty]
        public string resetResultMessage;
        [ObservableProperty]
        bool isConfirmOK;
        [ObservableProperty]
        bool isConfirmNotOK;

        partial void OnPasswordOperationChanged(int value)
        {
            if(PasswordOperation == 0)
            {
                ResetPanelVisible = true;
                ChangePanelVisible = false;
            }
            else if (PasswordOperation == 1)
            {
                ResetPanelVisible = false;
                ChangePanelVisible = true;
            }
        }

        [RelayCommand]
        async Task ResetPasswordAsync()
        {
           if(string.IsNullOrEmpty(Email))
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert("Reset Password ", "Are you sure to Reset Password?", accept: "OK", "Cancel");

            if (confirm)
            {
                var result = await Communication.Instance.ResetPasswordAsync(Email);

                if (result != null)
                {
                    if (result.Success)
                    {
                        IsConfirmOK = true;
                        IsConfirmNotOK = false;
                    }
                    else
                    {
                        IsConfirmOK = false;
                        IsConfirmNotOK = true;
                    }
                }
                else
                {
                    IsConfirmOK = false;
                    IsConfirmNotOK = true;
                }

                //clear
                Email = string.Empty;
            }
        }

        [RelayCommand]
        async Task ChangePasswordAsync()
        {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(RepeatNewPassword))
            {
                return;
            }

            if(!NewPassword.Equals(RepeatNewPassword)) {
                NewPassword = string.Empty;
                RepeatNewPassword = string.Empty;
                return;
            }

            var result = await Communication.Instance.ChangePasswordAsync(Settings.Current.CurrentUserSetting.UserName, OldPassword, NewPassword);

            if (result != null)
            {
                if (result.Success)
                {
                    IsConfirmOK = true;
                    IsConfirmNotOK = false;
                }
                else
                {
                    IsConfirmOK = false;
                    IsConfirmNotOK = true;
                }
            }
            else
            {
                IsConfirmOK = false;
                IsConfirmNotOK = true;
            }

            //clear
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            RepeatNewPassword = string.Empty;

        }


    }


}
