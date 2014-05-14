using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HMAC.Authorization
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly IDictionary<string, string> repoData
            = new Dictionary<string, string>()
                  {
                      {"1234","v87o2jh388d"},
                      {"89s8i2k","8ds7fgwnlksaas"}
                  };


        //We hash the API Key so it's not sent unencrypted over the wire
        //Prevents people sniffing the key and spoofing auth messages
        //It also adds a little more variability into our message signature calculation
        public string HashedApiKeyForUser(string userId)
        {
            if (!repoData.ContainsKey(userId))
            {
                return null;
            }

            var apiKey = repoData[userId];
            var hashed = ComputeHash(apiKey, new SHA1CryptoServiceProvider());
            return hashed;
        }

        private string ComputeHash(string inputData, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashed = algorithm.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

    }
}