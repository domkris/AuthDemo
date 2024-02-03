﻿using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Domain.Identity.Interfaces
{
    public interface IUserIdentityService : IIdentityService<User, long>
    {
    }
}
