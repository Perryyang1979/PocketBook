using Microcharts;
using Microsoft.Maui.Layouts;
using PocketBookClient.Data;
using PocketBookClient.Services;
using PocketBookClient.UI;
using SkiaSharp;
using PocketBookClient.Helper;

namespace PocketBookClient.ViewModel
{
    public partial class AdvancedReportViewModel:BaseViewModel
    {
        public ObservableCollection<UserTransaction> MonthTransactionList { get; } = new ObservableCollection<UserTransaction>();

        public ObservableCollection<MonthlySummary> MonthSummaryList { get; } = new ObservableCollection<MonthlySummary>();

        public ObservableCollection<MonthlyCategorySummary> MonthCategorySummaryList { get; } = new ObservableCollection<MonthlyCategorySummary>();
        public ObservableCollection<SelectOption> SelectedCurrentCategorys { get; set; } = new ObservableCollection<SelectOption>();

        [ObservableProperty]
        private Microcharts.Chart incomeChart;
        [ObservableProperty]
        private Microcharts.Chart expenseChart;
        [ObservableProperty]
        private Microcharts.Chart categoryChart;
        [ObservableProperty]
        public bool monthlyReprotPanelVisible = false;
        [ObservableProperty]
        public bool monthlyCategoryReprotPanelVisible = false;
        [ObservableProperty]
        public bool reportSwtichToggled = false;
        [ObservableProperty]
        public bool categoryReportSwtichToggled = false;
        [ObservableProperty]
        bool monthlyChartSwtichToggled = false;
        [ObservableProperty]
        public ObservableCollection<SelectOption> currrentIdentifications =new ObservableCollection<SelectOption>();
        [ObservableProperty]
        public ObservableCollection<SelectOption> selectedIdentifications = new ObservableCollection<SelectOption>();
        [ObservableProperty]
        public ObservableCollection<SelectOption> currentCategorys = new ObservableCollection<SelectOption>();
        [ObservableProperty]
        bool categoryChartVisble = false;
        [ObservableProperty]
        bool monthSummaryIncomeChartVisible = false;
        [ObservableProperty]
        bool monthSummaryExpenseChartVisible = false;
        [ObservableProperty]
        bool monthlyChartEnabled = false;
        [ObservableProperty]
        bool monthlyChartPanelVisible = false;

        private const int ThreholdCount = 4;

        public AdvancedReportViewModel()
        {
            Init();
            GetMonthSummary();
        }

        private void Init()
        {
            GetCurrentMonthRecord();
            MonthlyCategoryReprotPanelVisible = false;
            ReportSwtichToggled = false;
            CategoryReportSwtichToggled = false;
            MonthlyChartSwtichToggled = false;
            CategoryChartVisble = false;
            MonthlyChartEnabled = false;
            MonthSummaryIncomeChartVisible = false;
            MonthSummaryExpenseChartVisible = false;
            MonthlyChartPanelVisible = MonthSummaryIncomeChartVisible && MonthSummaryExpenseChartVisible;
            SelectedCurrentCategorys.CollectionChanged += SelectedCurrentCategorys_CollectionChanged;
        }


