using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Models
{
    public class User:DynamoDBUser
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
