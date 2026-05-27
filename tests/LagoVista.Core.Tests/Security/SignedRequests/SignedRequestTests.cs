using LagoVista.Core.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestTests
    {
        private const string Key1 = "key-one";
        private const string Key2 = "key-two";

        [Test]
        public void RuntimeInstanceV1_BuildsCanonicalSource_InV1Order()
        {
            var headers = CreateRuntimeHeaders("REQ001", "2026-05-23T16:20:51.0000000Z");

            var canonical = SignedRequestCanonicalizer.Build(new SignedRequestCanonicalContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Headers = headers
            });

            Assert.That(canonical, Is.EqualTo("REQ001\r\n2026-05-23T16:20:51.0000000Z\r\n1\r\nORG001\r\nUSER001\r\nINSTANCE001\r\n"));
        }

        [Test]
        public void HeaderBuilder_RuntimeInstanceV1_AddsAuthorizationHeader_AndValidatorAcceptsKey1()
        {
            var builder = new SignedRequestHeaderBuilder();
            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Key = Key1,
                RequestId = "REQ002",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = "ORG001",
                Organization = "Acme Org",
                UserId = "USER001",
                User = "Runtime User",
                InstanceId = "INSTANCE001",
                Instance = "Runtime Instance"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2
            });

            Assert.That(result.Successful, Is.True, result.ErrorMessage);
            Assert.That(result.RequestId, Is.EqualTo("REQ002"));
            Assert.That(result.MatchedKeyName, Is.EqualTo("Key1"));
            Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.HmacSha256));
            Assert.That(headers.ContainsKey(SignedRequestHeaders.Authorization), Is.True);
        }

        [Test]
        public void Validator_RuntimeInstanceV1_AcceptsKey2Fallback()
        {
            var builder = new SignedRequestHeaderBuilder();
            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Key = Key2,
                RequestId = "REQ003",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = "ORG001",
                Organization = "Acme Org",
                UserId = "USER001",
                User = "Runtime User",
                InstanceId = "INSTANCE001",
                Instance = "Runtime Instance"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Headers = headers,
                Key1 = "wrong-key",
                Key2 = Key2
            });

            Assert.That(result.Successful, Is.True, result.ErrorMessage);
            Assert.That(result.MatchedKeyName, Is.EqualTo("Key2"));
            Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.HmacSha256));
        }

        [Test]
        public void HeaderBuilder_ServiceHttpV1_AddsBodyHash_AndValidatorAcceptsRsaPss()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;
                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);
                var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
                var builder = new SignedRequestHeaderBuilder();

                var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    RequestId = "REQ004",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    AppKey = "portal",
                    SigningKeyId = "portal-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands?trace=true",
                    Body = body
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands?trace=true",
                    BodySha256 = SignedRequestBodyHasher.ComputeSha256Base64(body),
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        AppKey = "portal",
                        KeyId = "portal-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    })
                });

                Assert.That(result.Successful, Is.True, result.ErrorMessage);
                Assert.That(result.MatchedKeyName, Is.EqualTo("PublicKey"));
                Assert.That(result.MatchedKeyId, Is.EqualTo("portal-live-202605"));
                Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
                Assert.That(headers[SignedRequestHeaders.BodySha256], Is.EqualTo(SignedRequestBodyHasher.ComputeSha256Base64(body)));
                Assert.That(headers[SignedRequestHeaders.SignatureAlgorithm], Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
                Assert.That(headers[SignedRequestHeaders.KeyMaterialFormat], Is.EqualTo(SignedRequestKeyMaterialFormats.RsaXml));
                Assert.That(headers[SignedRequestHeaders.SigningKeyId], Is.EqualTo("portal-live-202605"));
            }
        }

        [Test]
        public void Validator_ServiceHttpV1_RejectsTamperedBodyHash()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;
                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);
                var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
                var tamperedBody = Encoding.UTF8.GetBytes("{\"hello\":\"moon\"}");
                var builder = new SignedRequestHeaderBuilder();

                var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    RequestId = "REQ005",
                    DateUtc = DateTimeOffset.UtcNow,
                    Version = "1",
                    AppKey = "portal",
                    SigningKeyId = "portal-live-202605",
                    PrivateKeyMaterial = privateKeyMaterial,
                    Method = "POST",
                    PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands",
                    Body = body
                });

                var validator = new SignedRequestValidator();
                var result = validator.Validate(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands",
                    BodySha256 = SignedRequestBodyHasher.ComputeSha256Base64(tamperedBody),
                    ValidationKeyResolver = new StaticValidationKeyResolver(new SignedRequestValidationKey
                    {
                        AppKey = "portal",
                        KeyId = "portal-live-202605",
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    })
                });

                Assert.That(result.Successful, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo("invalid_signature"));
            }
        }

        [Test]
        public void Validator_RejectsAuthorizationRequestIdMismatch()
        {
            var headers = CreateRuntimeHeaders("REQ006", DateTimeOffset.UtcNow.ToString("o"));
            headers[SignedRequestHeaders.Authorization] = SignedRequestAuthorization.CreateHeader("DIFFERENT", "abc");

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2
            });

            Assert.That(result.Successful, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("request_id_mismatch"));
        }

        [Test]
        public void Validator_RejectsTimestampOutsideAllowedSkew()
        {
            var builder = new SignedRequestHeaderBuilder();
            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Key = Key1,
                RequestId = "REQ007",
                DateUtc = DateTimeOffset.UtcNow.AddHours(-1),
                Version = "1",
                OrganizationId = "ORG001",
                Organization = "Acme Org",
                UserId = "USER001",
                User = "Runtime User",
                InstanceId = "INSTANCE001",
                Instance = "Runtime Instance"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceV1,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2,
                MaxClockSkew = TimeSpan.FromMinutes(5)
            });

            Assert.That(result.Successful, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("timestamp_out_of_range"));
        }

        [Test]
        public void SignedRequestAuthorization_Parse_RoundTripsHeader()
        {
            var header = SignedRequestAuthorization.CreateHeader("REQ008", "abc123==");

            var parsed = SignedRequestAuthorization.Parse(header);

            Assert.That(parsed.Scheme, Is.EqualTo("SAS"));
            Assert.That(parsed.RequestId, Is.EqualTo("REQ008"));
            Assert.That(parsed.Signature, Is.EqualTo("abc123=="));
        }

        private static Dictionary<string, string> CreateRuntimeHeaders(string requestId, string date)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { SignedRequestHeaders.RuntimeRequestId, requestId },
                { SignedRequestHeaders.Date, date },
                { SignedRequestHeaders.Version, "1" },
                { SignedRequestHeaders.OrganizationId, "ORG001" },
                { SignedRequestHeaders.UserId, "USER001" },
                { SignedRequestHeaders.InstanceId, "INSTANCE001" }
            };
        }

        private class StaticValidationKeyResolver : ISignedRequestValidationKeyResolver
        {
            private readonly SignedRequestValidationKey _key;

            public StaticValidationKeyResolver(SignedRequestValidationKey key)
            {
                _key = key;
            }

            public SignedRequestValidationKey Resolve(string appKey, string keyId, string algorithm)
            {
                if (String.Equals(_key.AppKey, appKey, StringComparison.OrdinalIgnoreCase) &&
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
