using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class ChoreRequest
    {
        [Required]
        [MinLength(3)]
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
