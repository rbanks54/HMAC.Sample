using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace HMAC.Authorization
{
    public class HmacAuthenticationHandler : DelegatingHandler
    {
        private const string UnauthorizedMessage = "Unauthorized request";

        private readonly ISecretRepository secretRepository;
        private readonly IBuildMessageRepresentation representationBuilder;
        private readonly ICalculateSignature signatureCalculator;

        public HmacAuthenticationHandler(ISecretRepository secretRepository,
            IBuildMessageRepresentation representationBuilder,
            ICalculateSignature signatureCalculator)
        {
            this.secretRepository = secretRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
        }

        protected async Task<bool> IsAuthenticated(HttpRequestMessage requestMessage)
        {
            var isDateValid = IsDateValid(requestMessage);
            if (!isDateValid) return false;

            if (requestMessage.Headers.Authorization == null 
                || requestMessage.Headers.Authorization.Scheme != Configuration.AuthenticationScheme)
            {
                return false;
            }

            var authValue = requestMessage.Headers.Authorization.Parameter;
            var paramValues = authValue.Split(':');
            if (paramValues.Length != 2) return false;

            var username = paramValues[0];
            var requestSignature = paramValues[1];
            var secret = secretRepository.GetSecretForUser(username);
            if (secret == null)
            {
                return false;
            }

            var representation = representationBuilder.BuildRequestRepresentation(requestMessage);
            if (representation == null)
            {
                return false;
            }

            if (requestMessage.Content.Headers.ContentMD5 != null 
                && !await IsMd5Valid(requestMessage))
            {
                return false;
            }

            var signature = signatureCalculator.Signature(secret, representation);

            if(MemoryCache.Default.Contains(signature))
            {
                return false;
            }

            var result = requestSignature == signature;
            if (result == true)
            {
                MemoryCache.Default.Add(signature, username,
                                        DateTimeOffset.UtcNow.AddMinutes(Configuration.ValidityPeriodInMinutes));
            }
            return result;
        }

        private async Task<bool> IsMd5Valid(HttpRequestMessage requestMessage)
        {
            var hashHeader = requestMessage.Content.Headers.ContentMD5;
            if (requestMessage.Content == null)
            {
                return hashHeader == null || hashHeader.Length == 0;
            }
            var hash = await Md5Helper.ComputeHash(requestMessage.Content);
            return hash.SequenceEqual(hashHeader);
        }

        private bool IsDateValid(HttpRequestMessage requestMessage)
        {
            var utcNow = DateTime.UtcNow;
            var date = requestMessage.Headers.Date.Value.UtcDateTime;
            if (date >= utcNow.AddMinutes(Configuration.ValidityPeriodInMinutes)
                || date <= utcNow.AddMinutes(-Configuration.ValidityPeriodInMinutes))
            {
                return false;
            }
            return true;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var isAuthenticated = await IsAuthenticated(request);

            if (!isAuthenticated)
            {
                var response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, UnauthorizedMessage);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(
                    Configuration.AuthenticationScheme));
                return response;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}