using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class ChoreEditRequest
    {
        [Required]
        [MinLength(3)]
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
