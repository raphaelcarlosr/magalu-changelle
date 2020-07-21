using Magalu.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magalu.API.Services
{
    public class UserService
    {
        public static User Get(string username, string password)
        {
            return new User() { Username = username, Password = password };
        }
    }
}
