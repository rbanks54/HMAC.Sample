using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HMAC.Authorization
{
    public class ResponseContentMd5Handler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(content);
                response.Content.Headers.ContentMD5 = hash;
            }
            return response;
        }
    }
}