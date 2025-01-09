using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Java.Util;

namespace PocketBookClient;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetDailyAlarm();
    }

    private void SetDailyAlarm()
    {
        var context = Android.App.Application.Context;
        var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

        var intent = new Intent(context, typeof(AlarmReceiver));
        var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);

        var calendar = Calendar.Instance;
        calendar.Set(CalendarField.HourOfDay, 21);
        calendar.Set(CalendarField.Minute, 40);

        calendar.Set(CalendarField.Second, 0);

        // 如果当前时间已经过了今天的目标时间，则首次触发时间应为明天
        if (Calendar.Instance.After(calendar))
        {
            calendar.Add(CalendarField.DayOfMonth, 1);
        }

        // 设置定时任务，每月重复
        alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay * 1, pendingIntent);
    }
}
