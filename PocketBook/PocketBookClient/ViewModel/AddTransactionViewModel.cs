using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class AddTransactionViewModel : BaseViewModel
    {
        public DateTime MinDate { get; private set; }
        public DateTime MaxDate { get; private set; }
        public DateTime SelectedDate { get; set; }

        public ReadOnlyCollection<string> MoneyChangeTypes { get; }

        public AddTransactionViewModel()
        {
            MinDate = DateTime.Today.AddDays(-100);
            MaxDate = DateTime.Today.AddDays(30);
            SelectedDate = DateTime.Today;
              var strTypes = new List<string>() { $"{LocalizationResourceManager["AddTranscation_IsIncome_Income"]}", $"{LocalizationResourceManager["AddTranscation_IsIncome_Expense"]}" };
              MoneyChangeTypes = new ReadOnlyCollection<string>(strTypes);
              GetCategories();
        }

        private void GetCategories()
        {
            Categories = new ObservableCollection<TransactionCategory>();

            if(string.IsNullOrEmpty(CurrentMoneyChangeType))
            {
                return;
            }

            if(CurrentMoneyChangeType == "True")
            {
                Categories.Add(new TransactionCategory() { CategoryDescription = "salary", CategoryDetailedDescription = "recored salary" });
                Categories.Add(new TransactionCategory() { CategoryDescription = "home rent", CategoryDetailedDescription = "recored rent home something" });
            }
            else
            {
                Categories.Add(new TransactionCategory() { CategoryDescription = "insurance", CategoryDetailedDescription = "recored insurance" });
                Categories.Add(new TransactionCategory() { CategoryDescription = "house loan", CategoryDetailedDescription = "house loan everything" });
            }
        }

        [ObservableProperty]
        private ObservableCollection<TransactionCategory> categories;
        [ObservableProperty]
        private string currentMoneyChangeType = string.Empty;
        [ObservableProperty]
        private double money = 0;
        [ObservableProperty]
        private int categorySelectedIndex = -1;
        [ObservableProperty]
        private string comments = string.Empty;

        partial void OnCurrentMoneyChangeTypeChanged(string value)
        {
            GetCategories();
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            var transactin = new UserTransaction()
            {
                Date = SelectedDate.ToShortDateString(),
                Amount = Money,
                IsIncome = CurrentMoneyChangeType.Equals("True"),
                Category = Categories[CategorySelectedIndex].CategoryDescription,
                Comments = Comments,
            };

            await Communication.Instance.AddTransaction(transactin);
        }
    }
}
