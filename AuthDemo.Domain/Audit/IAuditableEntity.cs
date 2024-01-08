namespace AuthDemo.Infrastructure.Audit
{
    public interface IAuditableEntity
    {
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

    }
}
