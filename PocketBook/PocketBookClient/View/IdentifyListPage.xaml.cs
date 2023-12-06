namespace PocketBookClient.View;

public partial class IdentifyListPage : ContentPage
{
	public IdentifyListPage(IdentifyListViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}


    protected override bool OnBackButtonPressed()
    {

		return base.OnBackButtonPressed();
     //   return false;
    }
}