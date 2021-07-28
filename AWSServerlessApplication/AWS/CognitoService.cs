using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using AWSServerlessApplication.Utils;
using Microsoft.Extensions.Options;


namespace AWSServerlessApplication.AWS
{
    public class CognitoService : ICognitoService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoAdmin;
        private readonly IAmazonCognitoIdentityProvider _cognito;
        private readonly AppSettings _appSettings;

        public CognitoService(IOptions<AppSettings> options, IAmazonCognitoIdentityProvider cognitoAdmin)
        {
            _appSettings = options.Value;
            _cognitoAdmin = cognitoAdmin;

            var region = _appSettings.UserPoolId.Split("_").FirstOrDefault() ?? "us-east-1";
            _cognito = new AmazonCognitoIdentityProviderClient(
                new Amazon.Runtime.AnonymousAWSCredentials(),
                Amazon.RegionEndpoint.GetBySystemName(region));

        }

        public async Task<AdminCreateUserResponse> CreateAsync(string email)
        {
            var request = new AdminCreateUserRequest
            {
                UserPoolId = _appSettings.UserPoolId,
                Username = email,
                TemporaryPassword = PasswordUtils.Generate(),
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "email_verified", Value = "true" }
                },
            };
            
            var task = await _cognitoAdmin.AdminCreateUserAsync(request);
            await SetUserPasswordAsync(email, request.TemporaryPassword);
            
            return task;
        }

        public async Task<AdminSetUserPasswordResponse> SetUserPasswordAsync(string email, string password)
        {
            var request = new AdminSetUserPasswordRequest
            {
                UserPoolId = _appSettings.UserPoolId,
                Username = email,
                Password = password,
                Permanent = true
            };

            return await _cognitoAdmin.AdminSetUserPasswordAsync(request);
        }

        public async Task InitForgotPasswordAsync(string email)
        {
            var request = new Amazon.CognitoIdentityProvider.Model.ForgotPasswordRequest
            {
                ClientId = _appSettings.UserPoolClientId,
                Username = email
            };

            await _cognito.ForgotPasswordAsync(request);
        }

        public async Task ChangeForgottenPassword(AWSServerlessApplication.Models.ForgotPasswordRequest passwordRequest)
        {
            var request = new ConfirmForgotPasswordRequest
            {
                ClientId = _appSettings.UserPoolClientId,
                Username = passwordRequest.UserId,
                ConfirmationCode = passwordRequest.VerificationCode,
                Password = passwordRequest.NewPassword
            };

            await _cognito.ConfirmForgotPasswordAsync(request);
        }

        public async Task AdminDisableUserAsync(string email)
        {
            AdminDisableUserRequest adminDisableUserRequest = new AdminDisableUserRequest
            {
                Username = email,
                UserPoolId = _appSettings.UserPoolId
            };

            await _cognitoAdmin.AdminDisableUserAsync(adminDisableUserRequest);
        }

    }
}