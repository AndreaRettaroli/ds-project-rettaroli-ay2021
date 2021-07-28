using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;


namespace AWSServerlessApplication.AWS
{
    public interface ICognitoService
    {
        Task<AdminCreateUserResponse> CreateAsync(string email);
        Task<AdminSetUserPasswordResponse> SetUserPasswordAsync(string email, string password);
        Task InitForgotPasswordAsync(string email);
        Task ChangeForgottenPassword(AWSServerlessApplication.Models.ForgotPasswordRequest passwordRequest);
        Task AdminDisableUserAsync(string email);
    }
}