namespace HMAC.Authorization
{
    public interface ISecretRepository
    {
        string GetSecretForUser(string username);
    }
}