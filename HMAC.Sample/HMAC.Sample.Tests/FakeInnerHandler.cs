using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HMAC.Sample.Tests
{
    public class FakeInnerHandler : DelegatingHandler
    {
        public HttpResponseMessage Message { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Message == null)
            {
                return base.SendAsync(request, cancellationToken);
            }
            return Task.Factory.StartNew(() => Message);
        }
    }
}