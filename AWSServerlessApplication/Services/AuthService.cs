
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Services.Interfaces;

namespace AWSServerlessApplication.Services
{
    public class AuthService : IAuthService
    {
        public DynamoDBUser AuthUser { get; private set; }

        public void Init(DynamoDBUser user)
        {
            AuthUser = user;
        }
    }
}