        private void SelectedCurrentCategorys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedCurrentCategorys.Count > 0)
            {
                CategoryChartVisble = true;
                LoadMonthCategoryChart();
            }
            else
            {
                CategoryChart = null;
                CategoryChartVisble = false;
            }
        }

        private async void GetCurrentMonthRecord()
        {
            MonthTransactionList.Clear();
            try
            {
                using (var call = Communication.Instance.GetDailyTransationsInCurrentMonthAsync())
                {
                    var token = new CancellationToken();
                    while (await call.ResponseStream.MoveNext(token))
                    {
                        var responseInfo = call.ResponseStream.Current;
                        MonthTransactionList.Add(new UserTransaction()
                        {
                            Date = responseInfo.TransactionDate,
                            Category = responseInfo.Category,
                            IsIncome = responseInfo.IsIncome,
                            Comments = responseInfo.Comments,
                            Amount = DoubleHelper.Round(responseInfo.Amount),
                            UserName = responseInfo.Username,
                            IdentifyDescription = responseInfo.Identification
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void GetMonthSummary()
        {
            MonthSummaryList.Clear();
            try
            {
                using (var call = Communication.Instance.GetMonthlyReport())
                {
                    var token = new CancellationToken();
                    while (await call.ResponseStream.MoveNext(token))
                    {
                        var responseInfo = call.ResponseStream.Current;
                        MonthSummaryList.Add(new MonthlySummary()
                        {
                            Date = responseInfo.Date,
                            TotalIncome = DoubleHelper.Round( responseInfo.TotalIncome),
                            TotalExpense = DoubleHelper.Round(responseInfo.TotalExpense),
                            Identify = responseInfo.Identify,
                            UserName = responseInfo.UserName
                        });
                    }
                }
                if (MonthSummaryList.Count >= ThreholdCount)
                {
                    MonthlyChartEnabled = true;
                }
                else
                {
                    MonthlyChartEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void GetMonthCategorySummary()
        {
            MonthCategorySummaryList.Clear();
            CurrentCategorys.Clear();

            using (var call = Communication.Instance.GetMonthlyCategoryReport())
            {
                var token = new CancellationToken();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;
                    MonthCategorySummaryList.Add(new MonthlyCategorySummary()
                    {
                        Date = responseInfo.Date,
                        Category = responseInfo.Category,
                        TotalIncome = double.IsNaN(responseInfo.TotalIncome) ? 0 : DoubleHelper.Round( responseInfo.TotalIncome),
                        TotalExpense = double.IsNaN(responseInfo.TotalExpense) ? 0 : DoubleHelper.Round(responseInfo.TotalExpense),
                        Identification = responseInfo.Identification

                    });

                    if (!CurrentCategorys.Any(c => c.Name.Equals(responseInfo.Category)))
                    {
                        CurrentCategorys.Add(new SelectOption()
                        {
                            Name = responseInfo.Category,
                            IsSelected = false
                        });
                    }
                    if (!CurrrentIdentifications.Any(c => c.Name.Equals(responseInfo.Identification)))
                    {
                        CurrrentIdentifications.Add(new SelectOption()
                        {
                            Name = responseInfo.Identification,
                            IsSelected = false
                        });
                    }
                }
            }
        }

        [RelayCommand]
        async Task PocketMonthRecordAsync()
        {
            MonthlyReprotPanelVisible = true;
            GetCurrentMonthRecord();
            GetMonthSummary();
        }

        partial void OnReportSwtichToggledChanged(bool oldValue, bool newValue)
        {
            if (ReportSwtichToggled)
            {
                MonthlyReprotPanelVisible = true;
                GetCurrentMonthRecord();
                GetMonthSummary();
            }
            else
            {
                MonthlyReprotPanelVisible = false;
            }
        }

        partial void OnCategoryReportSwtichToggledChanged(bool oldValue, bool newValue)
        {
            if (CategoryReportSwtichToggled)
            {
                MonthlyCategoryReprotPanelVisible = true;
                GetMonthCategorySummary();
            }
            else
            {
                MonthlyCategoryReprotPanelVisible = false;
            }
        }

        partial void OnMonthlyChartSwtichToggledChanged(bool value)
        {
            if (MonthlyChartSwtichToggled)
            {
                LoadMonthChart();
                MonthSummaryIncomeChartVisible = true;
                MonthSummaryExpenseChartVisible = true;
                MonthlyChartPanelVisible = MonthSummaryIncomeChartVisible && MonthSummaryExpenseChartVisible;
            }
            else
            {
                MonthSummaryIncomeChartVisible = false;
                MonthSummaryExpenseChartVisible = false;
                MonthlyChartPanelVisible = MonthSummaryIncomeChartVisible && MonthSummaryExpenseChartVisible;
            }
        }

        private void LoadMonthChart()
        {
            if (MonthSummaryList.Count == 0)
            {
                return;
            }

            var series = DrawLineChart.GetChartSerialFromMonthlyReport(MonthSummaryList);
            if (series.Count == 0)
            {
                return;
            }

            var incomeEntries = series[0].Entries;

            if (incomeEntries.Count() >= 2)
            {
                MonthSummaryIncomeChartVisible = true;

                IncomeChart = new LineChart { Entries = incomeEntries, IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(1.5f) };
            }
            else
            {
                IncomeChart = null;
                MonthSummaryIncomeChartVisible = false;

            }

            if (series.Count != 2)
            {
                return;
            }

            var expenseEntries = series[1].Entries;
            if (expenseEntries.Count() >= 2)
            {
                MonthSummaryExpenseChartVisible = true;
                ExpenseChart = new LineChart { Entries = expenseEntries, IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(1.5f) };
            }
            else
            {
                ExpenseChart = null;
                MonthSummaryExpenseChartVisible = false;
            }
            MonthlyChartPanelVisible = MonthSummaryIncomeChartVisible && MonthSummaryExpenseChartVisible;

            //Chart = new LineSeriesChart { Series = DrawLineChart.GetChartSerialFromMonthlyReport(MonthSummaryList),
            //    ShowYAxisLines = true,
            //    ShowYAxisText = true,
            //    LegendOption =SeriesLegendOption.Top,
            //    IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(1.5f) };
            // }
        }

        private void LoadMonthCategoryChart()
        {
            var seria = DrawLineChart.GetChartSerialFromMonthlyCategoryReport(MonthCategorySummaryList, SelectedCurrentCategorys);

            if (seria.Count > 0)
            {
                CategoryChart = new LineSeriesChart
                {
                    Series = seria,
                    ShowYAxisLines = true,
                    ShowYAxisText = true,
                    LegendOption = SeriesLegendOption.Top,
                    IsAnimated = true,
                    AnimationDuration = TimeSpan.FromSeconds(1.5f)
                };
            }
            else
            {
                CategoryChart = null;
                CategoryChartVisble = false;
            }

        }
    }
}
