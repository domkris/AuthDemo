using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthLoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }
    }
}
