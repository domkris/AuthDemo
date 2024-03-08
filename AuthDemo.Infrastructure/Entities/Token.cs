using AuthDemo.Infrastructure.Audit;

namespace AuthDemo.Infrastructure.Entities
{
    public class Token : BaseEntity, IAuditableEntity
    {
        public virtual long? UserId { get; set; }
        public virtual User? User { get; set; }
        public string JwtAccessTokenId { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTimeOffset Expires { get; set; }
        public DateTimeOffset? Revoked { get; set; }
        public string? ReplacedByRefreshToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired && !string.IsNullOrEmpty(ReplacedByRefreshToken);
        public virtual long? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual long? UpdatedById { get; set; }
        public virtual User? UpdatedBy { get; set; }
        public required virtual DateTimeOffset? CreatedAt { get; set; }
        public virtual DateTimeOffset? UpdatedAt { get; set; }
    }
}
