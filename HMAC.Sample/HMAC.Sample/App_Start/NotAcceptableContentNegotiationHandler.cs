using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace HMAC.Sample
{

    //When a request is made for a content type we don't accept we want to return a 
    //response of 406 instead of defaulting to a JSON response. The client app should know better :-) 
    public class NotAcceptableContentNegotiationHandler : DelegatingHandler
    {
        private readonly HttpConfiguration configuration;

        public NotAcceptableContentNegotiationHandler(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            this.configuration = configuration;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var acceptHeader = request.Headers.Accept;

            // from RFC:
            // If no Accept header field is present, then it is assumed that the client accepts all media types. 
            if (!acceptHeader.Any())
            {
                return base.SendAsync(request, cancellationToken);
            }

            var hasFormatterForRequestedMediaType = configuration
                .Formatters
                .Any(formatter => acceptHeader.Any(mediaType => formatter.SupportedMediaTypes.Contains(mediaType)
                  || formatter.MediaTypeMappings.Any(m => m.TryMatchMediaType(request) > 0)));

            if (!hasFormatterForRequestedMediaType)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}