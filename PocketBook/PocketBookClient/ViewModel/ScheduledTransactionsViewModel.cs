
using PocketBookClient.Data;
using PocketBookClient.Helper;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class ScheduledTransactionsViewModel : BaseViewModel
    {
        public ObservableCollection<ScheduledTransaction> Transactions { get; set; } = new ObservableCollection<ScheduledTransaction>();
        public ObservableCollection<DayOfWeek> DaysOfWeek { get; set; } = new ObservableCollection<DayOfWeek>(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>());

        private readonly IScheduleService scheduleService;
        //  private const string defaultGuid = "fe387da9-c3f9-44f0-8c8c-36a7e287254c";
        private readonly ScheduledTransaction defaultTransaction = new ScheduledTransaction()
        {
            Id = "fe387da9-c3f9-44f0-8c8c-36a7e287254c",
            Amount = 0,
            Frequency = FrequencyEnum.Monthly,
            Category = string.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(12),
            DayOfMonth = 1,
        };
            

        public ScheduledTransactionsViewModel()
        {
            scheduleService = ServiceHelper.GetService<IScheduleService>();
            if (scheduleService != null)
            {
                // Shell.Current.DisplayAlert("shedule check", "shedule checked today!", "OK");
                // scheduleService.ExecuteSheduleTransaction();
                IsScheduleCommandVisible = scheduleService.ExistTaskToBeExecute;
            }
        }

        [ObservableProperty]
        private ObservableCollection<string> categories = new ObservableCollection<string>();

        [ObservableProperty]
        bool addButtonEnable;

        [ObservableProperty]
        bool editButtonEnable;

        [ObservableProperty]
        bool deleteButtonEnable;
        [ObservableProperty]
        private ScheduledTransaction selectedTransaction;

        [ObservableProperty]
        bool isTransactionSelected;

        [ObservableProperty]
        bool isScheduleCommandVisible;

        [ObservableProperty]
        bool isRunning;

        #region private
        private void Init()
        {
            IsTransactionSelected = SelectedTransaction != null;
            AddButtonEnable = true;
            EditButtonEnable= false;
            DeleteButtonEnable = false;
            IsRunning = false;
            if (scheduleService != null)
            {
                IsScheduleCommandVisible = scheduleService.ExistTaskToBeExecute;
            }
        }

        private async void GetAllScheduledTransaction()
        {
            if (IsRunning)
            {
                return;
            }
            IsRunning = true;
            Transactions.Clear();

            var scheduleResult = await Communication.Instance.GetAllSheduledTransactions();
            foreach (var schedule in scheduleResult)
            {
                Transactions.Add(schedule);
            }

            IsRunning = false;
        }

        private async void GetCurrentCategories()
        {
            if(IsRunning)
            {
                return;
            }

            IsRunning = true;
            Categories.Clear();

            foreach (var category in Communication.Instance.GetCategories().Select(g => g.CategoryDescription))
            {

                Categories.Add(category);
            }

            if (Categories.Count > 0)
                SelectedTransaction.Category = Categories[0];

            IsRunning = false;
        }
        #endregion

        partial void OnSelectedTransactionChanged(ScheduledTransaction value)
        {
            //here use id to identify new added or exist
            if(value.Id == defaultTransaction.Id) 
            {
                AddButtonEnable = true;
                EditButtonEnable = false;
                DeleteButtonEnable= false;
            }
            else
            {
                AddButtonEnable = false;
                EditButtonEnable = true;
                DeleteButtonEnable = true;
            }
        }

        partial void OnSelectedTransactionChanging(ScheduledTransaction value)
        {
            if(IsRunning)
            {
                return;
            }
        }

        public void RefreshModel()
        {
            GetAllScheduledTransaction();
            SelectedTransaction = defaultTransaction.Clone();

            Transactions.Add(SelectedTransaction);
            GetCurrentCategories();
            Init();
        }

        [RelayCommand]
        async Task AddTransactionAsync()
        {
            if(IsRunning)
            {
                return;
            }

            IsRunning = true;
            if(!Categories.Contains(SelectedTransaction.Category))
            {
                await Shell.Current.DisplayAlert("Add schedule transaction failed ", "Category need be specified ", "OK");
                return;
            }

            if(Transactions.Any(t=>t.Category == SelectedTransaction.Category && t.Amount == SelectedTransaction.Amount 
            && t.Frequency == SelectedTransaction.Frequency && t.Id!=SelectedTransaction.Id ) )
            {
                await Shell.Current.DisplayAlert("Add schedule transaction failed ", "Current Scheduale transcation seems exist", "OK");
                return;
            }


            SelectedTransaction.Id = Guid.NewGuid().ToString();

            var result = await Communication.Instance.AddSheduledTransactionAsync(SelectedTransaction);

            if(result.Success)
            {
                SelectedTransaction = defaultTransaction.Clone();
                Transactions.Add(SelectedTransaction);
            }
            else
            {
                SelectedTransaction.Id = defaultTransaction.Id;
                await Shell.Current.DisplayAlert("Add schedule transaction failed ", "unexpected error", "OK");
            }

            IsRunning = false;
        }

        [RelayCommand]
        async Task EditTransactionAsync()
        {
            if(IsRunning)
            {
                return;
            }

            IsRunning = true;
            if(SelectedTransaction!=null)
            {
                var result = await Communication.Instance.UpdateSheduledTransactionAsync(SelectedTransaction);

                if (result.Success)
                {
                    SelectedTransaction = Transactions.SingleOrDefault(t => t.Id == defaultTransaction.Id);
           
                }
                else
                {
                    await Shell.Current.DisplayAlert("Edit schedule transaction failed ", "unexpected error", "OK");
                }
            }
            IsRunning = false;
        }

        [RelayCommand]
        async Task DeleteTransactionAsync()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            if (SelectedTransaction != null)
            {

                var result = await Communication.Instance.DeleteScheduledTransactionAsync(SelectedTransaction);

                if (result.Success)
                {
                    Transactions.Remove(SelectedTransaction);
                    SelectedTransaction = Transactions.SingleOrDefault(t => t.Id == defaultTransaction.Id);
               

                }
                else
                {
                    await Shell.Current.DisplayAlert("delete schedule transaction failed ", "unexpected error", "OK");
                }
            }
            IsRunning = false;
        }

        [RelayCommand]
        async Task ExecuteTransactionAsync()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            if(scheduleService!= null)
            {
                 var result = await scheduleService.ExecuteSheduleTransaction();

                if(!result)
                {
                    await Shell.Current.DisplayAlert("schedule transaction failed ", "unexpected error", "OK");
                }
            }

            IsRunning = false;
        }
    }
}
