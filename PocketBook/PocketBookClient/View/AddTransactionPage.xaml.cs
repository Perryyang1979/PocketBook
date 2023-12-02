namespace PocketBookClient.View;

public partial class AddTransactionPage : ContentPage
{
	public AddTransactionPage(AddTransactionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    //use method make sure when back button is pressed  to this page 
    // page will be refresh 
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var vm = BindingContext as AddTransactionViewModel;
         vm?.RefreshModel();

    }

}