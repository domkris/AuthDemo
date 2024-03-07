using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthPasswordChangeRequest
    {
        [Required]
        public required long UserId { get; set; }

        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public required string ConfirmNewPassword { get; set; }
    }
}
