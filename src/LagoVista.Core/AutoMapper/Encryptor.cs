using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.AutoMapper
{
    public class Encryptor : IEncryptor
    {
        public string Decrypt(string salt, string ciphertext, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(ciphertext);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            var plainText = streamReader.ReadToEnd();
                            Console.WriteLine($"[DECRYPTED] {plainText}");
                            return plainText.Replace(salt, String.Empty);
                        }
                    }
                }
            }
        }

        public string Encrypt(string salt, string plaintext, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var stringToEncrypt = $"{salt}{plaintext}";

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(stringToEncrypt);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}
