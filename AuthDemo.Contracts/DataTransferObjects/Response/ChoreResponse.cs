
namespace AuthDemo.Contracts.DataTransferObjects.Response
{
    public class ChoreResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public UserResponse? CreatedBy { get; set; }
        public UserResponse? UserAssignee { get; set; }
    }
}
