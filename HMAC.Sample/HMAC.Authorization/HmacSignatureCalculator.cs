using System;
using System.Security.Cryptography;
using System.Text;

namespace HMAC.Authorization
{
    public class HmacSignatureCalculator : ICalculateSignature
    {
        public string Signature(string hashedApiKey, string messageRepresentation)
        {
            var key = Encoding.UTF8.GetBytes(hashedApiKey);
            var message = Encoding.UTF8.GetBytes(messageRepresentation);
            string signature;

            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(message);
                signature = Convert.ToBase64String(hash);
            }

            return signature;
        }
    }
}