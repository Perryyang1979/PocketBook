
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
        public Task<FileResult> CapturePhotoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
