using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class UserTransaction
    {
        public double Amount { get; set; }
        public bool IsIncome { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
    }
}
