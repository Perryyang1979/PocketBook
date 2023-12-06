using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class AddTransactionViewModel : BaseViewModel
    {
        public DateTime MinDate { get; private set; }
        public DateTime MaxDate { get; private set; }
        public DateTime SelectedDate { get; set; }


        private readonly string IsIncome = "True";

        public AddTransactionViewModel()
        {
            MinDate = DateTime.Today.AddDays(-100);
            MaxDate = DateTime.Today.AddDays(30);
            SelectedDate = DateTime.Today;
            CurrentMoneyChangeType = IsIncome;
        }

        public async void RefreshModel()
        {
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
            }
        }

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
        private int identificationSelectedIndex = -1;

        partial void OnCurrentMoneyChangeTypeChanged(string value)
        {
            if (!string.IsNullOrEmpty(CurrentMoneyChangeType))
            {
                GetCurrentCategories(CurrentMoneyChangeType.Equals(IsIncome));
            }
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            var transaction = new UserTransaction()
            {
                Date = SelectedDate.ToShortDateString(),
                Amount = Money,
                IsIncome = CurrentMoneyChangeType.Equals(IsIncome),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                Comments = Comments,
            };

           var response =  await Communication.Instance.AddTransactionAsync(transaction);

            if(response!=null && response.Success)
            {
                await Shell.Current.GoToAsync(nameof(PocketOperationPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("Add record failed", response.Message, "OK");
                return;
            }
        }

        [RelayCommand]
        async Task CancelTransactionAsync()
        {
            await Shell.Current.GoToAsync(nameof(PocketOperationPage));
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
