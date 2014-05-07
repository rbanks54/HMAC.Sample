namespace HMAC.Authorization
{
    public class Configuration
    {
        public const string ApiKeyHeader = "X-ApiAuth-Key";
        public const string AuthenticationScheme = "ApiAuth";
        public const int ValidityPeriodInMinutes = 5;
    }
}