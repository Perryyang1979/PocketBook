using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcharts;
using Microsoft.Maui.Layouts;
using PocketBookClient.Data;
using PocketBookClient.Services;
using PocketBookClient.UI;
using SkiaSharp;

namespace PocketBookClient.ViewModel
{
    public partial class PocketOperationViewModel:BaseViewModel
    {
        public ObservableCollection<UserTransaction> TransactionList { get; } = new ObservableCollection<UserTransaction>();
        public ObservableCollection<UserTransaction> MonthTransactionList { get; } = new ObservableCollection<UserTransaction>();

        public ObservableCollection<MonthlySummary> MonthSummaryList { get; } = new ObservableCollection<MonthlySummary>();

        public ObservableCollection<MonthlyCategorySummary> MonthCategorySummaryList { get; } = new ObservableCollection<MonthlyCategorySummary>();
        public ObservableCollection<SelectOption> SelectedCurrentCategorys { get; set; } = new ObservableCollection<SelectOption>();

        private const int RecentlyDaysDefine = 3;

        [ObservableProperty]
        public bool monthlyReprotPanelVisible = false;
        [ObservableProperty]
        public bool monthlyCategoryReprotPanelVisible = false;
        [ObservableProperty]
        public bool reportSwtichToggled = false;
        [ObservableProperty]
        public bool categoryReportSwtichToggled = false;
        [ObservableProperty]
        public ObservableCollection<SelectOption> currrentIdentifications;
        [ObservableProperty]
        public ObservableCollection<SelectOption> selectedIdentifications = new ObservableCollection<SelectOption>();
        [ObservableProperty]
        public ObservableCollection<SelectOption> currentCategorys = new ObservableCollection<SelectOption>();
        [ObservableProperty]
        bool categoryChartVisble = false;

        public PocketOperationViewModel() {

            Init();
            GetMonthSummary();
        }

        private void Init()
        {
            GetRecentlyRecord();
            MonthlyReprotPanelVisible = false;
            MonthlyCategoryReprotPanelVisible = false;
            ReportSwtichToggled = false;
            CategoryReportSwtichToggled = false;
            CategoryChartVisble = false;
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

        private async void GetRecentlyRecord()
        {
            TransactionList.Clear();

            var endDate = DateTime.Today.Date;

            var startDate = endDate.AddDays(0-RecentlyDaysDefine);

            try
            {
                using (var call = Communication.Instance.GetDailyTransationsStreamAsync(startDate, endDate))
                {
                    var token = new CancellationToken();
                    while (await call.ResponseStream.MoveNext(token))
                    {
                        var responseInfo = call.ResponseStream.Current;
                        TransactionList.Add(new UserTransaction()
                        {
                            Date = responseInfo.TransactionDate,
                            Category = responseInfo.Category,
                            IsIncome = responseInfo.IsIncome,
                            Comments = responseInfo.Comments,
                            Amount = responseInfo.Amount,
                            UserName = responseInfo.Username,
                            IdentifyDescription = responseInfo.Identification
                        });
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
                            Amount = responseInfo.Amount,
                            UserName = responseInfo.Username,
                            IdentifyDescription = responseInfo.Identification
                        });
                    }
                }
            }catch(Exception ex)
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
                            TotalIncome = responseInfo.TotalIncome,
                            TotalExpense = responseInfo.TotalExpense

                        });
                    }
                }
                LoadMonthChart();
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void GetMonthCategorySummary()
        {
            MonthCategorySummaryList.Clear();
            CurrentCategorys.Clear();

            using (var  call = Communication.Instance.GetMonthlyCategoryReport())
            {
                var token = new CancellationToken();
                while (await call.ResponseStream.MoveNext(token))
                {
                    var responseInfo = call.ResponseStream.Current;
                    MonthCategorySummaryList.Add(new MonthlyCategorySummary()
                    {
                        Date = responseInfo.Date,
                        Category = responseInfo.Category,
                        TotalIncome = double.IsNaN(responseInfo.TotalIncome)?0:responseInfo.TotalIncome,
                        TotalExpense = double.IsNaN(responseInfo.TotalExpense)?0:responseInfo.TotalExpense,
                        Identification = responseInfo.Identification
                        
                    });

                    if(!CurrentCategorys.Any(c=>c.Name.Equals(responseInfo.Category)))
                    {
                        CurrentCategorys.Add(new SelectOption()
                        {
                            Name = responseInfo.Category,
                            IsSelected = false
                        }); 
                    }
                    if (!string.IsNullOrEmpty(responseInfo.Identification)&&!CurrrentIdentifications.Any(c => c.Name.Equals(responseInfo.Identification)))
                    {
                        CurrrentIdentifications.Add(new SelectOption()
                        {
                            Name = responseInfo.Identification,
                            IsSelected = false
                        });
                    }
                }
            }

           // LoadMonthChart();
        }

        [RelayCommand]
        async Task PocketRecordAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddTransactionPage));
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
           if(CategoryReportSwtichToggled)
            {
                MonthlyCategoryReprotPanelVisible = true;
                GetMonthCategorySummary();
            }
           else
            {
                MonthlyCategoryReprotPanelVisible = false;
            }
        }

        //partial void OnSelectedCurrentCategorysChanging(ObservableCollection<SelectOption> value)
        //{
        //    LoadMonthCategoryChart();
        //    throw new NotImplementedException();
        //}
        //partial void OnSelectedCurrentCategorysChanged(ObservableCollection<SelectOption> value)
        //{
        //    LoadMonthCategoryChart();
        //}

        [ObservableProperty]
        private Microcharts.Chart chart;

        [ObservableProperty]
        private Microcharts.Chart categoryChart;

        private void LoadMonthChart()
        {
            if(MonthSummaryList.Count == 0) { return; }
          //  if (Chart == null)
            {
                //  Chart = new LineChart { Entries = GetEntriesFromMonthlyReport(), IsAnimated = true,AnimationDuration =  TimeSpan.FromSeconds(1.5f) };

                Chart = new LineSeriesChart { Series = DrawLineChart.GetChartSerialFromMonthlyReport(MonthSummaryList),
                    ShowYAxisLines = true,
                    ShowYAxisText = true,
                    LegendOption =SeriesLegendOption.Top,
                    IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(1.5f) };
            }
        }

        private void LoadMonthCategoryChart()
        {
            var seria = DrawLineChart.GetChartSerialFromMonthlyCategoryReport(MonthCategorySummaryList, SelectedCurrentCategorys);

            if (seria.Count > 0)
            {
                CategoryChart = new LineSeriesChart
                {
                    Series =seria,
                    ShowYAxisLines = true,
                    ShowYAxisText = true,
                    LegendOption = SeriesLegendOption.Top,
                    IsAnimated = true,
                    AnimationDuration = TimeSpan.FromSeconds(1.5f)
                };
            }
            else { 
                CategoryChart = null; 
                CategoryChartVisble = false;
            }
            
        }
    }
}
