using System;

namespace LagoVista.Crypto.Modern
{
    public static class Base64Url
    {
        public static string Encode(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var b64 = Convert.ToBase64String(data);
            // base64url: replace chars and remove padding
            return b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static byte[] Decode(string base64Url)
        {
            if (string.IsNullOrWhiteSpace(base64Url)) throw new ArgumentNullException(nameof(base64Url));

            var s = base64Url.Replace('-', '+').Replace('_', '/');
            // restore padding
            switch (s.Length % 4)
            {
                case 0: break;
                case 2: s += "=="; break;
                case 3: s += "="; break;
                default: throw new FormatException("Invalid base64url length.");
            }

            return Convert.FromBase64String(s);
        }
    }
}
