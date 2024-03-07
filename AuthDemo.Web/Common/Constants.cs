namespace AuthDemo.Web.Common
{
    public struct Constants
    {
        public struct ReasonsOfRevoke
        {
            public const string UserRequestedLogout = "User requested Logout";
            public const string UserRequestedInvalidationOfUserTokens = "User requested invalidation of user's Tokens";
            public const string UserChangedPassword = "User changed Password";
            public const string UserChangedEmail = "User changed Email";
            public const string AdminChangedUserRole = "Admin changed user's Role";
            public const string AdminRequestedInvalidationOfUserTokens = "Admin requested invalidation of user's Tokens";
            public const string UserDeactivated = "User deactivated";
        }
    }
}
