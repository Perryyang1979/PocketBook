using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBookClient.Data;
using PocketBookClient.Services;

namespace PocketBookClient.ViewModel
{
    public partial class PocketOperationViewModel:BaseViewModel
    {
        public ObservableCollection<UserTransaction> TransactionList { get; } = new ObservableCollection<UserTransaction>();
        public ObservableCollection<UserTransaction> MonthTransactionList { get; } = new ObservableCollection<UserTransaction>();

        public ObservableCollection<MonthlySummary> MonthSummaryList { get; } = new ObservableCollection<MonthlySummary>();
        private const int RecentlyDaysDefine = 3;

        public PocketOperationViewModel() {
            GetRecentlyRecord();
            GetMonthSummary();
        }

        private async void GetRecentlyRecord()
        {
            TransactionList.Clear();

            var endDate = DateTime.Today.Date;

            var startDate = endDate.AddDays(0-RecentlyDaysDefine);

            using(var call = Communication.Instance.GetDailyTransationsStreamAsync(startDate, endDate))
            {
                var token = new CancellationToken();
                while(await call.ResponseStream.MoveNext(token))
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
                        IdentifyDescription = responseInfo.Identification
                    });
                }
            }
        }

        private async void GetCurrentMonthRecord()
        {
            MonthTransactionList.Clear();

            using (var call = Communication.Instance.GetDailyTransationsInCurrentMonthAsync())
            {
                var token = new CancellationToken();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;
                    MonthTransactionList.Add(new UserTransaction()
                    {
                        Date = responseInfo.TransactionDate,
                        Category = responseInfo.Category,
                        IsIncome = responseInfo.IsIncome,
                        Comments = responseInfo.Comments,
                        Amount = responseInfo.Amount,
                        UserName = responseInfo.Username,
                        IdentifyDescription = responseInfo.Identification
                    });
                }
            }
        }

        private async void GetMonthSummary()
        {
            MonthSummaryList.Clear();

            using (var call = Communication.Instance.GetMonthlyReport())
            {
                var token = new CancellationToken();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;
                    MonthSummaryList.Add(new MonthlySummary()
                    {
                        Date = responseInfo.Date,
                        TotalIncome = responseInfo.TotalIncome,
                        TotalExpense = responseInfo.TotalExpense
                      
                    });
                }
            }
        }

        [RelayCommand]
        async Task PocketRecordAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
        }

        [RelayCommand]
        async Task PocketMonthRecordAsync()
        {
             GetCurrentMonthRecord();
        }
    }
}
