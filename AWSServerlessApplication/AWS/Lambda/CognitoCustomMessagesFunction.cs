using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.AWS.Lambda
{
    public class CognitoCustomMessagesFunction
    {
        private const string USERNAME_PLACEHOLDER = "{##USERNAME##}";
        private const string WEBSITE_URL_PLACEHOLDER = "{##URL##}";
        private const string FORGOT_PSW_HTML_BUCKET_KEY = "forgot_password.html";

        private ILambdaContext _lambdaContext;
        private S3Service _s3Service;
        private CognitoCustomMessageRequest _cognitoEvent;
        public object Handler(CognitoCustomMessageRequest cognitoEvent, ILambdaContext context)
        {
            try
            {
                var bucketName = Environment.GetEnvironmentVariable("ContentBucketName");
                var injector = new Injector();
                _cognitoEvent = cognitoEvent;
                _lambdaContext = context;
                var s3Client = injector.CreateS3();
                _s3Service = new S3Service(s3Client, bucketName);
                var triggerSource = cognitoEvent.TriggerSource;

                context?.Logger.LogLine(triggerSource);
                var result = HandleTriggerAsync(triggerSource).GetAwaiter().GetResult();
                context?.Logger.LogLine(JsonConvert.SerializeObject(result));
                return result;

            }
            catch (Exception e)
            {
                context?.Logger.LogLine($"Something went wrong: {e.Message}");
                context?.Logger.LogLine($"Something went wrong stack: {e.StackTrace}");
                return cognitoEvent;
            }
        }
        private async Task<CognitoCustomMessageRequest> HandleTriggerAsync(string triggerSource)
        {
            string subject = string.Empty;
            string body = string.Empty;
            subject = "Reset your password";
            body = await _s3Service.GetHtmlFileAsync(FORGOT_PSW_HTML_BUCKET_KEY);
            var username = _cognitoEvent.UserName.ToString();
            body = body.Replace(USERNAME_PLACEHOLDER, username);
            body = body.Replace(WEBSITE_URL_PLACEHOLDER, Environment.GetEnvironmentVariable("WebsiteUrl") ?? "https://localhost:44357");//TODO:sistemare post pubblicazione
            CustomizeResponse(subject, body);
            return _cognitoEvent;

        }
        private void CustomizeResponse(string subject, string body)
        {
            _cognitoEvent.Response.EmailSubject = subject;
            _cognitoEvent.Response.EmailMessage = body;
        }
    }
}
