using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AWS.Lambda
{
    public class CognitoCustomMessageRequest
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("region")]
        public string Region { get; set; }
        [JsonPropertyName("userPoolId")]
        public string UserPoolId { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("callerContext")]
        public CallerContext CallerContext { get; set; }
        [JsonPropertyName("triggerSource")]
        public string TriggerSource { get; set; }
        [JsonPropertyName("request")]
        public Request Request { get; set; }
        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }
    public class CallerContext
    {
        [JsonPropertyName("awsSdkVersion")]
        public string AwsSdkVersion { get; set; }
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
    }

    public class UserAttributes
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }
        [JsonPropertyName("email_verified")]
        public string EmailVerified { get; set; }

        [JsonPropertyName("cognito:user_status")]
        public string CognitoUserStatus { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
    public class Request
    {
        [JsonPropertyName("userAttributes")]
        public UserAttributes UserAttributes { get; set; }
        [JsonPropertyName("codeParameter")]
        public string CodeParameter { get; set; }
        [JsonPropertyName("linkParameter")]
        public string LinkParameter { get; set; }
        [JsonPropertyName("usernameParameter")]
        public string UsernameParameter { get; set; }
    }
    public class Response
    {
        [JsonPropertyName("smsMessage")]
        public string SmsMessage { get; set; }
        [JsonPropertyName("emailMessage")]
        public string EmailMessage { get; set; }
        [JsonPropertyName("emailSubject")]
        public string EmailSubject { get; set; }
    }

}
