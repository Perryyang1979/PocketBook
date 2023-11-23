using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Helper
{
    public static class ThemeHelper
    {
        public static void SetTheme()
        {
            switch (MyAppSettings.APPTheme)
            {
                //default
                case 0:
                    App.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
                //light
                case 1:
                    App.Current.UserAppTheme = AppTheme.Light;
                    break;
                //dark
                case 2:
                    App.Current.UserAppTheme = AppTheme.Dark;
                    break;
            }

            var nav = App.Current.MainPage as NavigationPage;

            var e = DependencyService.Get<IEnvironment>();
            if (App.Current.RequestedTheme == AppTheme.Dark)
            {
                e?.SetStatusBarColor(Colors.Black, false);
                if (nav != null)
                {
                    nav.BarBackgroundColor = Colors.Black;
                    nav.BarTextColor = Colors.White;
                }
            }
            else
            {
                e?.SetStatusBarColor(Colors.White, true);
                if (nav != null)
                {
                    nav.BarBackgroundColor = Colors.White;
                    nav.BarTextColor = Colors.Black;
                }
            }


        }
    }
}
