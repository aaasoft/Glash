using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Utils
{
    public class CryptoUtils
    {
        public static string ComputeMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", "").ToLower();
        }

        public static string GetAnswer(string question, string password)
        {
            return ComputeMD5Hash($"{question}:{password}");
        }
    }
}
