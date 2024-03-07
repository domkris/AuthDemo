using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthDemo.Infrastructure.EntityTypeConfiguration
{
    internal sealed class Token : IEntityTypeConfiguration<Entities.Token>
    {
        public void Configure(EntityTypeBuilder<Entities.Token> builder)
        {
            builder.HasIndex(entity => entity.RefreshToken);
        } 
    }
}
