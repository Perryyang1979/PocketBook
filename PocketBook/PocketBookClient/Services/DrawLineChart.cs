using Microcharts;
using SkiaSharp;
using PocketBookClient.Data;

namespace PocketBookClient.Services
{
   public static class DrawLineChart
    {
        private static SKColor[] StandColors = new SKColor[] { SKColor.Parse("#2c3e50"), SKColor.Parse("#77d065"), SKColor.Parse("#b455b6"), SKColor.Parse("#3498db") };
        public static  List<ChartSerie> GetChartSerialFromMonthlyReport(ObservableCollection<MonthlySummary> monthSummaryList)
        {
            var listEntry = new List<ChartEntry>();
            var listEntry2 = new List<ChartEntry>();

            Dictionary<string,MonthlySummary> monthlySummaryTotal = new Dictionary<string, MonthlySummary>();

            foreach(var monthlySummary in monthSummaryList)
            {
                if(monthlySummaryTotal.ContainsKey(monthlySummary.Date))
                {
                    monthlySummaryTotal[monthlySummary.Date].TotalIncome += monthlySummary.TotalIncome;
                    monthlySummaryTotal[monthlySummary.Date].TotalExpense += monthlySummary.TotalExpense;
                }
                else
                {
                    monthlySummaryTotal.Add(monthlySummary.Date, monthlySummary);
                }
            }


            foreach (var monthsummary in monthlySummaryTotal.Values)
            {
                listEntry.Add(new ChartEntry((float)monthsummary.TotalIncome)
                {
                    Label = monthsummary.Date,
                    ValueLabel = monthsummary.TotalIncome.ToString(),
                    Color = StandColors[0]
                });

                listEntry2.Add(new ChartEntry((float)monthsummary.TotalExpense)
                {
                    Label = monthsummary.Date,
                    ValueLabel = monthsummary.TotalExpense.ToString(),
                    Color = StandColors[1]
                });
            }

            var charSerie1 = new ChartSerie()
            {
                Entries = listEntry,
                Name = "Income",
                Color = StandColors[0],
                OtherColor = StandColors[2]
            };

            var charSerie2 = new ChartSerie()
            {
                Entries = listEntry2,
                Name = "expense",
                Color = StandColors[1],
                OtherColor = StandColors[3]
            };

            return new List<ChartSerie>()
            {
                {charSerie1 },
                {charSerie2 },
            };
        }

        public static List<ChartSerie> GetChartSerialFromMonthlyCategoryReport(ObservableCollection<MonthlyCategorySummary> monthSummaryList,  ObservableCollection<SelectOption> selectedCategorys)
        {
            var listCategorys = new List<ChartSerie>();

            var showLineCount = 0;

            foreach (var selectedCategory in selectedCategorys)
            {
                var listEntry = new List<ChartEntry>();


                foreach (var monthsummary in monthSummaryList.Where(m=>m.Category.Equals( selectedCategory.Name)))
                {
                    listEntry.Add(new ChartEntry((float)(monthsummary.TotalIncome+monthsummary.TotalExpense))
                    {
                        Label = monthsummary.Date,
                        ValueLabel = (monthsummary.TotalIncome + monthsummary.TotalExpense).ToString(),
                        Color = StandColors[showLineCount%4]
                    });               
                }

                if (listEntry.Count > 0)
                {
                    var charSerie1 = new ChartSerie()
                    {
                        Entries = listEntry,
                        Name = selectedCategory.Name,
                        Color = StandColors[showLineCount % 4],
                        OtherColor = StandColors[(showLineCount+1) % 4 ]
                    };

                    showLineCount++;
                    listCategorys.Add(charSerie1);
                }
            }

            return listCategorys;
        }
    }
}
