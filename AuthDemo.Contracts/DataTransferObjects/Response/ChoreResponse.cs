using AuthDemo.Infrastructure.Audit;

namespace AuthDemo.Contracts.DataTransferObjects.Response
{
    public class ChoreResponse : IAuditableEntity
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
