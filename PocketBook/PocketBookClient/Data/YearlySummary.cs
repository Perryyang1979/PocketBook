using PocketBookClient.Helper;

namespace PocketBookClient.Data
{
    public class YearlySummary
    {
        public YearlySummary() { }
        public YearlySummary(MonthlySummary monthlySummary)
        {
            this.UserName = monthlySummary.UserName;
            this.TotalIncome = DoubleHelper.Round(monthlySummary.TotalIncome);
            this.TotalExpense = DoubleHelper.Round(monthlySummary.TotalExpense);
        }

        public string UserName { get; set; }

        private double _totalIncome;
        public double TotalIncome {
            get
            {
                return _totalIncome;
            }
            set
            {
                _totalIncome = DoubleHelper.Round(value);
            }
         }

        private double _totalExpense;
        public double TotalExpense {
            get
            {
                return _totalExpense;
            }
            set
            {
                _totalExpense = DoubleHelper.Round(value);
            } 
        }
        public double Balance => DoubleHelper.Round(TotalIncome - TotalExpense);
    }
}
