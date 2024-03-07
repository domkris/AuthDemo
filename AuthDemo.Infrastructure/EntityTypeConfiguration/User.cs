using AuthDemo.Infrastructure.LookupData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using AuthDemo.Infrastructure.Utilities;

namespace AuthDemo.Infrastructure.EntityTypeConfiguration
{
    
    internal sealed class User : IEntityTypeConfiguration<Entities.User>
    {
        public void Configure(EntityTypeBuilder<Entities.User> builder)
        {
            builder.HasOne(user => user.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedById")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(user => user.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedById")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(SeedData());
        }

        private static IEnumerable<Entities.User> SeedData()
        {
            Entities.User systemUser = new()
            {
                Id = 1,
                RoleId = (long)Roles.Administrator,
                IsActive = false,
                //CreatedAt = DateTime.UtcNow,
                UserName = "system",
                NormalizedUserName = "SYSTEM",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            Entities.User adminUser = new()
            {
                Id = 2,
                RoleId = (long)Roles.Administrator,
                IsActive = true,
                CreatedById = 1,
                //CreatedAt = DateTime.UtcNow,
                UserName = "adminauthdemo",
                NormalizedUserName = "ADMINAUTHDEMO",
                Email = "admin@authdemo.com",
                NormalizedEmail = "ADMIN@AUTHDEMO.COM",
                EmailConfirmed = true,
                SecurityStamp = GetSecurityStamp(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            adminUser.PasswordHash = new PasswordHasher<Entities.User>().HashPassword(adminUser, "12345678");

            return new List<Entities.User>() 
            {
                systemUser,
                adminUser
            };
        }

        private static string GetSecurityStamp()
        {
            return SecurityStampGenerator.GenerateSecurityStamp();
        }
    }
    
}

