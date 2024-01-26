namespace AuthDemo.Infrastructure.Audit
{
    public interface IAuditableEntity
    {
        long? CreatedById { get; set; }
        long? UpdatedById { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

    }
}
