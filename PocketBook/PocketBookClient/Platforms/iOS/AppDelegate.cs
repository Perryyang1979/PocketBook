
using Foundation;
using UIKit;
using PocketBookClient.Data;
using PocketBookClient.Services;
using UserNotifications;

namespace PocketBookClient;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    //# version<IOS 13.0
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        //set 1 days interval
        UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(86400);///*UIApplication.BackgroundFetchIntervalMinimum*/);

        var options = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
        var (granted, error) = UNUserNotificationCenter.Current.RequestAuthorizationAsync(options).Result;
        if (granted)
        {
            ScheduleDailyNotification();
            LoadAndScheduleTasksAsync();
        }

        return base.FinishedLaunching(application, launchOptions);
    }

    //[Export("application:performFetchWithCompletionHandler:")]
    //public void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    //{
    //    // Perform task
    //    RecordTransaction();

    //    // Inform system of fetch results
    //    completionHandler(UIBackgroundFetchResult.NewData);
    //}

    //private string ScheduleTaskId = "com.Yang.Pocket.schedule";

    //public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    //{
    //    base.FinishedLaunching(app, options);
    //    RegisterBackgroundTasks();
    //    ScheduleBackgroundTask();
    //    return true;
    //}

    //private void ScheduleBackgroundTask()
    //{
    //    try
    //    {
    //        var request = new BGAppRefreshTaskRequest(ScheduleTaskId)
    //        {
    //            EarliestBeginDate = NSDate.Now.AddSeconds(86400) // 24小时后运行
    //        };

    //        BGTaskScheduler.Shared.Submit(request, out _);
    //        Console.WriteLine("Background fetch task scheduled.");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error scheduling task: {ex.Message}");
    //    }
    //}


    //private void RegisterBackgroundTasks()
    //{
    //    BGTaskScheduler.Shared.Register(ScheduleTaskId, null, HandleBackgroundFetch);
    //}

    //private void HandleBackgroundFetch(BGTask task)
    //{
    //    // 开始后台任务
    //    PerformBackgroundFetch(task);
    //}

    //private void PerformBackgroundFetch(BGTask task)
    //{
    //    // 处理任务逻辑
    //    Console.WriteLine("Performing background fetch...");
    //    RecordTransaction();

    //    task.SetTaskCompleted(success: true);
    //}

    //private void RecordTransaction()
    //{
    //    var scheduleService = ServiceHelper.GetService<IScheduleService>();
    //    if (scheduleService != null)
    //    {
    //        Shell.Current.DisplayAlert("shedule check", "shedule checked today!", "OK");
    //        scheduleService.ExecuteSheduleTransaction();
    //    }
    //}

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

    public async Task LoadAndScheduleTasksAsync()
    {
        var tasks = await Communication.Instance.GetAllSheduledTransactions();
        foreach (var task in tasks)
        {
            ScheduleTask(task);
        }
    }

    /// <summary>
    /// 调度单个任务。
    /// </summary>
    private void ScheduleTask(ScheduledTransaction task)
    {
        var now = DateTime.Now;

        // 检查时间范围
        if (now < task.StartDate || now > task.EndDate)
        {
            return;
        }
        //每天的任务默认都可以执行
        // 每周的任务
        if (task.Frequency == FrequencyEnum.Weekly)
        {
            if (now.DayOfWeek != task.DayOfWeek)
            {
                Console.WriteLine($"Task {task.ToString} does not match today's day of week.");
                return;
            }
        }
        // 每月的任务
        else if (task.Frequency == FrequencyEnum.Monthly)
        {
            if (task.DayOfMonth != now.Day)
            {
                Console.WriteLine($"Task {task.ToString} does not match today's day of month.");
                return;
            }
        }

        // 创建通知
        var content = new UNMutableNotificationContent
        {
            Title = "Pocket-Record",
            Body = $"There is Schedule task today,please execute it!",
            Sound = UNNotificationSound.Default
        };

        // 设置触发器
        var trigger = UNCalendarNotificationTrigger.CreateTrigger(
            new NSDateComponents
            {
                Hour = 20,
                Minute = 30
            },
            repeats: true
        );

        var request = UNNotificationRequest.FromIdentifier(task.ToString(), content, trigger);

        // 添加通知
        UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Failed to schedule task {task.ToString}: {error}");
            }
            else
            {
                Console.WriteLine($"Task {task.ToString()} scheduled successfully.");
            }
        });
    }




}
