using System;
using Amazon.Extensions.CognitoAuthentication;

namespace AWSServerlessApplication.Authentication
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Username { get; set; }
        public CognitoToken CognitoToken { get; set; }
        public AuthenticationError Error { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }

        public AuthenticationResult()
        {
        }

        public AuthenticationResult(CognitoUser user, AuthFlowResponse response)
        {
            Success = true;
            Username = user.Username;
            CognitoToken = new CognitoToken
            {
                IdToken = response.AuthenticationResult.IdToken,
                AccessToken = response.AuthenticationResult.AccessToken,
                RefreshToken = response.AuthenticationResult.RefreshToken
            };
        }

        public AuthenticationResult(AuthenticationError error, string username)
        {
            Success = false;
            Error = error;
            Message = GetMessage(error, username);
        }

        private string GetMessage(AuthenticationError error, string username)
        {
            return error switch
            {
                AuthenticationError.NEW_PASSWORD_REQUIRED => "You can't sign in with the default password",
                AuthenticationError.USER_NOT_AUTHORIZED => "Incorrect Email or Password",
                AuthenticationError.USER_NOT_FOUND => $"Username {username} does not exists",
                AuthenticationError.NOT_PERMITTED => $"Password already updated",
                _ => $"Username {username} does not exists"
            };
        }
    }
}