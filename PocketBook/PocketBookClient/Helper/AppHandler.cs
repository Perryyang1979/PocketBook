using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Helper
{
    public static class AppHandler
    {
        public static void RemoveBorders()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
            if (view is BorderlessEntry entry) {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
            handler.PlatformView.Layer.BorderWidth = 0;
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            
#endif
                }
                });
        

//            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
//            {
//#if ANDROID
//                handler.PlatformView.Background = null;
//                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
//#elif IOS
//            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
//            handler.PlatformView.Layer.BorderWidth = 0;
//            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
//#endif
//            });
        }
    }
}
