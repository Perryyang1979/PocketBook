using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class MonthlySummary
    {
        public string Date { get; set; }
        public string UserName { get; set; }

        public string Identify { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpense { get; set; }
    }
}
