global using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using GrpcShared;
using Microsoft.Maui.Handlers;
using Microcharts;
using Microcharts.Maui;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Grpc.Net.Client.Web;

namespace PocketBookClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
           string baseAddress = "https://pocketbookserver20231218194147.azurewebsites.net";
        //string baseAddress = "https://localhost:7050";
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMicrocharts()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
             .ConfigureMauiHandlers(_ =>
             {
                 //ViewHandler.ViewMapper.AppendToMapping(nameof(VisualElement.BackgroundColor), (v, h) => h.UpdateValue(nameof(IView.Backgeound)));
                 ButtonHandler.Mapper.AppendToMapping(
                     nameof(Button.BackgroundColor),
                     (handler, View) => handler.UpdateValue(nameof(IView.Background)));
             });


        PocketBookClient.Helper.AppHandler.RemoveBorders(); 

        builder.Services.AddScoped(services =>
        {
            //var baseUri = new Uri(baseAddress);
            //var channel = GrpcChannel.ForAddress(baseUri);
            var baseUri = new Uri(baseAddress);
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
            return new AccountBook.AccountBookClient(channel);
        });

#if DEBUG
        //  builder.Logging.AddDebug();
#endif
// builder.Services.AddLocalization();
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

        builder.Services.AddSingleton<LoginPageViewModel>();
        builder.Services.AddSingleton<SettingPageViewModel>();
        builder.Services.AddTransient<RegistrationPageViewModel>();
        builder.Services.AddTransient<PocketOperationViewModel>();
        builder.Services.AddTransient<AddTransactionViewModel>();
        builder.Services.AddTransient<CategoryListViewModel>();
        builder.Services.AddTransient<IdentifyListViewModel>();
        builder.Services.AddTransient<UserInfoPageViewModel>();
        builder.Services.AddTransient<PasswordManageViewModel>();


        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<SettingPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddTransient<PocketOperationPage>();
        builder.Services.AddTransient<AddTransactionPage>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<IdentifyListPage>();
        builder.Services.AddTransient<UserInfoPage>();
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<PasswordManagePage>();

        return builder.Build();
	}
}
