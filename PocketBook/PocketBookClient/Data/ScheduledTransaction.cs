using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public partial class ScheduledTransaction: ObservableObject
    {
        public string Id { get; set; }
        [ObservableProperty]
        public string category;
        [ObservableProperty]
        public double amount;
        [ObservableProperty]
        public FrequencyEnum frequency;
        [ObservableProperty]
        public DateTime startDate;
        [ObservableProperty]
        public DateTime endDate;
        [ObservableProperty]
        public int dayOfMonth;
        [ObservableProperty]
        public DayOfWeek dayOfWeek;

        public bool IsIncome { get; set; }

        partial void OnFrequencyChanged(FrequencyEnum value)
        {
            OnPropertyChanged(nameof(RequireDayOfMonth));
            OnPropertyChanged(nameof(RequireDayOfWeek));
        }

        public bool RequireDayOfMonth => Frequency == FrequencyEnum.Monthly;
        public bool RequireDayOfWeek => Frequency == FrequencyEnum.Weekly;

        public ScheduledTransaction Clone()
        {
            return new ScheduledTransaction()
            {
                Category = this.Category,
                Amount = this.Amount,
                Frequency = this.Frequency,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DayOfMonth = this.DayOfMonth,
                DayOfWeek = this.DayOfWeek,
                Id = this.Id
            };
        }
    }

    public enum FrequencyEnum
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
    }

}
