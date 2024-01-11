﻿using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Infrastructure.Entities
{
    public class User : IdentityUser<long>, IAuditableEntity
    {
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}