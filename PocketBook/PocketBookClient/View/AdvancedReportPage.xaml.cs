namespace PocketBookClient.View;

public partial class AdvancedReportPage : ContentPage
{
	public AdvancedReportPage(AdvancedReportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Shell.SetNavBarIsVisible(this, true);

        var viewModel = BindingContext as AdvancedReportViewModel;
        viewModel?.RefreshModel();
    }

    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var viewModel = BindingContext as AdvancedReportViewModel;
        if (viewModel != null)
        {
            viewModel.OnCheckBoxCheckedChanged(e.Value);
        }
    }
}