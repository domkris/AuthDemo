using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class AuthTokenRemoveRequest
    {
        [Required]
        public required long UserId { get; set; }

        [Required]
        public required string TokenId { get; set; }
    }
}
