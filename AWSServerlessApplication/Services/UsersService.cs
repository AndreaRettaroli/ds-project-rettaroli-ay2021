using Amazon.DynamoDBv2;
using AWSServerlessApplication.Authentication;
using AWSServerlessApplication.AWS;
using AWSServerlessApplication.Extensions;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Services.DynamoDBRequests;
using AWSServerlessApplication.Services.Interfaces;
using AWSServerlessApplication.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Model;
using Amazon.Lambda;
using System.Text.Json;

namespace AWSServerlessApplication.Services
{
    public class UsersService : IUsersService
    {
        private readonly IAuthService _authService;
        private readonly ICognitoService _cognito;
        private readonly IAmazonDynamoDB _dynamoDB;
        private readonly IAmazonLambda _amazonLambda;

        private readonly string _tableName;
        public UsersService(IAuthService authService,
            IAmazonDynamoDB dynamoDB,
            ICognitoService cognito,
            IAmazonLambda amazonLambda)
        {
            _authService = authService;
            _dynamoDB = dynamoDB;
            _cognito = cognito;
            _amazonLambda = amazonLambda;
            _tableName = UsersTable.TableName;

        }
        public async Task<User> CreateAsync(DynamoDBUser user)
        {
            try
            {
                var response = await _cognito.CreateAsync(user.Email);
                user.Id = response.User.Username;
                var request = UserRequest.Insert(user);
                await _dynamoDB.PutItemAsync(request);
                return request.Item.ToDynamoDBUser().ToUser();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var request = UserRequest.DeleteAsync(id);
                var response = await _dynamoDB.UpdateItemAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User> GetAsync(string id)
        {
            try
            {
                var result = await _dynamoDB.GetItemAsync(_tableName, id.ToDynamoDBKey(UsersTable.Id));
                return result.Item.ToDynamoDBUser().ToUser();
            }
            catch
            {
                return null; 
            }
        }

        public async Task<List<User>> ListAsync()
        {
            var response= await _amazonLambda.InvokeAsync(new InvokeRequest
            {
              FunctionName = "sd-dev-GetUsersLambdaFunction",

            });
            var users = await JsonSerializer.DeserializeAsync<List<User>>(response.Payload);
            if (users.Count > 0)
                return users;
            return new List<User>();
                          
        }

        public async Task<User> SetPasswordAsync(Credentials credentials)
        {
            try
            {
                var now = DateTime.UtcNow;
                var user = _authService.AuthUser;
                var response = await _cognito.SetUserPasswordAsync(user.Email, credentials.Password);
                var request = UserRequest.Confirm(user.Id, now);
                await _dynamoDB.UpdateItemAsync(request);
                user.ConfirmedAt = now.TruncateToSecondStart();
                return user.ToUser();
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> UpdateAsync(DynamoDBUser user)
        {
            try
            {
                var currentUser = await GetAsync(user.Id);
                if (currentUser == null || currentUser.Deleted != null)
                    return null;
                user.FillUpdate(currentUser);
                var request = UserRequest.Insert(user);
                await _dynamoDB.PutItemAsync(request);
                return request.Item.ToDynamoDBUser().ToUser();
            }
            catch
            {
                return null;
            }
        }
    }
}
