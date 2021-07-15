using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.AWS
{
    interface IS3Service
    {
        Task<string> GetHtmlFileAsync(string objectKey);
        Task<Stream> GetObjectStream(string objectKey);
    }
}
