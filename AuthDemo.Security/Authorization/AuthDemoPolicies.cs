using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Security.Authorization
{
    public static class AuthDemoPolicies
    {
        
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string AdminOrManager = "AdminOrManager";
        }
        
    }
}
