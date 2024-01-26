using AuthDemo.Infrastructure.LookupData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
    
}

