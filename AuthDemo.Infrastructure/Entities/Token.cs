using AuthDemo.Infrastructure.Attributes;
using AuthDemo.Infrastructure.Audit;

namespace AuthDemo.Infrastructure.Entities
{
    [ExcludeFromAuditLog]
    public class Token : BaseEntity, IAuditableEntity
    {
        public long? UserId { get; set; }
        public User? User { get; set; }
        public string JwtAccessTokenId { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTimeOffset Expires { get; set; }
        public DateTimeOffset? Revoked { get; set; }
        public string? ReplacedByRefreshToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired && !string.IsNullOrEmpty(ReplacedByRefreshToken);
        public long? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public long? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public required DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
