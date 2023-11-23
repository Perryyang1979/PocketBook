using PocketBookClient.Resources.Languages;
using PocketBookClient.Helper;
using System.Globalization;
namespace PocketBookClient.ViewModel
{
    public partial class SettingPageViewModel:BaseViewModel
    {
        public SettingPageViewModel() {
            SetTheme();
            SetLanguage();
        }

        private void SetTheme()
        {
            switch (MyAppSettings.APPTheme)
            {
                case 0:
                   SelectedTheme = "System";
                    break;
                case 1:
                    SelectedTheme = "Light";
                    break;
                case 2:
                    SelectedTheme = "Dark";
                    break;
            }
        }
        private void SetLanguage()
        {
            if(AppResources.Culture.TwoLetterISOLanguageName
                       .Equals("zh", StringComparison.InvariantCultureIgnoreCase))
            {
                SelectedLanguage = "ZH";
            }
            else
                SelectedLanguage = "EN";
        }

        [ObservableProperty]
        private string title;
        [ObservableProperty]
        private string selectedTheme;
        [ObservableProperty]
        private string selectedLanguage;

        partial void OnSelectedThemeChanged(string value)
        {
         //   if(value != selectedTheme)
            {
                switch (value)
                {
                    case "System":
                        MyAppSettings.APPTheme = 0;
                        break;
                    case "Light":
                        MyAppSettings.APPTheme = 1;
                        break;
                    case "Dark":
                        MyAppSettings.APPTheme = 2;
                        break;
                }

                ThemeHelper.SetTheme();
            }
        }

        partial void OnSelectedLanguageChanged(string value)
        {
           // if(value != selectedLanguage) 
            { 
                switch(value)
                {
                    case "EN":
                        //var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName
                        //.Equals("zh", StringComparison.InvariantCultureIgnoreCase) ?
                        //new CultureInfo("en-US") : new CultureInfo("zh-CN");
                        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("en-US"));
                       // SelectedLanguage = "EN";
                        break;
                    case "ZH":
                       // var switchToCulture2 = AppResources.Culture.TwoLetterISOLanguageName
                       //.Equals("zh", StringComparison.InvariantCultureIgnoreCase) ?
                       //new CultureInfo("en-US") : new CultureInfo("zh-CN");
                        LocalizationResourceManager.Instance.SetCulture(new CultureInfo("zh-CN"));
                       // SelectedLanguage = "ZH";
                        break;
                }
            }
        }
    }
}
