namespace AWSServerlessApplication.Utils
{
    public class AppSettings
    {
        public string TableNamePrefix { get; set; }
        public string UserPoolId { get; set; }
        public string UserPoolClientId { get; set; }
        public string ProjectServiceBaseAddress { get; set; }
        public string GrooveServiceBaseAddress { get; set; }
    }
}