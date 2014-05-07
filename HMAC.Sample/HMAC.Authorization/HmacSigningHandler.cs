using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HMAC.Authorization
{
    public class HmacSigningHandler : HttpClientHandler
    {
        private readonly ISecretRepository _secretRepository;
        private readonly IBuildMessageRepresentation _representationBuilder;
        private readonly ICalculateSignature _signatureCalculator;

        public string Username { get; set; }

        public HmacSigningHandler(ISecretRepository secretRepository,
                              IBuildMessageRepresentation representationBuilder,
                              ICalculateSignature signatureCalculator)
        {
            _secretRepository = secretRepository;
            _representationBuilder = representationBuilder;
            _signatureCalculator = signatureCalculator;
        }

        protected  override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                System.Threading.CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.ApiKeyHeader))
            {
                request.Headers.Add(Configuration.ApiKeyHeader, Username);
            }
            request.Headers.Date = new DateTimeOffset(DateTime.Now,DateTime.Now-DateTime.UtcNow);
            var representation = _representationBuilder.BuildRequestRepresentation(request);
            var secret = _secretRepository.GetSecretForUser(Username);
            string signature = _signatureCalculator.Signature(secret,
                representation);

            var header = new AuthenticationHeaderValue(Configuration.AuthenticationScheme, signature);

            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}