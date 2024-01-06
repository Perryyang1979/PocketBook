using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class AddTransactionViewModel : BaseViewModel
    {
        public DateTime MinDate { get; private set; }
        public DateTime MaxDate { get; private set; }

        private readonly string IsIncome = "True";

        private readonly string IsExpense = "False";
        public ObservableCollection<UserTransaction> TransactionList { get; } = new ObservableCollection<UserTransaction>();
        private const int RecentlyDaysDefine = 10;

        [ObservableProperty]
        private bool contentVisible;
        [ObservableProperty]
        private bool warningVisible;

        public AddTransactionViewModel()
        {
            if (!CheckValidUser()) return;
            Init();
         //   GetRecentlyRecord();
         //   GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
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
            Money = 0;
            Comments = string.Empty;
            CategorySelectedIndex = -1;
            IdentificationSelectedIndex = -1;

            AddButtonVisible = true;
            UpdateButtonVisible = !AddButtonVisible;
        }

        private async void GetRecentlyRecord()
        {
            TransactionList.Clear();

            var endDate = DateTime.Today.Date;

            var startDate = endDate.AddDays(0 - RecentlyDaysDefine);

            try
            {
                using (var call = Communication.Instance.GetDailyTransationsStreamAsync(startDate, endDate))
                {
                    var token = new CancellationToken();
                    while (await call.ResponseStream.MoveNext(token))
                    {
                        var responseInfo = call.ResponseStream.Current;
                        TransactionList.Add(new UserTransaction()
                        {
                            Date = responseInfo.TransactionDate,
                            Category = responseInfo.Category,
                            IsIncome = responseInfo.IsIncome,
                            Comments = responseInfo.Comments,
                            Amount = responseInfo.Amount,
                            UserName = responseInfo.Username,
                            IdentifyDescription = responseInfo.Identification,
                            Id = responseInfo.Id,
                        });
                    }
                }

                if(TransactionList.Count>0)
                {
                    TransactionList.OrderByDescending(t => t.Date).ToList();
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
            GetCurrentIdentification();
        }


        private bool requestIsSending =false;
        private async void GetCurrentCategories(bool isIncome)
        {
            if(requestIsSending)
            {
                return;
            }
            using (var call = Communication.Instance.GetAllCategoryStream())
            {
                    requestIsSending = true;
                    CancellationToken token = new CancellationToken();
                    Categories.Clear();
                    while (await call.ResponseStream.MoveNext(token))
                    {
                            var responseInfo = call.ResponseStream.Current;

                            if (responseInfo.IsIncome == isIncome)
                            {
                                Categories.Add(new TransactionCategory()
                                {
                                    IsIncome = responseInfo.IsIncome,
                                    CategoryDescription = responseInfo.Description,
                                    CategoryDetailedDescription = responseInfo.Detail
                                });
                            }
                     }
                requestIsSending = false;
                if(Categories.Any())
                {
                    Categories =new  ObservableCollection<TransactionCategory>( Categories.OrderBy(c=>c.CategoryDescription));
                    CategorySelectedIndex = 0;
                }
            }
        }

        private bool identificationrequestIsSending = false;
        private async void GetCurrentIdentification()
        {
            if (identificationrequestIsSending)
            {
                return;
            }
            using (var call = Communication.Instance.GetAllIdentificationStream())
            {
                requestIsSending = true;
                CancellationToken token = new CancellationToken();
                Identifications.Clear();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;


                    Identifications.Add(new TransactionIdentification()
                    {
                            IdentifyDescription = responseInfo.Description,
                    });
                    
                }
                identificationrequestIsSending = false;
                if (Identifications.Any())
                {
                    Identifications = new ObservableCollection<TransactionIdentification>(Identifications.OrderByDescending(c => c.IdentifyDescription));
                    IdentificationSelectedIndex = 0;
                }
            }
        }
        [ObservableProperty]
        public DateTime selectedDate;
        [ObservableProperty]
        private ObservableCollection<TransactionCategory> categories =new ObservableCollection<TransactionCategory>();
        [ObservableProperty]
        private string currentMoneyChangeType = string.Empty;
        [ObservableProperty]
        private double money = 0;
        [ObservableProperty]
        private int categorySelectedIndex = -1;
        [ObservableProperty]
        private string comments = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TransactionIdentification> identifications = new ObservableCollection<TransactionIdentification>();

        [ObservableProperty]
        public int identificationSelectedIndex = -1;
        [ObservableProperty]
        private UserTransaction currentSelectedItem = null;
        [ObservableProperty]
        private bool addButtonVisible = true;
        [ObservableProperty]
        private bool updateButtonVisible = false;

        partial void OnCurrentMoneyChangeTypeChanged(string value)
        {
            if (!string.IsNullOrEmpty(CurrentMoneyChangeType))
            {
                GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
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
            Money = value.Amount;
            SelectedDate = Convert.ToDateTime(value.Date);
            Comments = value.Comments;
            CurrentMoneyChangeType = value.IsIncome? IsIncome : IsExpense;
            if (Categories.Count > 0) {
                CategorySelectedIndex = Categories.IndexOf(Categories.SingleOrDefault(c => c.CategoryDescription.Equals(value.Category)));
            }
            if (Identifications.Count > 0)
            {
                IdentificationSelectedIndex = Identifications.IndexOf(Identifications.SingleOrDefault(i => i.IdentifyDescription.Equals(value.IdentifyDescription)));
            }


            AddButtonVisible = false;
            UpdateButtonVisible = !AddButtonVisible;
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            if (IdentificationSelectedIndex < 0)
            {
                await Shell.Current.DisplayAlert("Add Record Fail ", "Identification need be specified ", "OK");
                return;
            }

            if (CategorySelectedIndex < 0)
            {
                await Shell.Current.DisplayAlert("Add Record Fail ", "Category need be specified ", "OK");
                return;
            }

            var transaction = new UserTransaction()
            {
                Date = SelectedDate.ToShortDateString(),
                Amount = Money,
                IsIncome = CurrentMoneyChangeType.Equals(IsIncome),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                IdentifyDescription =IdentificationSelectedIndex>-1? Identifications[IdentificationSelectedIndex].IdentifyDescription:string.Empty,
                Comments = Comments,
            };

           var response =  await Communication.Instance.AddTransactionAsync(transaction);

            if(response!=null && response.Success)
            {
                Init();
                GetRecentlyRecord();
            }
            else
            {
                await Shell.Current.DisplayAlert("Add record failed", response.Message, "OK");
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

            var transaction = new UserTransaction()
            {
                Date = SelectedDate.ToShortDateString(),
                Amount = Money,
                IsIncome = CurrentMoneyChangeType.Equals(IsIncome),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                IdentifyDescription = IdentificationSelectedIndex > -1 ? Identifications[IdentificationSelectedIndex].IdentifyDescription : string.Empty,
                Comments = Comments,
                Id = CurrentSelectedItem.Id
            };

            var response = await Communication.Instance.UpdateTransactionAsync(transaction);

            if (response != null && response.Success)
            {
                Init();
                GetRecentlyRecord();
            }
            else
            {
                await Shell.Current.DisplayAlert("update record failed", response.Message, "OK");
                return;
            }
        }

        [RelayCommand]
        async Task CancelTransactionAsync()
        {
            CurrentSelectedItem = null;
           // await Shell.Current.GoToAsync(nameof(PocketOperationPage));
        }

        [RelayCommand]
        async Task AddCategoryAsync()
        {
            await Shell.Current.GoToAsync(nameof(CategoryListPage), true, new Dictionary<string, object>
        {
            {"CurrentCategoryType", CurrentMoneyChangeType}
        });
        }

        [RelayCommand]
        async Task AddIdentifyAsync()
        {
            await Shell.Current.GoToAsync(nameof(IdentifyListPage));
        }


    }
}
