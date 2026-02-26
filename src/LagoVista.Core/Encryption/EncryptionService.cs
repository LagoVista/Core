using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Encryption
{
    public class EncryptionServices : IEncryptionServices
    {
        ISecureStorage _secureStorage;
        ILogger _logger;
        string _secretId;
        EntityHeader _org;
        EntityHeader _user;
        string _encryptionKey;

        public EncryptionServices(ISecureStorage secureStorage, ILogger logger, string secretId, EntityHeader org, EntityHeader user)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _secretId = secretId ?? throw new ArgumentNullException(nameof(secretId));
            _org = org ?? throw new ArgumentNullException(nameof(org));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InvokeResult> SetAccountEncryptionString()
        {
            var keyResult = await _secureStorage.GetSecretAsync(_org, _secretId, _user);
            if (!keyResult.Successful)
            {
                var key = Guid.NewGuid().ToId();
                await _secureStorage.AddSecretAsync(_org, _secretId, key);
                keyResult = await _secureStorage.GetSecretAsync(_org, _secretId, _user);
                if (!keyResult.Successful)
                    throw new ArgumentNullException($"Could not create encryption key for: {_secretId} - {keyResult.ErrorMessage}");
            }

            _encryptionKey = keyResult.Result;
            return InvokeResult.Success;
        }

        public string Encrypt(string id, decimal rate)
        {
            return Encrypt(id, (double)rate);
        }

        public string Encrypt(string id, int rate)
        {
            return Encrypt(id, (double)rate);
        }

        public string Encrypt(string id, double rate)
        {
            return Encrypt(id, rate.ToString());
        }

        public string Encrypt(string id, string str)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("-------");
                Console.WriteLine("ENCRYPT");
                Console.WriteLine("ID  : " + id);
                Console.WriteLine("STR: " + str);
                Console.WriteLine("KEY : " + _encryptionKey);
                Console.WriteLine("-------");
            }

            byte[] iv = new byte[16];
            byte[] array;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                var stringToEncrypt = $"{id}{str}";

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

        protected byte[] Encrypt(byte[] buffer)
        {
            if (Debugger.IsAttached || true)
            {
                Console.WriteLine("-------");
                Console.WriteLine("ENCRYPT");
                Console.WriteLine("BUFF SIZE  : " + buffer.Length);
                Console.WriteLine("KEY : " + _encryptionKey);
                Console.WriteLine("-------");
            }

            byte[] iv = new byte[16];
            byte[] array;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(buffer, 0, buffer.Length);
                        cryptoStream.FlushFinalBlock();
                        array = memoryStream.ToArray();
                    }
                }
            }

            if (Debugger.IsAttached || true)
            {
                Console.WriteLine("-------");
                Console.WriteLine("ENCRYPT");
                Console.WriteLine("ENCRYPTED SIZE  : " + array.Length);
                Console.WriteLine("KEY : " + _encryptionKey);
                Console.WriteLine("-------");
            }


            return array;
        }

        private byte[] BuildBufferAsync(Stream stream, int chunkSize = 4096)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));

            var buffer = new List<byte>();
            var chunk = new byte[chunkSize];
            int bytesRead;

            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer.Add(chunk[i]);
                    }
                }
            }

            return buffer.ToArray();
        }

        protected byte[] Decrypt(byte[] buffer)
        {
            if (Debugger.IsAttached || true)
            {
                Console.WriteLine("-------");
                Console.WriteLine("DECRYPT");
                Console.WriteLine("ORIG BUFFER: " + buffer.Length);
                Console.WriteLine("KEY : " + _encryptionKey);
                Console.WriteLine("-------");
            }

            byte[] iv = new byte[16];

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (var memoryStream = new MemoryStream(buffer))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        using (var cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            return BuildBufferAsync(cryptoStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.AddException("[EncryptedManagerBase__Decrypt]", ex, GetType().Name.ToKVP("type"));
                return null;
            }
        }


        public decimal Decrypt(string id, string rate)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("-------");
                Console.WriteLine("DECRYPT");
                Console.WriteLine("ID  : " + id);
                Console.WriteLine("RATE: " + rate);
                Console.WriteLine("KEY : " + _encryptionKey);
                Console.WriteLine("-------");
            }

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(rate);

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                var plainText = streamReader.ReadToEnd();
                                var number = "-";
                                try
                                {
                                    number = plainText.Replace(id, String.Empty);
                                    return Convert.ToDecimal(number.Trim());
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("PLAIN TEXT: " + plainText);
                                    Console.WriteLine("NUBMER: " + number);

                                    throw;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.AddException("[EncryptedManagerBase__Decrypt]", ex, id.ToKVP("id"), GetType().Name.ToKVP("type"), _encryptionKey.Substring(0, 5).ToKVP("key"));
                return -999.99m;
            }
        }

        public string DecryptString(string id, string value)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(value);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            var plainText = streamReader.ReadToEnd();
                            return plainText.Replace(id, String.Empty);
                        }
                    }
                }
            }
        }
    }
}
