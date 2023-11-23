using PocketBookClient.Resources.Languages;
using System.ComponentModel;
using System.Globalization;

namespace PocketBookClient.Helper;

public class LocalizationResourceManager : INotifyPropertyChanged
{
    private LocalizationResourceManager()
    {
        AppResources.Culture = CultureInfo.CurrentCulture;
    }

    public static LocalizationResourceManager Instance { get; } = new();

    public object this[string resourceKey]
        => AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

    public event PropertyChangedEventHandler PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        AppResources.Culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public string Language
    {
        get
        {
            if (AppResources.Culture.TwoLetterISOLanguageName
                      .Equals("zh", StringComparison.InvariantCultureIgnoreCase))
                return "ZH";
            return  "EN";
        }
    }
}
