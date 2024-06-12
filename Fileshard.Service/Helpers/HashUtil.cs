using System.IO;
using System.Security.Cryptography;

namespace Fileshard.Frontend.Helpers
{
    public class HashUtil
    {
        public static string ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
