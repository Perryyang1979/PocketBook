global using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using GrpcShared;
using Microsoft.Maui.Handlers;

namespace PocketBookClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        string baseAddress = "https://localhost:7050";
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
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

        //builder.Services.AddScoped(services =>
        //{
        //    var baseUri = new Uri(baseAddress);
        //    var channel = GrpcChannel.ForAddress(baseUri);
        //    return new User.UserClient(channel);
        //});

        builder.Services.AddScoped(services =>
        {
            var baseUri = new Uri(baseAddress);
            var channel = GrpcChannel.ForAddress(baseUri);
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

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddSingleton<SettingPage>();

        return builder.Build();
	}
}
