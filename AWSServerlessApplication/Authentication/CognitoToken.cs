namespace AWSServerlessApplication.Authentication
{
    public class CognitoToken
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}