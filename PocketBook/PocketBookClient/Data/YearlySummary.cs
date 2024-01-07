using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class YearlySummary
    {
        public YearlySummary() { }
        public YearlySummary(MonthlySummary monthlySummary)
        {
            this.UserName = monthlySummary.UserName;
            this.TotalIncome = monthlySummary.TotalIncome;
            this.TotalExpense = monthlySummary.TotalExpense;
        }

        public string UserName { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpense { get; set; }
        public double Balance => TotalIncome - TotalExpense;
    }
}
