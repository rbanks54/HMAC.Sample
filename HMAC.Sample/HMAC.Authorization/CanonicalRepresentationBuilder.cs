using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace HMAC.Authorization
{
    public class CanonicalRepresentationBuilder : IBuildMessageRepresentation
    {
        /// <summary>
        /// Builds message representation as follows:
        /// HTTP METHOD\n +
        /// Content-MD5\n +  
        /// Timestamp\n +
        /// UserId\n +
        /// Request URI
        /// </summary>
        /// <returns></returns>
        public string BuildRequestRepresentation(HttpRequestMessage requestMessage)
        {
            bool valid = IsRequestValid(requestMessage);
            if (!valid)
            {
                return null;
            }

            if (!requestMessage.Headers.Date.HasValue)
            {
                return null;
            }
            var date = requestMessage.Headers.Date.Value.UtcDateTime;

            var md5 = requestMessage.Content == null ||
                requestMessage.Content.Headers.ContentMD5 == null ?  "" 
                : Convert.ToBase64String(requestMessage.Content.Headers.ContentMD5);

            var httpMethod = requestMessage.Method.Method;
            var userId = requestMessage.Headers.Authorization.Parameter.Split(':')[0];
            var uri = requestMessage.RequestUri.AbsolutePath.ToLower();
            // you may need to add more headers if thats required for security reasons
            var representation = String.Join("\n", httpMethod,
                md5, date.ToString(CultureInfo.InvariantCulture),
                userId, uri);
            
            return representation;
        }

        private bool IsRequestValid(HttpRequestMessage requestMessage)
        {
            //for simplicity I am omitting headers check (all required headers should be present)

            return true;
        }
    }
}