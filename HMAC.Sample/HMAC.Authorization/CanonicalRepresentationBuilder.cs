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
        public string BuildRequestRepresentation(HttpRequestMessage requestMessage, string userId)
        {
            var valid = IsRequestValid(requestMessage);
            if (!valid)
            {
                return null;
            }

            //We check if the date header has a value in the IsRequestValid method
            var date = requestMessage.Headers.Date.Value.UtcDateTime;

            var md5 = requestMessage.Content == null ||
                requestMessage.Content.Headers.ContentMD5 == null ?  "" 
                : Convert.ToBase64String(requestMessage.Content.Headers.ContentMD5);

            var httpMethod = requestMessage.Method.Method;
            var uri = requestMessage.RequestUri.AbsolutePath.ToLower();
            var representation = String.Join("\n", httpMethod,
                md5, date.ToString(CultureInfo.InvariantCulture),
                userId, uri);
            
            return representation;
        }

        private bool IsRequestValid(HttpRequestMessage requestMessage)
        {
            if (!requestMessage.Headers.Contains(Configuration.ApiKeyHeader))
                return false;

            if (!requestMessage.Headers.Date.HasValue)
                return false;

            return true;
        }
    }
}