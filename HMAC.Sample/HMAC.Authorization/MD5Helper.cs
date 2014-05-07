using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HMAC.Authorization
{
    public class Md5Helper
    {
        public static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (var md5 = MD5.Create())
            {
                var content = await httpContent.ReadAsByteArrayAsync();
                byte[] hash = md5.ComputeHash(content);
                return hash;
            }
        }
    }
}