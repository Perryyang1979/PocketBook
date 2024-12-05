
using Foundation;
using UIKit;
using PocketBookClient.Helper;
using PocketBookClient.Services;
using UserNotifications;
using BackgroundTasks;

namespace PocketBookClient;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    // version <IOS 13.0
    //public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    //{
    //    //set 1 days interval
    //    UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(86400);///*UIApplication.BackgroundFetchIntervalMinimum*/);

    //    var options = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
    //    var (granted, error) =  UNUserNotificationCenter.Current.RequestAuthorizationAsync(options).Result;
    //    if(granted) {
    //        ScheduleDailyNotification(); 
    //    }

    //    return base.FinishedLaunching(application, launchOptions);
    //}

    //[Export("application:performFetchWithCompletionHandler:")]
    //public void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    //{
    //    // Perform task
    //    RecordTransaction();

    //    // Inform system of fetch results
    //    completionHandler(UIBackgroundFetchResult.NewData);
    //}

    private string ScheduleTaskId = "com.Yang.Pocket.schedule";

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        base.FinishedLaunching(app, options);
        RegisterBackgroundTasks();
        ScheduleBackgroundTask();
        return true;
    }

    private void ScheduleBackgroundTask()
    {
        try
        {
            var request = new BGAppRefreshTaskRequest(ScheduleTaskId)
            {
                EarliestBeginDate = NSDate.Now.AddSeconds(86400) // 24小时后运行
            };

            BGTaskScheduler.Shared.Submit(request,out _);
            Console.WriteLine("Background fetch task scheduled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scheduling task: {ex.Message}");
        }
    }


    private void RegisterBackgroundTasks()
    {
         BGTaskScheduler.Shared.Register(ScheduleTaskId, null, HandleBackgroundFetch);
    }

    private void HandleBackgroundFetch(BGTask task)
    {
        // 开始后台任务
        PerformBackgroundFetch(task);
    }

    private void PerformBackgroundFetch(BGTask task)
    {
        // 处理任务逻辑
        Console.WriteLine("Performing background fetch...");
        RecordTransaction();

        task.SetTaskCompleted(success: true);
    }

    private void RecordTransaction()
    {
        var scheduleService = ServiceHelper.GetService<IScheduleService>();
        if(scheduleService != null)
        {
            Shell.Current.DisplayAlert("shedule check", "shedule checked today!", "OK");
            scheduleService.ExecuteSheduleTransaction();
        }
    }

    public string GetCredential()
    {
        return SecureStorage.GetAsync("perry_token").Result;
    }

    public void ScheduleDailyNotification()
{
        var token = GetCredential();

        var user = "User";
        if (!string.IsNullOrEmpty(token))
        {
            var userPass = token.Split("-@");

            user = userPass[0];
        }
        var content = new UNMutableNotificationContent();

    content.Title = "Pocket-Record";
    content.Body = $"Dear {user},it is time to record !Persistence becomes a habit.";

    var dateComponents = new NSDateComponents
    {
        Hour = 20,
        Minute = 0
    };
    var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, true);

    var request = UNNotificationRequest.FromIdentifier("dailyReminder", content, trigger);

    UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => {
        if (err != null)
        {
            // Handle any errors
        }
    });
}




}
