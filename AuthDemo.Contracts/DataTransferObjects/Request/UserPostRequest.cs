using AuthDemo.Contracts.DataTransferObjects.Common;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{ 
    public class UserPostRequest
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }

        [Required]
        [Compare("Password")]
        public required string ConfirmPassword { get; set;}

        [Required]
        public required Roles Role { get; set; }
    }
}
