using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthTokenRequest
    {
        [Required]
        public required string AccessToken { get; set; }

        [Required]
        public required string RefreshToken { get; set; }
    }
}
