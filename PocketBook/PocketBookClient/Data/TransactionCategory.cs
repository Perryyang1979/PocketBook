using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class TransactionCategory
    {
        public bool IsIncome { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryDetailedDescription { get; set; }
    }
}
