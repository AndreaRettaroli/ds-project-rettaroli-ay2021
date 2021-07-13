using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Models
{
    public class ForgotPasswordRequest
    {
        public string UserId { get; set; }
        public string VerificationCode { get; set; }
        public string NewPassword { get; set; }
    }
}
