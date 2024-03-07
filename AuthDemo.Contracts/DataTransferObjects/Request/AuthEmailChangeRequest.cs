using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthEmailChangeRequest
    {
        [Required]
        public required long UserId { get; set; }

        [Required]
        [EmailAddress]
        public required string CurrentEmail { get; set; }

        [Required]
        [EmailAddress]
        public required string NewEmail { get; set; }
    }
}
