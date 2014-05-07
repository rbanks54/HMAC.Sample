using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HMAC.Authorization
{
    public class SecretRepository : ISecretRepository
    {
        private readonly IDictionary<string, string> repoData
            = new Dictionary<string, string>()
                  {
                      {"1234","v87o2jh388d"},
                      {"89s8i2k","8ds7fgwnlksaas"}
                  };

        public string GetSecretForUser(string apiKey)
        {
            if (!repoData.ContainsKey(apiKey))
            {
                return null;
            }

            var secret = repoData[apiKey];
            var hashed = ComputeHash(secret, new SHA1CryptoServiceProvider());
            return hashed;
        }

        //TODO: Workout why we hash the secrets? (used to be username/password, so maybe that's why)
        private string ComputeHash(string inputData, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashed = algorithm.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

    }
}