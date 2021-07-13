using AWSServerlessApplication.Models;
using AWSServerlessApplication.Services.Interfaces;

using System.Threading.Tasks;

namespace AWSServerlessApplication.Services
{
    public class UsersService : IUsersService
    {
        public Task<User> GetAsync(string name)
        {
            return null;
        }
    }
}
