using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.AWS
{
    public class S3Service : IS3Service
    {
        private IAmazonS3 _s3;
        private string _bucketName { get; set; }
        public S3Service(IAmazonS3 s3)
        {
            _s3 = s3;
            _bucketName = "sd-aws-serverless-app-dev-content";

        }
        public S3Service(IAmazonS3 s3, string bucketName)
        {
            _s3 = s3;
            _bucketName = bucketName;
        }

        public async Task<string> GetHtmlFileAsync(string objectKey)
        {
            try
            {
                Stream stream = await GetObjectStream(objectKey);

                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();

                return text;
            }
            catch (Exception e)
            {
                return " eccezione " + e.Message;
            }
        }

        public async Task<Stream> GetObjectStream(string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey
            };
            var response = await _s3.GetObjectAsync(request);
            response.HttpStatusCode.ToString();
            return response.ResponseStream;
        }
    }
}
