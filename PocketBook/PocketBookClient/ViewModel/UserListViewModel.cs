using PocketBookClient.Data;
using PocketBookClient.Services;
using System.Security.Cryptography;

namespace PocketBookClient.ViewModel
{
    public partial class UserListViewModel:BaseViewModel
    {
         public ObservableCollection<User> UserList { get; set; } = new ObservableCollection<User>();
        [ObservableProperty]
        private User currentSelectedItem = null;
        [ObservableProperty]
        private bool deleteButtonEnable = false;

        public UserListViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            CurrentSelectedItem = null;
            DeleteButtonEnable = false;
            GetCurrentUsers();
        }

        [RelayCommand]
        async Task DeleteUserAsync()
        {
            if (CurrentSelectedItem == null)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert("delete User ", "Are you sure to delete?", accept: "OK", "Cancel");

            if (confirm)
            {
                var result = await Communication.Instance.DeleteUserAsync(CurrentSelectedItem.Name);

                if (result == null)
                {
                    await Shell.Current.DisplayAlert("delete user failed", "unexpected error.", "OK");
                }
                if (!result.Success)
                {
                    await Shell.Current.DisplayAlert("delete user failed", result.Message, "OK");
                }

                GetCurrentUsers();
            }
        }

        private async void GetCurrentUsers()
        {
            UserList.Clear();

            using (var call = Communication.Instance.GetAllUserStream())
            {
                var token = new CancellationToken();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;

                    UserList.Add(new User()
                        {
                            Name = responseInfo.Username,
                            Email = responseInfo.Email,
                            PhoneNumber = responseInfo.PhoneNumber
                        });
                    
                }
            }
            if (UserList.Any())
            {
                CurrentSelectedItem = UserList.FirstOrDefault();
            }
        }

        partial void OnCurrentSelectedItemChanged(User value)
        {
            if (value != null)
            {
                DeleteButtonEnable = true;
            }
        }
    }
}
