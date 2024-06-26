using PocketBookClient.Helper;

namespace PocketBookClient.View;

public partial class ScheduleTransactionPage : ContentPage
{
    public ScheduleTransactionPage(ScheduledTransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
       // Shell.SetNavBarIsVisible(this, true);

        var viewModel = BindingContext as ScheduledTransactionsViewModel;
        viewModel?.RefreshModel();
    }
}