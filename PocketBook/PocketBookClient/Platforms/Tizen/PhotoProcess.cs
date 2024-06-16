using PocketBookClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Platforms
{
    public async class PhotoProcess : IPhotoProcess
    {
        public Task<string> ProcessReceipt(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
