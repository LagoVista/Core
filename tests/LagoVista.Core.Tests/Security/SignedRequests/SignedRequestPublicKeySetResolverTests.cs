using LagoVista.Core.Security;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestPublicKeySetResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnMatchingPublicKey()
        {
            var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet());

            var key = resolver.Resolve("api", "api-live-202605", SignedRequestSignatureAlgorithms.RsaPssSha256);

            Assert.That(key, Is.Not.Null);
            Assert.That(key.AppKey, Is.EqualTo("api"));
            Assert.That(key.KeyId, Is.EqualTo("api-live-202605"));
            Assert.That(key.Algorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
            Assert.That(key.KeyMaterialFormat, Is.EqualTo(SignedRequestKeyMaterialFormats.RsaXml));
            Assert.That(key.PublicKeyMaterial, Is.EqualTo("public-key-api"));
            Assert.That(key.Status, Is.EqualTo(SignedRequestValidationKeyStatuses.Active));
        }

        [Test]
        public void Resolve_ShouldMatchCallerKeyAndAlgorithmCaseInsensitively()
        {
            var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet());

            var key = resolver.Resolve("API", "API-LIVE-202605", "RSA-PSS-SHA256");

            Assert.That(key, Is.Not.Null);
            Assert.That(key.AppKey, Is.EqualTo("api"));
            Assert.That(key.KeyId, Is.EqualTo("api-live-202605"));
        }

        [Test]
        public void Resolve_ShouldReturnRevokedKeySoValidatorCanFailClosedWithStatusReason()
        {
            var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet());

            var key = resolver.Resolve("api", "api-live-202604", SignedRequestSignatureAlgorithms.RsaPssSha256);

            Assert.That(key, Is.Not.Null);
            Assert.That(key.Status, Is.EqualTo(SignedRequestValidationKeyStatuses.Revoked));
        }

        [Test]
        public void Resolve_ShouldReturnNullWhenKeyIsNotFound()
        {
            var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet());

            var key = resolver.Resolve("api", "api-live-209912", SignedRequestSignatureAlgorithms.RsaPssSha256);

            Assert.That(key, Is.Null);
        }

        [Test]
        public void Update_ShouldReplaceCurrentKeySet()
        {
            var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet());

            resolver.Update(new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = "2026-05-26.002",
                GeneratedUtc = DateTimeOffset.UtcNow,
                Keys =
                {
                    new SignedRequestPublicKeyEntry
                    {
                        AppKey = "mcp",
                        KeyId = "mcp-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = "public-key-mcp",
                        Status = SignedRequestValidationKeyStatuses.Active
                    }
                }
            });

            Assert.That(resolver.Resolve("api", "api-live-202605", SignedRequestSignatureAlgorithms.RsaPssSha256), Is.Null);

            var key = resolver.Resolve("mcp", "mcp-live-202605", SignedRequestSignatureAlgorithms.RsaPssSha256);
            Assert.That(key, Is.Not.Null);
            Assert.That(key.PublicKeyMaterial, Is.EqualTo("public-key-mcp"));
            Assert.That(resolver.Current.Version, Is.EqualTo("2026-05-26.002"));
        }

        private static SignedRequestPublicKeySet CreateKeySet()
        {
            return new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = "2026-05-26.001",
                GeneratedUtc = DateTimeOffset.UtcNow,
                Keys =
                {
                    new SignedRequestPublicKeyEntry
                    {
                        AppKey = "api",
                        KeyId = "api-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = "public-key-api",
                        Status = SignedRequestValidationKeyStatuses.Active
                    },
                    new SignedRequestPublicKeyEntry
                    {
                        AppKey = "api",
                        KeyId = "api-live-202604",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = "public-key-api-old",
                        Status = SignedRequestValidationKeyStatuses.Revoked
                    }
                }
            };
        }
    }
}
