using PocketBookClient.Data;
using PocketBookClient.Helper;
using PocketBookClient.Services;
using System.Text.RegularExpressions;

namespace PocketBookClient.ViewModel
{
    public partial class AddTransactionViewModel : BaseViewModel
    {
        public DateTime MinDate { get; private set; }
        public DateTime MaxDate { get; private set; }

        private readonly string IsIncome = "True";

        private readonly string IsExpense = "False";
        private const int RecentlyDaysDefine = 10;

        private readonly ICamera _iOSCamera;
        private readonly IPhotoProcess _photoProcess;

        [ObservableProperty]
        private bool contentVisible;
        [ObservableProperty]
        private bool warningVisible;

        public AddTransactionViewModel()
        {
            if (!CheckValidUser()) return;

            _iOSCamera = ServiceHelper.GetService<ICamera>();
            _photoProcess = ServiceHelper.GetService<IPhotoProcess>();
            Init();
            GetRecentlyRecord();
            GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
            //    GetCurrentIdentification();
        }

        private bool CheckValidUser()
        {
            if (Settings.Current.CurrentUserSetting == null || string.IsNullOrEmpty(Settings.Current.CurrentUserSetting.UserName))
            {
                ContentVisible = false;
                WarningVisible = true;

                return false;
            }
            else
            {
                ContentVisible = true;
                WarningVisible = false;
                return true;
            }
        }

        private void Init()
        {
            MinDate = DateTime.Today.AddDays(-100);
            MaxDate = DateTime.Today.AddDays(30);
            SelectedDate = DateTime.Today;
            CurrentMoneyChangeType = IsExpense;

            //clear filed
            Money = "0";
            Comments = string.Empty;
            if (Categories.Any())
            {
                CategorySelectedIndex = 0;
            }
            else
            {
                CategorySelectedIndex = -1;
            }

            AddButtonVisible = true;
            UpdateButtonVisible = !AddButtonVisible;

            PhotoCameraVisible = false;
            ImageReceive = null;
        }

        private async void GetRecentlyRecord()
        {
            TransactionList.Clear();
            var endDate = DateTime.Today.Date;
            var startDate = endDate.AddDays(0 - RecentlyDaysDefine);
            try

            {
                foreach (var tran in Communication.Instance.GetDailyTransations(startDate, endDate))
                {

                    TransactionList.Add(tran);
                }


                if (TransactionList.Count>0)
                {
                  //  TransactionList.OrderByDescending(t => t.Date);
                    CurrentSelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void RefreshModel()
        {
            if (Settings.Current.CurrentUserSetting == null || string.IsNullOrEmpty(Settings.Current.CurrentUserSetting.UserName))
            {
                return;
            }
            GetRecentlyRecord();
            GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
            PhotoCameraVisible = false;
            ImageReceive = null;
            //clear filed
            Money = "0";
            Comments = string.Empty;
        }


        private bool requestIsSending =false;
        private async void GetCurrentCategories(bool isIncome)
        {
            if(requestIsSending)
            {
                return;
            }

                requestIsSending = true;

                Categories.Clear();

                foreach (var category in Communication.Instance.GetCategories().Where(c => c.IsIncome == isIncome))
                {

                Categories.Add(category);
                }

                requestIsSending = false;
                if (Categories.Any())
                {
                    Categories = new ObservableCollection<TransactionCategory>(Categories.OrderBy(c => c.CategoryDescription));
                    CategorySelectedIndex = 0;
                }
        }

        [ObservableProperty]
        public ObservableCollection<UserTransaction> transactionList  = new ObservableCollection<UserTransaction>();

        [ObservableProperty]
        public DateTime selectedDate;
        [ObservableProperty]
        private ObservableCollection<TransactionCategory> categories =new ObservableCollection<TransactionCategory>();
        [ObservableProperty]
        private string currentMoneyChangeType = string.Empty;
        [ObservableProperty]
        private string money = "0";
        [ObservableProperty]
        private int categorySelectedIndex = -1;
        [ObservableProperty]
        private string comments = string.Empty;

        [ObservableProperty]
        public int identificationSelectedIndex = -1;
        [ObservableProperty]
        private UserTransaction currentSelectedItem = null;
        [ObservableProperty]
        private bool addButtonVisible = true;
        [ObservableProperty]
        private bool updateButtonVisible = false;
        [ObservableProperty]
        private ImageSource imageReceive = null;
        [ObservableProperty]
        private bool photoCameraVisible = false;

        partial void OnCurrentMoneyChangeTypeChanged(string value)
        {
            if (!string.IsNullOrEmpty(CurrentMoneyChangeType))
            {
                GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
            }
        }

        partial void OnMoneyChanged(string oldValue, string newValue)
        {
            bool isValid = Regex.IsMatch(newValue, @"^[-+]?\d+\.?\d*$"); 

            if (isValid)
            {
                Money = newValue;
            }
            else
            {
                Money = string.Empty;
            }
        }

        partial void OnCurrentSelectedItemChanged(UserTransaction value)
        {

            CurrentSelectedItem = value;
            //update field.
           // throw new NotImplementedException();
           if(CurrentSelectedItem == null)
            {
                Init();
                return;
            }
            //set value
            Money = value.Amount.ToString();
            SelectedDate = Convert.ToDateTime(value.Date);
            Comments = value.Comments;
            CurrentMoneyChangeType = value.IsIncome? IsIncome : IsExpense;
            GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));

            if (Categories.Count > 0) {
                CategorySelectedIndex = Categories.IndexOf(Categories.SingleOrDefault(c => c.CategoryDescription.Equals(value.Category)));
            }

            AddButtonVisible = false;
            UpdateButtonVisible = !AddButtonVisible;
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            if (CategorySelectedIndex < 0)
            {
                await Shell.Current.DisplayAlert("Add Record Fail ", "Category need be specified ", "OK");
                return;
            }

            var convertMoney = -1.0;
            double.TryParse(Money,out convertMoney);

            if (convertMoney <= 0.0)
            {
                await Shell.Current.DisplayAlert("Add Record Fail ", "Money Input should be above 0", "OK");
                return;
            }

            PhotoCameraVisible = false;
            ImageReceive = null;

            var transaction = new UserTransaction()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Settings.Current.CurrentUserSetting.UserName,
                Date = SelectedDate.ToShortDateString(),
                Amount = convertMoney,
                IsIncome = CurrentMoneyChangeType.Equals(IsIncome),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                Comments = Comments,
            };

            var response = await Communication.Instance.AddTransactionAsync(transaction);

            if (response != null && response.Success)
            {
                Init();
                GetRecentlyRecord();
            }
            else
            {
                await Shell.Current.DisplayAlert("Add record failed", response?.Message, "OK");
                return;
            }
        }

