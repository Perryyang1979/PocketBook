
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketBookClient.Services;


namespace PocketBookClient.Platforms
{
    public class Camera : ICamera
    {
        public async Task<FileResult> CapturePhotoAsync()
        {
            TaskCompletionSource<FileResult> taskCompletionSource = new TaskCompletionSource<FileResult>();
            taskCompletionSource.TrySetException(new PermissionException("Camera permission not granted"));
            return await taskCompletionSource.Task;
        }
    }
}
