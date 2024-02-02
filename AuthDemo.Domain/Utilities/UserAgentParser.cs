using AuthDemo.Domain.Cache.CacheObjects;
using System.Text.RegularExpressions;

namespace AuthDemo.Domain.Utilities
{
    internal partial class UserAgentParser
    {
        public UserAgentInfo Parse(string userAgentString)
        {
            if (string.IsNullOrEmpty(userAgentString))
            {
                // Handle empty or null user-agent string
                return new UserAgentInfo
                {
                    BrowserName = "Unknown",
                    Version = "Unknown",
                    Platform = "Unknown"
                };
            }

            // Regular expressions for extracting browser name, version, and platform
            Regex browserRegex = BrowserRegex();
            Regex platformRegex = PlatformRegex();

            var browserMatch = browserRegex.Match(userAgentString);
            var platformMatch = platformRegex.Match(userAgentString);

            string browserName = browserMatch.Success ? browserMatch.Groups["browser"].Value : "Unknown";
            string browserVersion = browserMatch.Success ? browserMatch.Groups["version"].Value : "Unknown";
            string platform = platformMatch.Success ? platformMatch.Groups["platform"].Value : "Unknown";

            return new UserAgentInfo
            {
                BrowserName = browserName,
                Version = browserVersion,
                Platform = platform
            };
        }

        [GeneratedRegex(@"(?<browser>Chrome|Firefox|Safari|Edge|MSIE|Trident(?=\/))\s*(?<version>[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase, "hr-HR")]
        private static partial Regex BrowserRegex();
        [GeneratedRegex(@"\((?<platform>Windows|Macintosh|Linux)", RegexOptions.IgnoreCase, "hr-HR")]
        private static partial Regex PlatformRegex();
    }
}


