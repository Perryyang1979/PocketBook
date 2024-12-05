using Microcharts;
using Microsoft.Maui.Layouts;
using PocketBookClient.Data;
using PocketBookClient.Services;
using PocketBookClient.UI;
using SkiaSharp;
using PocketBookClient.Helper;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace PocketBookClient.ViewModel
{
    public partial class AdvancedReportViewModel:BaseViewModel
    {
        public ObservableCollection<UserTransaction> MonthTransactionList { get; } = new ObservableCollection<UserTransaction>();
        public ObservableCollection<MonthlySummary> MonthSummaryList { get; } = new ObservableCollection<MonthlySummary>();
        public ObservableCollection<YearlySummary> YearlySummaryList { get; } = new ObservableCollection<YearlySummary>();

        public ObservableCollection<YearlySummary> YearlySpecialIdentifySummaryList { get; } = new ObservableCollection<YearlySummary>();

        public List<string> SelectedCategoryDescriptions { get; } = new List<string>();

        public List<bool> SelectedCategoryColumnVisible = new List<bool>();
        public ObservableCollection<MonthlyCategorySummary> MonthCategorySummaryList { get; } = new ObservableCollection<MonthlyCategorySummary>();
        public ObservableCollection<GridCategoryData> SelectedCategorySummaryList { get; } = new ObservableCollection<GridCategoryData>();

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
        [ObservableProperty]
        bool yearlyIdentificationSummarylVisible = false;
        [ObservableProperty]
        bool gridCategorySummaryVisible = false;

        //grid not support array
        //[ObservableProperty]
        // List<string> selectedCategoryDescriptions  = new List<string>();

        [ObservableProperty]
         ObservableCollection<SelectOption> selectedCurrentCategorys  = new ObservableCollection<SelectOption>();

        [ObservableProperty]
        string titleOfFirstCategory = string.Empty;
        [ObservableProperty]
        string titleOfSecondCategory = string.Empty;
        [ObservableProperty]
        string titleOfThirdCategory = string.Empty;
        [ObservableProperty]
        string titleOfFourthCategory = string.Empty;


        private const int ThreholdCount = 4;
        private const string FamilyName = "  ";
        private const int SupportItemsCount = 4;

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
            YearlyIdentificationSummarylVisible = false;
            GridCategorySummaryVisible = false;

            MonthlyChartPanelVisible = MonthSummaryIncomeChartVisible && MonthSummaryExpenseChartVisible;
            SelectedCurrentCategorys.CollectionChanged += SelectedCurrentCategorys_CollectionChanged;
        }

        public async void RefreshModel()
        {
            if (Settings.Current.CurrentUserSetting == null || string.IsNullOrEmpty(Settings.Current.CurrentUserSetting.UserName))
            {
                return;
            }
            GetCurrentMonthRecord();
            GetMonthSummary();
        }

        private void  InitGridTitle()
        {
             TitleOfFirstCategory = string.Empty;
             TitleOfSecondCategory = string.Empty;
             TitleOfThirdCategory = string.Empty;
             TitleOfFourthCategory = string.Empty;
        }

        private void SetGridTitle(List<string> titles)
        {
           //default make sure 
           if(titles.Count<4)
            {
                for(int i = titles.Count;i<SupportItemsCount;i++)
                titles.Add(string.Empty);
            }

            TitleOfFirstCategory = titles[0];
            TitleOfSecondCategory = titles[1];
            TitleOfThirdCategory = titles[2];
            TitleOfFourthCategory = titles[3];
        }


        private void SelectedCurrentCategorys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateGridCategoryDescriptions();
            if (SelectedCurrentCategorys.Count > 0)
            {
                CategoryChartVisble = true;
                GridCategorySummaryVisible = true;
                PopulateCategoryGridDataSource();



#if ANDROID
                if (SelectedCategorySummaryList.Count > 1)
                {
                    LoadMonthCategoryChart();
                }
#elif IOS
                CategoryChart = null;
                CategoryChartVisble = false;
#endif
            }
            else
            {
                GridCategorySummaryVisible = false;
                CategoryChart = null;
                CategoryChartVisble = false;
            }
        }

        private void PopulateGridCategoryDescriptions()
        {
            InitGridTitle();
            SelectedCategoryDescriptions.Clear();
            SelectedCategoryColumnVisible.Clear();
            for (int i = 0;i< SelectedCurrentCategorys.Count;i++)
            {
                SelectedCategoryDescriptions.Add(SelectedCurrentCategorys[i].Name);
                SelectedCategoryColumnVisible.Add( true);
            }

            for(int i = SelectedCurrentCategorys.Count;i< SupportItemsCount; i++)
            {
                SelectedCategoryColumnVisible.Add(false);
            }

            SetGridTitle(SelectedCategoryDescriptions);
        }

        private void PopulateCategoryGridDataSource()
        {
            SelectedCategorySummaryList.Clear();
            Dictionary<string, GridCategoryData> dictionaryCategoryData = new Dictionary<string, GridCategoryData>();
            for (int i = 0; i< SelectedCategoryDescriptions.Count;i++)
            {
                var selectedCategory = SelectedCategoryDescriptions[i];
                if(selectedCategory == string.Empty)
                {
                    continue;
                }
                foreach (var monthsummary in MonthCategorySummaryList.Where(m => m.Category.Equals(selectedCategory)).OrderBy(m=>m.Date))
                {
                   if(dictionaryCategoryData.ContainsKey(monthsummary.Date))
                   {
                        dictionaryCategoryData[monthsummary.Date].AppendMoney(i, monthsummary.TotalMoney);
                   }
                   else
                   {     var money = new double[4];
                        money[i] = (double)monthsummary.TotalMoney;
                        dictionaryCategoryData.Add(monthsummary.Date, new GridCategoryData(monthsummary.Date, money,SelectedCategoryColumnVisible));
                   }
                }
            }

            foreach(var categorydata in dictionaryCategoryData.Values)
            {
                SelectedCategorySummaryList.Add(categorydata);
            }
        }

        private async void GetCurrentMonthRecord()
        {
            MonthTransactionList.Clear();
            try
            {
               foreach(var usertrans in Communication.Instance.GetDailyTransationsInCurrentMonth())
                {
                    MonthTransactionList.Add(usertrans);
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
                foreach(var summary in Communication.Instance.GetMonthlyReport())
                {
                    MonthSummaryList.Add(summary);
                }
        

                GenerateYearlySummary();
#if ANDROID
                if (MonthSummaryList.Count >= ThreholdCount)
                {
                    MonthlyChartEnabled = true;
                }
                else
                {
                    MonthlyChartEnabled = false;
                }
#elif IOS
                MonthlyChartEnabled = false;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void GenerateYearlySummary()
        {
            YearlySummaryList.Clear();
            if(MonthSummaryList.Count==0)
            {
                return;
            }


            Dictionary<string, YearlySummary> yearlySummaryTotal = new Dictionary<string, YearlySummary>();
            //Dictionary<string, YearlySummary> yearlyIdentifySummaryTotal = new Dictionary<string, YearlySummary>();

            yearlySummaryTotal.Add(FamilyName, new YearlySummary() { UserName = FamilyName });
            foreach (var monthlySummary in MonthSummaryList)
            {
                if (yearlySummaryTotal.ContainsKey(monthlySummary.UserName))
                {
                    yearlySummaryTotal[monthlySummary.UserName].TotalIncome += DoubleHelper.Round( monthlySummary.TotalIncome);
                    yearlySummaryTotal[monthlySummary.UserName].TotalExpense += DoubleHelper.Round(monthlySummary.TotalExpense);
                }
                else
                {
                    yearlySummaryTotal.Add(monthlySummary.UserName, new YearlySummary(monthlySummary));
                }


                yearlySummaryTotal[FamilyName].TotalIncome += monthlySummary.TotalIncome;
                yearlySummaryTotal[FamilyName].TotalExpense += monthlySummary.TotalExpense;
            }

                  foreach(var summary in yearlySummaryTotal.Values.OrderByDescending(y=>y.UserName))
                {
                    YearlySummaryList.Add(summary);
                }
        }

        private async void GetMonthCategorySummary()
        {
            MonthCategorySummaryList.Clear();
            CurrentCategorys.Clear();


            foreach(var summary in Communication.Instance.GetMonthlyCategoryReport())
            {
                MonthCategorySummaryList.Add(summary);

                if (!CurrentCategorys.Any(c => c.Name.Equals(summary.Category)))
                {
                    CurrentCategorys.Add(new SelectOption()
                    {
                        Name = summary.Category,
                        IsSelected = false
                    });
                }
            }

            MonthlyCategoryReprotPanelVisible = MonthCategorySummaryList.Count > 1;


          
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
                GetMonthCategorySummary();
                PopulateGridCategoryDescriptions();
            }
            else
            {
                MonthlyCategoryReprotPanelVisible = false;

                if (SelectedCurrentCategorys.Count > 0)
                {
                    for (int i = 0; i < SelectedCurrentCategorys.Count; i++)
                    {
                        SelectedCurrentCategorys[i].IsSelected = false;
                    }

                    for(int i = 0;i<CurrentCategorys.Count;i++ )
                    {
                        var current = CurrentCategorys[i];
                        if(current.IsSelected)
                        {
                            current.IsSelected = false;
                        }
                    }
                    SelectedCurrentCategorys.Clear();
                }
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
#if ANDROID

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
#elif IOS
            IncomeChart = null;
            MonthSummaryIncomeChartVisible = false;
#endif

            if (series.Count != 2)
            {
                return;
            }

            var expenseEntries = series[1].Entries;
#if ANDROID
            if (expenseEntries.Count() >= 2 )
            {
                MonthSummaryExpenseChartVisible = true;
                ExpenseChart = new LineChart { Entries = expenseEntries, IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(1.5f) };
            }
            else
            {
                ExpenseChart = null;
                MonthSummaryExpenseChartVisible = false;
            }
#elif IOS
            ExpenseChart = null;
            MonthSummaryExpenseChartVisible = false;
#endif
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
            var seria = DrawLineChart.GetChartSerialFromMonthlyCategoryReport(SelectedCategorySummaryList,SelectedCategoryDescriptions);

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
