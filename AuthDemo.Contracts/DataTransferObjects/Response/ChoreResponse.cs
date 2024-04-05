
namespace AuthDemo.Contracts.DataTransferObjects.Response
{
    public class ChoreResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsFinished { get; set; }
        public bool IsApproved { get; set; }
        public SimpleUserResponse? CreatedBy { get; set; }
        public SimpleUserResponse? UpdatedBy { get; set; }
        public SimpleUserResponse? UserAssignee { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
