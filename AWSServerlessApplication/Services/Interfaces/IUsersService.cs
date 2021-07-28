using AWSServerlessApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSServerlessApplication.Authentication;

namespace AWSServerlessApplication.Services.Interfaces
{
    public interface IUsersService
    {
        Task<User> GetAsync(string id);
        Task<List<User>> ListAsync();
        Task<User> CreateAsync(DynamoDBUser userRequest);
        Task<User> UpdateAsync(DynamoDBUser userRequest);
        Task<User> SetPasswordAsync(Credentials credentials);
        Task<bool> DeleteAsync(string id);
    }
}
