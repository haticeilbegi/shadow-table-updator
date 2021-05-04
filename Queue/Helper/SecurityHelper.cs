using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Queue.Helper
{
    public class SecurityHelper
    {
        private const string Key = "fdde0edb-0600-4862-a54e-983c25e51358";
        public static string Ecrypt(string text)
        {
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Key));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));

            string sbinary = "";
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sbinary += hashBytes[i].ToString("x2"); // hex format
            }

            return (sbinary);
        }
    }
}
