using PocketBookClient.Helper;

namespace PocketBookClient.View;

public partial class AddTransactionPage : ContentPage
{
    public AddTransactionPage(AddTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
       // Shell.SetNavBarIsVisible(this, true);

        var viewModel = BindingContext as AddTransactionViewModel;
        viewModel?.RefreshModel();
    }
}