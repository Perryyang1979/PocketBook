namespace PocketBookClient.View;

public partial class AdvancedReportPage : ContentPage
{
	public AdvancedReportPage(AdvancedReportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}