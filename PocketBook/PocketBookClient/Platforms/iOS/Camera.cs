using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using CoreImage;
using UIKit;
using Vision;
using PocketBookClient.Services;

namespace PocketBookClient.Platforms
{
    public class Camera : ICamera
    {
        private UIImagePickerController _picker;

        public async Task<FileResult> CapturePhotoAsync()
        {
            TaskCompletionSource<FileResult> taskCompletionSource = new TaskCompletionSource<FileResult>();

            if (await Permissions.RequestAsync<Permissions.Camera>() != PermissionStatus.Granted)
            {
                taskCompletionSource.TrySetException(new PermissionException("Camera permission not granted"));
                return await taskCompletionSource.Task;
            }

            //Create an image picker object
            _picker = new UIKit.UIImagePickerController
            {
                SourceType = UIKit.UIImagePickerControllerSourceType.Camera
            };

            //Make sure we can find the top most view controller to launch the camera
            UIKit.UIViewController vc = Platform.GetCurrentUIViewController();

            vc.PresentViewController(_picker, true, null);
            _picker.FinishedPickingMedia += async (_, e) =>
            {
                // get the original image
                UIKit.UIImage originalImage = e.Info[UIKit.UIImagePickerController.OriginalImage] as UIKit.UIImage;

                var normalizedImage = HelperService.NormalizeOrientation(originalImage);

                string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jpgFilename = Path.Combine(documentsDirectory, $"{Guid.NewGuid()}.jpg");
                NSData imgData = normalizedImage.AsJPEG();

                await vc.DismissViewControllerAsync(true);

                if (imgData.Save(jpgFilename, false, out NSError err))
                {
                    taskCompletionSource.TrySetResult(new FileResult(jpgFilename));
                }
                else
                {
                    taskCompletionSource.TrySetException(new Exception("Unable to save the image" + err));
                }

                _picker.DismissViewController(true, null);
            };
            _picker.Canceled += async (_, e) =>
            {
                await vc.DismissViewControllerAsync(true);
                taskCompletionSource.TrySetResult(null);
                _picker?.Dispose();
                _picker = null;
            };

            return await taskCompletionSource.Task;
        }

        private class CameraDelegate : UIKit.UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIKit.UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissModalViewController(true);
            }
        }
    }
}
