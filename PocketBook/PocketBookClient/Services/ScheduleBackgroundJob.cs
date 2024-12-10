//using Shiny.Jobs;
using PocketBookClient.Data;

namespace PocketBookClient.Services
{
    public class ScheduleBackgroundJob 
    {
        public async Task Run(object jobInfo, CancellationToken cancelToken)
        {
            // 在这里实现你的任务逻辑
            Console.WriteLine("Running background job...");

            // 模拟一个耗时任务
            await ExecuteSheduleTransaction();

            // 记录完成时间或进行数据处理
            Console.WriteLine("Background job completed.");
        }

        private async Task ExecuteSheduleTransaction()
        {
            var scheduleTaskList = await Communication.Instance.GetAllSheduledTransactions();

            foreach (var schedule in scheduleTaskList.Where(s => CanBeExecute(s)))
            {

                var transaction = new UserTransaction()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Settings.Current.CurrentUserSetting.UserName,
                    Date = DateTime.Now.ToShortDateString(),
                    Amount = schedule.Amount,
                    IsIncome = schedule.IsIncome,
                    Category = schedule.Category,
                    Comments = "schedule task",
                };

                await Communication.Instance.AddTransactionAsync(transaction);
            }
        }

        private bool CanBeExecute(ScheduledTransaction transaction)
        {
            if (transaction == null) return false;

            var today = DateTime.Today;
            if (today < transaction.StartDate.Date || today > transaction.EndDate.Date)
            {
                return false;
            }

            switch (transaction.Frequency)
            {
                case FrequencyEnum.Daily:
                    return true;

                case FrequencyEnum.Weekly:
                    return today.DayOfWeek == transaction.DayOfWeek;

                case FrequencyEnum.Monthly:
                    return today.Day == transaction.DayOfMonth;

                default:
                    return false;
            }
        }


    }
}
