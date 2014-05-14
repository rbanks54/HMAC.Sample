namespace HMAC.Authorization
{
    public interface ICalculateSignature
    {
        string Signature(string hashedApiKey, string messageRepresentation);
    }
}