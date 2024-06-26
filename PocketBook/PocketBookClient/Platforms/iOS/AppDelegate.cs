
using Foundation;
using UIKit;
using PocketBookClient.Helper;
using PocketBookClient.Services;

namespace PocketBookClient;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        //set 1 days interval
        UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(86400);///*UIApplication.BackgroundFetchIntervalMinimum*/);
        return base.FinishedLaunching(application, launchOptions);
    }

    [Export("application:performFetchWithCompletionHandler:")]
    public void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    {
        // Perform task
        RecordTransaction();

        // Inform system of fetch results
        completionHandler(UIBackgroundFetchResult.NewData);
    }
    private void RecordTransaction()
    {
        var scheduleService = ServiceHelper.GetService<IScheduleService>();
        if(scheduleService != null)
        {
            scheduleService.ExecuteSheduleTransaction();
        }
    }



}
