using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public partial class UserTransaction:ObservableObject
    {
        private const string Income = "收入";
        private const string Expense = "支出";

        [ObservableProperty]
        public double amount;

        [ObservableProperty]
        public bool isIncome;

        [ObservableProperty]
        public string category;

        [ObservableProperty]
        public string date;

        public string Comments { get; set; }

        public string CurrentInOutLable { 
            get
            {
                return IsIncome ?Income:Expense;
            }
        }

        public string UserName { get; set; }

        public string Id { get; set;}
    }
}
