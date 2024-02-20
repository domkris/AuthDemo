using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthUserEmailChangeRequest
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
