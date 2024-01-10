using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public class MonthlyCategorySummary
    {
        public string Date { get; set; }
        public string Category { get; set; }
        public double TotalMoney { get; set; }
        public string Identification { get; set; }
    }
}
