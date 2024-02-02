namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class ChoreAssignUserRequest
    {
        public required long ChoreId { get; set; }
        public required long UserId { get; set; }
    }
}
