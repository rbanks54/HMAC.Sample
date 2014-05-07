using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace HMAC.Authorization
{
    public class ApiCredentials
    {
        public string ApiKey { get; set; }
        public string Signature { get; set; }

        public static ApiCredentials GetFromRequestHeaders(HttpRequestHeaders requestHeaders)
        {
            //this could/should be another interface for testing purposes
            var authenticationHeader = requestHeaders.Authorization;
            if (authenticationHeader.Scheme != Configuration.AuthenticationScheme)
            {
                return null;
            }
            if (requestHeaders.Authorization == null || requestHeaders.Authorization.Parameter == null
                || !requestHeaders.Authorization.Parameter.Contains(":"))
            {
                return null;
            }

            var apiKey = requestHeaders.Authorization.Parameter.Split(':')[0]; 
            if (apiKey == null)
            {
                return null;
            }

            var decodedBytes = Convert.FromBase64String(authenticationHeader.Parameter);
            var signature = Encoding.UTF8.GetString(decodedBytes);
            return new ApiCredentials()
            {
                Signature = signature,
                ApiKey = apiKey
            };
        }

        public AuthenticationHeaderValue ToAuthenticationHeader()
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Signature));
            return new AuthenticationHeaderValue(Configuration.AuthenticationScheme, encoded);
        }
    }
}