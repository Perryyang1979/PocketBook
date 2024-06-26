using Android.App;
using Android.Content;
using Android.Util;
using PocketBookClient.Helper;
using PocketBookClient.Services;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class AlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        RecordTransaction();
    }

    private void RecordTransaction()
    {
        var scheduleService = ServiceHelper.GetService<IScheduleService>();
        if (scheduleService != null)
        {
            scheduleService.ExecuteSheduleTransaction();
        }
    }
}

