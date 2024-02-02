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
