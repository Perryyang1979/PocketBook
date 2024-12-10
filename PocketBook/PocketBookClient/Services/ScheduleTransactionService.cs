using PocketBookClient.Data;
using static PocketBookClient.Services.Communication;


namespace PocketBookClient.Services
{
    public class ScheduleTransactionService:IScheduleService
    {
        public ScheduleTransactionService() { }

        private readonly string ScheduleComments = "schedule task";

        public async Task<bool> ExecuteSheduleTransaction()
        {
            List<ScheduledTransaction> scheduleTaskList = await Communication.Instance.GetAllSheduledTransactions();

            bool execute = true;

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
                    Comments = ScheduleComments,
                };

               var result = await Communication.Instance.AddTransactionAsync(transaction);

                if(!result.Success)
                {
                    execute = false;
                }
            }

            return execute;
        }

        public bool ExistTaskToBeExecute
        {
            get
            {
                List<ScheduledTransaction> scheduleTaskList = Communication.Instance.GetAllSheduledTransactions().Result;

                var canbeExecuteTransctions = scheduleTaskList.Where(s => CanBeExecute(s));
                if ( !canbeExecuteTransctions.Any())
                {
                    return false;
                }

                foreach(var transaction in canbeExecuteTransctions)
                {

                      var record = Communication.Instance.GetCurrentDailyTransations().
                            Where(t=>t.Amount == transaction.Amount && t.Category == transaction.Category && t.Comments == ScheduleComments);
                      return !(record.Any());
                }

                return true;
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
                    return  today.Day == transaction.DayOfMonth;

                default:
                    return false;
            }
        }
    }
}
