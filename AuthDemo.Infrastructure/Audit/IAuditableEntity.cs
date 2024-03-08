namespace AuthDemo.Infrastructure.Audit
{
    public interface IAuditableEntity
    {
        long? CreatedById { get; set; }
        long? UpdatedById { get; set; }
        DateTimeOffset? CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }

    }
}
