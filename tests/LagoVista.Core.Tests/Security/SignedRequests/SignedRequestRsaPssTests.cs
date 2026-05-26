using LagoVista.Core.Security;
using NUnit.Framework;
using System;
using System.Security.Cryptography;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestRsaPssTests
    {
        [Test]
        public void ServiceHttpV1_ShouldSignAndValidateWithRsaPssSha256()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;

                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);

                var headerBuilder = new SignedRequestHeaderBuilder();
                var headers = headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    CallerId = "api",
                    RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    SigningKeyId = "api-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work?x=1",
                    Body = new byte[] { 1, 2, 3 }
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work?x=1",
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        CallerId = "api",
                        KeyId = "api-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    })
                });

                Assert.That(result.Successful, Is.True, result.ErrorMessage);
                Assert.That(result.MatchedKeyId, Is.EqualTo("api-live-202605"));
                Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
                Assert.That(headers[SignedRequestHeaders.SignatureAlgorithm], Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
                Assert.That(headers[SignedRequestHeaders.KeyMaterialFormat], Is.EqualTo(SignedRequestKeyMaterialFormats.RsaXml));
            }
        }

        [Test]
        public void ServiceHttpV1_ShouldRejectRevokedPublicKey()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;

                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);

                var headerBuilder = new SignedRequestHeaderBuilder();
                var headers = headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    CallerId = "api",
                    RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    SigningKeyId = "api-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    Body = new byte[] { 1, 2, 3 }
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        CallerId = "api",
                        KeyId = "api-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Revoked
                    })
                });

                Assert.That(result.Successful, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo("validation_key_not_active"));
            }
        }

        [Test]
        public void ServiceHttpV1_ShouldRejectWrongPublicKey()
        {
            using (var signingRsa = RSA.Create())
            using (var wrongRsa = RSA.Create())
            {
                signingRsa.KeySize = 3072;
                wrongRsa.KeySize = 3072;

                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(signingRsa.ExportParameters(true), true);
                var wrongPublicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(wrongRsa.ExportParameters(false), false);

                var headerBuilder = new SignedRequestHeaderBuilder();
                var headers = headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    CallerId = "api",
                    RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    SigningKeyId = "api-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    Body = new byte[] { 1, 2, 3 }
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        CallerId = "api",
                        KeyId = "api-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = wrongPublicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    })
                });

                Assert.That(result.Successful, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo("invalid_signature"));
            }
        }

        [Test]
        public void ServiceHttpV1_ShouldRejectMismatchedKeyId()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;

                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);

                var headerBuilder = new SignedRequestHeaderBuilder();
                var headers = headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    CallerId = "api",
                    RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    SigningKeyId = "api-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    Body = new byte[] { 1, 2, 3 }
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work",
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        CallerId = "api",
                        KeyId = "api-live-202606",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    })
                });

                Assert.That(result.Successful, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo("validation_key_not_found"));
            }
        }

        [Test]
        public void RuntimeInstanceHttpV1_ShouldContinueToUseHmacSha256()
        {
            var headerBuilder = new SignedRequestHeaderBuilder();
            var headers = headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceHttpV1,
                Key = "runtime-instance-private-key",
                RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = "7D2D3457FD0D4713AD283B6734C13A53",
                Organization = "Fred's Fish",
                UserId = "7D2D3457FD0D4713AD283B6734C13A5E",
                User = "Fred",
                InstanceId = "59B328E9E6D249999BA864CE153B5F5E",
                Instance = "Fish Runtime",
                Method = "POST",
                PathAndQuery = "/api/runtime/messages",
                Body = new byte[] { 4, 5, 6 }
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceHttpV1,
                Headers = headers,
                Key1 = "runtime-instance-private-key",
                Method = "POST",
                PathAndQuery = "/api/runtime/messages"
            });

            Assert.That(result.Successful, Is.True, result.ErrorMessage);
            Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.HmacSha256));
        }

        private class StaticValidationKeyResolver : ISignedRequestValidationKeyResolver
        {
            private readonly SignedRequestValidationKey _key;

            public StaticValidationKeyResolver(SignedRequestValidationKey key)
            {
                _key = key;
            }

            public SignedRequestValidationKey Resolve(string callerId, string keyId, string algorithm)
            {
                if (String.Equals(_key.CallerId, callerId, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(_key.KeyId, keyId, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(SignedRequestSignatureAlgorithms.Normalize(_key.Algorithm), SignedRequestSignatureAlgorithms.Normalize(algorithm), StringComparison.OrdinalIgnoreCase))
                {
                    return _key;
                }

                return null;
            }
        }
    }
}
