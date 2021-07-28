using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using AWSServerlessApplication.Utils;
using Microsoft.Extensions.Options;


namespace AWSServerlessApplication.Authentication
{
    public class CognitoAuthentication
    {
        private readonly RegionEndpoint _region;
        private readonly AppSettings _appSettings;

        public CognitoAuthentication(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            var regionString = _appSettings.UserPoolId.Split("_").FirstOrDefault() ?? "eu-west-1";
            _region = RegionEndpoint.GetBySystemName(regionString);
        }

        public async Task<AuthenticationResult> SignInAsync(Credentials credentials)
        {
            try
            {
                using var cognito = new AmazonCognitoIdentityProviderClient(
                    new Amazon.Runtime.AnonymousAWSCredentials(),
                    _region);
                var (user, startResponse) = await StartSrpAuthAsync(cognito, credentials);
                return startResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED
                    ? new AuthenticationResult(user, await NewPasswordRequiredAsync(credentials, user, startResponse))
                    : new AuthenticationResult(user, startResponse);
            }
            catch (NotAuthorizedException)
            {
                return new AuthenticationResult(AuthenticationError.USER_NOT_AUTHORIZED, credentials.Email);
            }
            catch (UserNotFoundException)
            {
                return new AuthenticationResult(AuthenticationError.USER_NOT_FOUND, credentials.Email);
            }
        }

        private async Task<(CognitoUser, AuthFlowResponse)> StartSrpAuthAsync(AmazonCognitoIdentityProviderClient cognito, Credentials credentials)
        {
            var userPool = new CognitoUserPool(_appSettings.UserPoolId, _appSettings.UserPoolClientId, cognito);
            var user = new CognitoUser(credentials.Email, _appSettings.UserPoolClientId, userPool, cognito);
            var response = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest
            {
                Password = credentials.Password
            });
            return (user, response);
        }

        private static async Task<AuthFlowResponse> NewPasswordRequiredAsync(Credentials credentials, CognitoUser user, AuthFlowResponse authResponse)
        {
            return await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest
            {
                SessionID = authResponse.SessionID,
                NewPassword = credentials.Password
            });
        }
    }
}