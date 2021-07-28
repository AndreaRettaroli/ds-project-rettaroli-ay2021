using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using AWSServerlessApplication.Services;
using AWSServerlessApplication.Services.Interfaces;
using AWSServerlessApplication.Utils;

namespace AWSServerlessApplication.Authentication
{
    public class JwtBearerManager
    {
        private readonly string _region;
        private readonly string _userPoolId;
        private readonly string[] _audiences;

        public JwtBearerManager(string userPoolId, params string[] audiences)
        {
            _region = userPoolId.Split('_')[0];
            _userPoolId = userPoolId;
            _audiences = audiences;
        }

        public void SetJwtBearerOptions(JwtBearerOptions options)
        {
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = TokenValidated,
            };
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    // get JsonWebKeySet from AWS
                    var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
                    // serialize the result
                    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                    // cast the result to be the type expected by IssuerSigningKeyResolver
                    return keys;
                },
                NameClaimType = CustomClaimTypes.CognitoUsername,
                ValidIssuer = $"https://cognito-idp.{_region}.amazonaws.com/{_userPoolId}",
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidAudiences = _audiences,
                ValidateAudience = true
            };
        }

        public async Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            Console.WriteLine(context.Exception.ToString());
            await Task.FromResult(true);
        }

        public async Task TokenValidated(TokenValidatedContext context)
        {
            var identity = context.Principal.Identities.First();

            var userService = context.HttpContext.RequestServices.GetService(typeof(IUsersService)) as UsersService;
            var authService = context.HttpContext.RequestServices.GetService(typeof(IAuthService)) as AuthService;
            var accessToken = context.SecurityToken as JwtSecurityToken;

            var user = await userService.GetAsync(identity.Name);
            if (user != null)
            {
                user.Token = accessToken.RawData;
                authService.Init(user);
            }

            await Task.FromResult(true);
        }
    }
}