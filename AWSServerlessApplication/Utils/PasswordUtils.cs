using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Utils
{
    public class PasswordUtils
    {
        public static string Generate()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            return "-"+Convert.ToBase64String(bytes).Replace('+', '_').Replace('/', '-');
        }
    }
}
