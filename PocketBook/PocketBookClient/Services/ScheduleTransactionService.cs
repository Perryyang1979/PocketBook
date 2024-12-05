using Microsoft.VisualBasic;
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


        //private bool CanBeExecute(ScheduledTransaction transaction)
        //{
        //    if(DateTime.Today < transaction.StartDate || DateTime.Today> transaction.EndDate)
        //    {
        //        return false;
        //    }

        //    switch(transaction.Frequency)
        //    {
        //        case FrequencyEnum.Daily: return true;
        //        case FrequencyEnum.Weekly: return DateTime.Today.DayOfWeek == transaction.DayOfWeek;
        //        case FrequencyEnum.Monthly: return DateTime.Today.Day == transaction.DayOfMonth;
        //        default: return false;

        //    }
        //}

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
                    return  today.Day == transaction.DayOfMonth;

                default:
                    return false;
            }
        }
    }
}
