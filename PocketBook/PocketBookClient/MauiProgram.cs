
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microcharts;
using Microcharts.Maui;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using SkiaSharp.Views.Maui.Controls.Hosting;
using PocketBookClient.Services;
using PocketBookClient.Platforms;

namespace PocketBookClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMicrocharts()
            .UseSkiaSharp()
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


#if DEBUG
        //  builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<ICamera,Camera>();
        builder.Services.AddSingleton<IPhotoProcess, PhotoProcess>();
        builder.Services.AddSingleton<IScheduleService, ScheduleTransactionService>();

        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddSingleton<SettingPageViewModel>();
        builder.Services.AddTransient<RegistrationPageViewModel>();
        builder.Services.AddTransient<AddTransactionViewModel>();
        builder.Services.AddTransient<CategoryListViewModel>();
        builder.Services.AddTransient<IdentifyListViewModel>();
        builder.Services.AddTransient<UserInfoPageViewModel>();
        builder.Services.AddTransient<PasswordManageViewModel>();
        builder.Services.AddTransient<AdvancedReportViewModel>();
        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<ScheduledTransactionsViewModel>();


        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<SettingPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddTransient<AddTransactionPage>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<IdentifyListPage>();
        builder.Services.AddTransient<UserInfoPage>();
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<PasswordManagePage>();
        builder.Services.AddTransient<AdvancedReportPage>();
        builder.Services.AddTransient<UserListPage>();
        builder.Services.AddTransient<ScheduleTransactionPage>();

        return builder.Build();
	}
}
