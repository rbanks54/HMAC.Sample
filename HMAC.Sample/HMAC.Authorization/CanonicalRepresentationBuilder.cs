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
        /// HTTP METHOD + | +
        /// Content-MD5 + | + 
        /// Timestamp + | +
        /// UserId + | +
        /// Request URI
        /// </summary>
        /// <returns></returns>
        public string BuildRequestRepresentation(HttpRequestMessage requestMessage, string userId)
        {
            if (!requestMessage.Headers.Date.HasValue && !requestMessage.Headers.Contains("X-Date"))
                return null;

            DateTime date;
            if (requestMessage.Headers.Date.HasValue)
            {
                date = requestMessage.Headers.Date.Value.UtcDateTime;
            }
            else
            {
                var success = DateTime.TryParse(requestMessage.Headers.GetValues("X-Date").First(), CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out date);
                if (!success)
                    return null;
            }

            var md5 = requestMessage.Content == null ||
                requestMessage.Content.Headers.ContentMD5 == null ?  "" 
                : Convert.ToBase64String(requestMessage.Content.Headers.ContentMD5);

            var httpMethod = requestMessage.Method.Method;
            var uri = requestMessage.RequestUri.AbsolutePath.ToLower();
            var representation = String.Join("|", httpMethod,
                md5, date.ToString(CultureInfo.InvariantCulture),
                userId, uri);
            
            return representation;
        }
    }
}