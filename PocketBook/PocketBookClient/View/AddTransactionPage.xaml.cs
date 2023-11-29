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
}