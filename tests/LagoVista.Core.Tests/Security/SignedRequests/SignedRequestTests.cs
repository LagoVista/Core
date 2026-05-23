using LagoVista.Core.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestTests
    {
        private const string Key1 = "key-one";
        private const string Key2 = "key-two";

        [Test]
        public void RuntimeInstanceLegacy_BuildsCanonicalSource_InLegacyOrder()
        {
            var headers = CreateRuntimeHeaders("REQ001", "2026-05-23T16:20:51.0000000Z");

            var canonical = SignedRequestCanonicalizer.Build(new SignedRequestCanonicalContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Headers = headers
            });

            Assert.That(canonical, Is.EqualTo("REQ001\r\n2026-05-23T16:20:51.0000000Z\r\n1\r\nORG001\r\nUSER001\r\nINSTANCE001\r\n"));
        }

        [Test]
        public void HeaderBuilder_RuntimeInstanceLegacy_AddsAuthorizationHeader_AndValidatorAcceptsKey1()
        {
            var builder = new SignedRequestHeaderBuilder();
            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Key = Key1,
                RequestId = "REQ002",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = "ORG001",
                UserId = "USER001",
                InstanceId = "INSTANCE001"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2
            });

            Assert.That(result.Successful, Is.True);
            Assert.That(result.RequestId, Is.EqualTo("REQ002"));
            Assert.That(result.MatchedKeyName, Is.EqualTo("Key1"));
            Assert.That(headers.ContainsKey(SignedRequestHeaders.Authorization), Is.True);
        }

        [Test]
        public void Validator_RuntimeInstanceLegacy_AcceptsKey2Fallback()
        {
            var builder = new SignedRequestHeaderBuilder();
            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Key = Key2,
                RequestId = "REQ003",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                OrganizationId = "ORG001",
                UserId = "USER001",
                InstanceId = "INSTANCE001"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Headers = headers,
                Key1 = "wrong-key",
                Key2 = Key2
            });

            Assert.That(result.Successful, Is.True);
            Assert.That(result.MatchedKeyName, Is.EqualTo("Key2"));
        }

        [Test]
        public void HeaderBuilder_ServiceHttpV1_AddsBodyHash_AndValidatorAccepts()
        {
            var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
            var builder = new SignedRequestHeaderBuilder();

            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Key = Key1,
                RequestId = "REQ004",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                CallerId = "portal",
                Method = "POST",
                PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands?trace=true",
                Body = body
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2,
                Method = "POST",
                PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands?trace=true",
                BodySha256 = SignedRequestBodyHasher.ComputeSha256Base64(body)
            });

            Assert.That(result.Successful, Is.True);
            Assert.That(result.MatchedKeyName, Is.EqualTo("Key1"));
            Assert.That(headers[SignedRequestHeaders.BodySha256], Is.EqualTo(SignedRequestBodyHasher.ComputeSha256Base64(body)));
        }

        [Test]
        public void Validator_ServiceHttpV1_RejectsTamperedBodyHash()
        {
            var body = Encoding.UTF8.GetBytes("{\"hello\":\"world\"}");
            var tamperedBody = Encoding.UTF8.GetBytes("{\"hello\":\"moon\"}");
            var builder = new SignedRequestHeaderBuilder();

            var headers = builder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Key = Key1,
                RequestId = "REQ005",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                CallerId = "portal",
                Method = "POST",
                PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands",
                Body = body
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                Headers = headers,
                Key1 = Key1,
                Key2 = Key2,
                Method = "POST",
                PathAndQuery = "/api/remote-control/targets/INSTANCE001/commands",
                BodySha256 = SignedRequestBodyHasher.ComputeSha256Base64(tamperedBody)
            });

            Assert.That(result.Successful, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("invalid_signature"));
        }

        [Test]
        public void Validator_RejectsAuthorizationRequestIdMismatch()
        {
            var headers = CreateRuntimeHeaders("REQ006", DateTimeOffset.UtcNow.ToString("o"));
            headers[SignedRequestHeaders.Authorization] = SignedRequestAuthorization.CreateHeader("DIFFERENT", "abc");

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
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
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
                Key = Key1,
                RequestId = "REQ007",
                DateUtc = DateTimeOffset.UtcNow.AddHours(-1),
                Version = "1",
                OrganizationId = "ORG001",
                UserId = "USER001",
                InstanceId = "INSTANCE001"
            });

            var validator = new SignedRequestValidator();
            var result = validator.Validate(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceLegacy,
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
    }
}
