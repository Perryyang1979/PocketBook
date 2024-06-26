using PocketBookClient.Data;
using System;


namespace PocketBookClient.Services
{
    public class ScheduleTransactionService:IScheduleService
    {
        public ScheduleTransactionService() { }

        public async Task ExecuteSheduleTransaction()
        {
            var scheduleTaskList = await Communication.Instance.GetAllSheduledTransactions();

            foreach (var schedule in scheduleTaskList.Where(s=>CanBeExecute(s)))
            {

                var transaction = new UserTransaction()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Settings.Current.CurrentUserSetting.UserName,
                    Date = DateTime.Now.ToShortDateString(),
                    Amount = schedule.Amount,
                    IsIncome = schedule.IsIncome,
                    Category =schedule.Category,
                    Comments = "schedule task",
                };

                await Communication.Instance.AddTransactionAsync(transaction);
            }
        }


        private bool CanBeExecute(ScheduledTransaction transaction)
        {
            switch(transaction.Frequency)
            {
                case FrequencyEnum.Daily: return true;
                case FrequencyEnum.Weekly: return DateTime.Today.DayOfWeek == transaction.DayOfWeek;
                case FrequencyEnum.Monthly: return DateTime.Today.Day == transaction.DayOfMonth;
                default: return false;

            }
        }
    }
}
