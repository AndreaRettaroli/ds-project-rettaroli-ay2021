using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Models
{
    public class DynamoDBUser
    {
        public  string Id { get; set; }
        public  string Name { get; set; }
        public  string Surname { get; set; }
        public  string DisplayName { get; set; }
        public  string Email { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public  string ImageUrl { get; set; }
        public string Token { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
