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

    public string CurrentUserName { get; set; } 
}
