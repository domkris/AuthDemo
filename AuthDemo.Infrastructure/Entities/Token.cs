using AuthDemo.Infrastructure.Audit;

namespace AuthDemo.Infrastructure.Entities
{
    public class Token : BaseEntity, IAuditableEntity
    {
        public virtual long? UserId { get; set; }
        public virtual User? User { get; set; }
        public string JwtAccessTokenId { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public string? ReplacedByRefreshToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired && !string.IsNullOrEmpty(ReplacedByRefreshToken);
        public virtual long? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual long? UpdatedById { get; set; }
        public virtual User? UpdatedBy { get; set; }
        public required virtual DateTime? CreatedAt { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
    }
}
