using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Platforms
{
    public static  class HelperService
    {
        public static UIKit.UIImage NormalizeOrientation(UIKit.UIImage image)
        {
            if (image.Orientation == UIKit.UIImageOrientation.Up)
            {
                return image;
            }

            UIKit.UIGraphics.BeginImageContext(image.Size);
            image.Draw(new CoreGraphics.CGRect(CoreGraphics.CGPoint.Empty, image.Size));
            var normalizedImage = UIKit.UIGraphics.GetImageFromCurrentImageContext();
            UIKit.UIGraphics.EndImageContext();
            return normalizedImage;
        }
    }
}
