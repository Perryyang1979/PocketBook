using GrpcShared;
namespace PocketBookClient;

public partial class MainPage : ContentPage
{
    User.UserClient _userClient;
    AccountBook.AccountBookClient _accountClient;

    public MainPage(User.UserClient userClient,AccountBook.AccountBookClient accountClient)
    {
        _userClient = userClient;
        _accountClient = accountClient;

        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var obj = new object();
        var rnd = new Random(obj.GetHashCode());

        int randomId = rnd.Next(1, 4);
        var request = new GetUserRequest { UserId = randomId };

        //Test UserRegister

        var userRegisterRequest = new PocketUserInfo()
        {
            Username ="Perry",
            Password = "123456",
            PhoneNumber = "1234567890",
            Email= "eguyang@",
        };

        try
        {
            //var userperson = await _userClient.GetUserByIdAsync(request);

            //if (userperson != null)
            //{
            //    CounterBtn.Text = $"{userperson.UserId} {userperson.UserName}";
            //    SemanticScreenReader.Announce(CounterBtn.Text);
            //}

            var registerState = await _accountClient.RegisterUserAsync(userRegisterRequest);

            if(registerState != null )
            {
                    CounterBtn.Text = registerState.Message;
                    SemanticScreenReader.Announce(CounterBtn.Text);
            }
        }
        catch (Exception ex)
        {

        }
    }
}

