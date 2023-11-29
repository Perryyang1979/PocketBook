namespace PocketBookClient.View;

public partial class CategoryListPage : ContentPage
{
	public CategoryListPage(CategoryListViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}


    protected override bool OnBackButtonPressed()
    {
        return false;
    }
}