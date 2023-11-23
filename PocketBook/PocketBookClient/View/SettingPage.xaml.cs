namespace PocketBookClient.View;

public partial class SettingPage : ContentPage
{
	public SettingPage(SettingPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}