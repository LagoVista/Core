using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Rpc.Tests.Server.Utils
{
    internal class AzureSaSBuilder
    {
        public static string BuildSignature(
            string fullyQualifiedNamespace,
            string entityName,
            string sharedAccessKeyName,
            string sharedAccessKey,
            DateTimeOffset expirationTime)
        {
            if (string.IsNullOrEmpty(fullyQualifiedNamespace))
            {
                throw new ArgumentException($" {nameof(fullyQualifiedNamespace)} cannot be null or empty.", nameof(fullyQualifiedNamespace));
            }

            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentException($" {nameof(entityName)} cannot be null or empty.", nameof(entityName));
            }

            if (string.IsNullOrEmpty(fullyQualifiedNamespace))
            {
                throw new ArgumentException($" {nameof(sharedAccessKeyName)} cannot be null or empty.", nameof(sharedAccessKeyName));
            }

            if (string.IsNullOrEmpty(fullyQualifiedNamespace))
            {
                throw new ArgumentException($" {nameof(sharedAccessKey)} cannot be null or empty.", nameof(sharedAccessKey));
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedAccessKey));

            var encodedAudience = WebUtility.UrlEncode(BuildAudience(fullyQualifiedNamespace, entityName));
            var expiration = Convert.ToString(ConvertToUnixTime(expirationTime), CultureInfo.InvariantCulture);
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{encodedAudience}\n{expiration}")));

            return string.Format(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}&{7}={8}",
                "SharedAccessSignature",
                "sr",
                encodedAudience,
                "sig",
                WebUtility.UrlEncode(signature),
                "se",
                WebUtility.UrlEncode(expiration),
                "skn",
                WebUtility.UrlEncode(sharedAccessKeyName));
        }

        private static string BuildAudience(
            string fullyQualifiedNamespace,
            string entityName)
        {
            var builder = new UriBuilder(fullyQualifiedNamespace)
            {
                Scheme = "amqps",
                Path = entityName,
                Port = -1,
                Fragment = string.Empty,
                Password = string.Empty,
                UserName = string.Empty,
            };

            builder.Path = builder.Path.TrimEnd('/');
            return builder.Uri.AbsoluteUri.ToLowerInvariant();
        }

        private static long ConvertToUnixTime(DateTimeOffset dateTimeOffset) =>
            Convert.ToInt64((dateTimeOffset - Epoch).TotalSeconds);

        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
}
