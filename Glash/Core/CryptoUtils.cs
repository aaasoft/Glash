using System.Security.Cryptography;
using System.Text;

namespace Glash.Core
{
    public class CryptoUtils
    {
        public static string ComputeMD5Hash(string input)
        {
            var buffer = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", "").ToLower();
        }

        public static string GetAnswer(string question, string password)
        {
            return ComputeMD5Hash($"{question}:{password}");
        }
    }
}
