using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Helper
{
    public static class MyAppSettings
    {
        // 0 = default, 1 = light, 2 = dark
        const int appTheme = 0;
        public static int APPTheme
        {
            get => Preferences.Get(nameof(APPTheme), appTheme);
            set => Preferences.Set(nameof(APPTheme), value);
        }
    }
}
