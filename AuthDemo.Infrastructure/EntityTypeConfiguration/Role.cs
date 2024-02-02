using AuthDemo.Infrastructure.LookupData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthDemo.Infrastructure.EntityTypeConfiguration
{
    internal sealed class Role : IEntityTypeConfiguration<Entities.Role>
    {
        public void Configure(EntityTypeBuilder<Entities.Role> builder)
        {
            builder.HasIndex(entity => entity.Name).IsUnique();
            builder.HasIndex(entity => entity.NormalizedName).IsUnique();
            builder.Property(entity => entity.Name).IsRequired();
            builder.Property(entity => entity.NormalizedName).IsRequired();
            builder.HasMany(entity => entity.Users)
                .WithOne(entity => entity.Role);
            builder.HasData(SeedData());
        }

        private static IEnumerable<Entities.Role> SeedData()
        {
            return Enum.GetValues(typeof(Roles))
                .OfType<Roles>()
                .Select(role => new Entities.Role
                {
                    Id = (long) role,
                    Name = role.ToString(),
                    NormalizedName = role.ToString().ToUpperInvariant()
                });
        }
    }
}
