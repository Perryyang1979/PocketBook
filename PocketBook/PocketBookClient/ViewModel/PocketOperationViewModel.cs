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
        private const int RecentlyDaysDefine = 3;

        public PocketOperationViewModel() {
            GetRecentlyRecord();
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
                    });
                }
            }
        }

        [RelayCommand]
        async Task PocketRecordAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
        }
    }
}
