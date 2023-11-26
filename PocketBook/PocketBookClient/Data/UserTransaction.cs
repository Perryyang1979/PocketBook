using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class UserTransaction
    {
        private const string Income = "收入";
        private const string Expense = "支出";
        public double Amount { get; set; }
        public bool IsIncome { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }

        public string Comments { get; set; }

        public string CurrentInOutLable { 
            get
            {
                return IsIncome ?Income:Expense;
            }
        }
    }
}