        [RelayCommand]
        async Task UpdateTransactionAsync()
        {

            if (CategorySelectedIndex < 0)
            {
                await Shell.Current.DisplayAlert("Update Record Fail ", "Category need be specified ", "OK");
                return;
            }

            var convertMoney = -1.0;
            double.TryParse(Money, out convertMoney);

            if(convertMoney < 0.0) {
                await Shell.Current.DisplayAlert("Update Record Fail ", "Money Input is wrong", "OK");
                return;
            }

            var transaction = new UserTransaction()
            {
                UserName = Settings.Current.CurrentUserSetting.UserName,
                Date = SelectedDate.ToShortDateString(),
                Amount = convertMoney,
                IsIncome = CurrentMoneyChangeType.Equals(IsIncome),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                Comments = Comments,
                Id = CurrentSelectedItem.Id
            };

            Communication.MessageResponse response = null;

            if(convertMoney == 0)
            {
                response = await Communication.Instance.DeleteUserTransactionAsync(transaction);
            }

            else response = await Communication.Instance.UpdateTransactionAsync(transaction);

            if (response != null && response.Success)
            {
                Init();
                GetRecentlyRecord();
            }
            else
            {
                await Shell.Current.DisplayAlert("update record failed", response?.Message, "OK");
                return;
            }
        }

        [RelayCommand]
        async Task CancelTransactionAsync()
        {
            CurrentSelectedItem = null;
            Init();
            //await Shell.Current.GoToAsync(nameof(PocketOperationPage));
        }

        [RelayCommand]
        async Task AddCategoryAsync()
        {
            await Shell.Current.GoToAsync(nameof(CategoryListPage), true, new Dictionary<string, object>
        {
            {"CurrentCategoryType", CurrentMoneyChangeType}
        });
        }


        void LoadImageFromStreamAsync(Stream stream)
         {
               ImageReceive = ImageSource.FromStream(() => stream);
         }

            [RelayCommand]
        async Task CameraPhotoTransactionAsync()
        {
            FileResult photo = null;

#if ANDROID
            photo =  await MediaPicker.CapturePhotoAsync();
#elif IOS
           if(_iOSCamera != null)
                    photo =  await _iOSCamera.CapturePhotoAsync();
#endif


            if (photo != null)
                {
                    PhotoCameraVisible = true;
                    var stream = await photo.OpenReadAsync();
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);                 
                
                using (var newStream = File.OpenWrite(filePath))
                {
                    await stream.CopyToAsync(newStream);
                }

                ImageReceive = ImageSource.FromFile(filePath);

                //var photoProcessHelper = new PhotoProcessHelper();
                //var transaction = await photoProcessHelper.ProcessReceipt(filePath);

                var photoTransactionService = new PhotoProcessService(_photoProcess);
                var transaction = await photoTransactionService.HandleProcess(filePath);

                CurrentMoneyChangeType = IsExpense;
                //clear filed
                Money = transaction.Amount.ToString();

                if (Categories.Count > 0 && transaction.IdentifiedBrand != string.Empty) {

                    var category = Categories.FirstOrDefault(c=>c.CategoryDescription.Equals(transaction.IdentifiedBrand));
                    CategorySelectedIndex = Categories.IndexOf(category);
                }      
                else 
                    CategorySelectedIndex = -1;
            }
        }
    }
}
