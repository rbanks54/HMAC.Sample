using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HMAC.Authorization
{
    public class HmacSigningHandler : HttpClientHandler
    {
        private readonly IApiKeyRepository apiKeyRepository;
        private readonly IBuildMessageRepresentation representationBuilder;
        private readonly ICalculateSignature signatureCalculator;

        public string UserId { get; set; }

        public HmacSigningHandler(IApiKeyRepository apiKeyRepository,
                              IBuildMessageRepresentation representationBuilder,
                              ICalculateSignature signatureCalculator)
        {
            this.apiKeyRepository = apiKeyRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
        }

        protected  override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                System.Threading.CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.ApiKeyHeader))
            {
                request.Headers.Add(Configuration.ApiKeyHeader, UserId);
            }
            request.Headers.Date = new DateTimeOffset(DateTime.Now,DateTime.Now-DateTime.UtcNow);
            var representation = representationBuilder.BuildRequestRepresentation(request);
            var hashedApiKey = apiKeyRepository.HashedApiKeyForUser(UserId);
            string signature = signatureCalculator.Signature(hashedApiKey,representation);

            var header = new AuthenticationHeaderValue(Configuration.AuthenticationScheme, signature);

            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}