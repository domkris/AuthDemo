using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Response
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
