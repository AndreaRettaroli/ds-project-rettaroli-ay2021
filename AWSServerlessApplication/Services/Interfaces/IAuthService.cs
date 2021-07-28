using AWSServerlessApplication.Models;

namespace AWSServerlessApplication.Services.Interfaces
{
    public interface IAuthService
    {
        DynamoDBUser AuthUser { get; }
        void Init(DynamoDBUser user);
    }
}