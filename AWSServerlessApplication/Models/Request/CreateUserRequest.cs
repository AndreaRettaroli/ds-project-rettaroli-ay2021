using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Models
{
    public class CreateUserRequest
    {

        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
