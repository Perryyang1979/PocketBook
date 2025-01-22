
using CommunityToolkit.Mvvm.ComponentModel;

namespace PocketBookClient.Services;

public partial class Settings : ObservableObject
{
    private static Settings settings;

    public static Settings Current
    {
        get
        {
            return settings??(settings=new Settings()); 
        }
    }

    public const string LoggedInKey = "logged_in";

    public RecordYearLimit YearLimit { get; set; } = RecordYearLimit.ThisYear;

    public RecordYearLimit PossibleLimit { get; set; } = RecordYearLimit.ThisYear;

    public bool LoggedIn
    {
        get
        {
            return Preferences.Default.Get<bool>(LoggedInKey, false);
        }
        set
        {
            Preferences.Default.Set(LoggedInKey, value);
        }
    }

    public delegate void CurrentUserEventHandler(object sender, EventArgs e);

    // Declare the event.
    public  event CurrentUserEventHandler OnCurrentUserChangeEvent;

    private CurrentUserLoginSetting _currentUserLoginSetting;
    public CurrentUserLoginSetting CurrentUserSetting 
    {
        get
        {
            return _currentUserLoginSetting;
        }

        set
        {
            _currentUserLoginSetting = value;
            OnPropertyChanged(nameof(CurrentUserSetting));
            OnCurrentUserChangeEvent?.Invoke(this, new EventArgs());
        }
    } 

    public class CurrentUserLoginSetting
    {
        public string UserName { get; set; }
        public string UserGroupId { get; set; }
    }


    public enum RecordYearLimit
    {
        ThisYear = 0,
        LastYear = 1,
        TheYearBeforeLastYear = 2,
    }
}
