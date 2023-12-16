using GrpcShared;
namespace PocketBookClient;

public partial class MainPage : ContentPage
{
    private readonly AccountBook.AccountBookClient _accountClient;

    public MainPage(AccountBook.AccountBookClient accountClient)
    {
        _accountClient = accountClient;

        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
      
    }
}

