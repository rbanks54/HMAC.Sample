namespace HMAC.Authorization
{
    public interface IApiKeyRepository
    {
        string HashedApiKeyForUser(string userId);
    }
}