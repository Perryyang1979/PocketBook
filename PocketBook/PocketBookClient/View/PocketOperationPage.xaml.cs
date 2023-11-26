namespace PocketBookClient.View;

public partial class PocketOperationPage : ContentPage
{
	public PocketOperationPage(PocketOperationViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}