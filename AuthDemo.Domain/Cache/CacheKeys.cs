using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Domain.Cache
{
    public static class CacheKeys
    {
        public const string App = "AuthDemo";

        public enum CacheResources
        {
            UserToken
        }
    }
}
