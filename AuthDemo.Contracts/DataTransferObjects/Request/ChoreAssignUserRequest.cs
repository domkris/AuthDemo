using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Request
{
    public class ChoreAssignUserRequest
    {
        public required long ChoreId { get; set; }
        public required long UserId { get; set; }
    }
}
