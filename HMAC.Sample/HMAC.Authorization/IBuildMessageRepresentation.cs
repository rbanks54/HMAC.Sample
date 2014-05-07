using System.Net.Http;

namespace HMAC.Authorization
{
    public interface IBuildMessageRepresentation
    {
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}