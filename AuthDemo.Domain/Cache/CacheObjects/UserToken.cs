using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Domain.Cache.CacheObjects
{
    public class UserToken
    {
        public required string UserId { get; set; }
        public required string TokenId { get; set; }
        public required DateTime TokenExpiration { get; set; }
        public required TimeSpan TokenDuration { get; set; }
        public required string Token { get; set; }
    }
}
