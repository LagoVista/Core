using System;
using System.Security.Cryptography;

namespace LagoVista.Core.Security
{
    public static class SignedRequestBodyHasher
    {
        public static string ComputeSha256Base64(byte[] body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(body));
            }
        }
    }
}
