namespace HMAC.Authorization
{
    public interface ICalculateSignature
    {
        string Signature(string secret, string value);
    }
}