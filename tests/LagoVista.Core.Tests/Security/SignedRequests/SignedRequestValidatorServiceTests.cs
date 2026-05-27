using LagoVista.Core.Security;
using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestValidatorServiceTests
    {
        [Test]
        public async Task ValidateServiceHttpV1Async_ShouldUseInjectedResolver()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;
                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);
                var headers = CreateServiceHeaders(privateKeyMaterial, "api-live-202605");
                var resolver = new SignedRequestPublicKeySetResolver(CreateKeySet("api-live-202605", publicKeyMaterial));
                var refreshService = new TestSignedRequestPublicKeyRefreshService();
                var service = new SignedRequestValidatorService(resolver, refreshService);

                var result = await service.ValidateServiceHttpV1Async(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work"
                });

                Assert.That(result.Successful, Is.True, result.ErrorMessage);
                Assert.That(result.MatchedKeyId, Is.EqualTo("api-live-202605"));
                Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.RsaPssSha256));
                Assert.That(refreshService.RefreshCount, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ValidateServiceHttpV1Async_ShouldRefreshOnceWhenKeyIsNotFound()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 3072;
                var privateKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(true), true);
                var publicKeyMaterial = SignedRequestRsaXmlSerializer.ToXmlString(rsa.ExportParameters(false), false);
                var headers = CreateServiceHeaders(privateKeyMaterial, "api-live-202606");
                var resolver = new SignedRequestPublicKeySetResolver(new SignedRequestPublicKeySet { Environment = "live", Version = "empty", GeneratedUtc = DateTimeOffset.UtcNow });
                var refreshService = new UpdatingRefreshService(resolver, CreateKeySet("api-live-202606", publicKeyMaterial));
                var service = new SignedRequestValidatorService(resolver, refreshService);

                var result = await service.ValidateServiceHttpV1Async(new SignedRequestValidationContext
                {
                    Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                    Headers = headers,
                    Method = "POST",
                    PathAndQuery = "/api/internal/work"
                });

                Assert.That(result.Successful, Is.True, result.ErrorMessage);
                Assert.That(result.MatchedKeyId, Is.EqualTo("api-live-202606"));
                Assert.That(refreshService.RefreshCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void ValidateRuntimeInstanceHttpV1_ShouldValidateWithoutPublicKeyResolver()
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
            var resolver = new SignedRequestPublicKeySetResolver(new SignedRequestPublicKeySet { Environment = "live", Version = "empty", GeneratedUtc = DateTimeOffset.UtcNow });
            var refreshService = new TestSignedRequestPublicKeyRefreshService();
            var service = new SignedRequestValidatorService(resolver, refreshService);

            var result = service.ValidateRuntimeInstanceHttpV1(new SignedRequestValidationContext
            {
                Profile = SignedRequestCanonicalProfile.RuntimeInstanceHttpV1,
                Headers = headers,
                Key1 = "runtime-instance-private-key",
                Method = "POST",
                PathAndQuery = "/api/runtime/messages"
            });

            Assert.That(result.Successful, Is.True, result.ErrorMessage);
            Assert.That(result.SignatureAlgorithm, Is.EqualTo(SignedRequestSignatureAlgorithms.HmacSha256));
            Assert.That(refreshService.RefreshCount, Is.EqualTo(0));
        }

        [Test]
        public void ValidateServiceHttpV1Async_ShouldFailLoudlyForWrongProfile()
        {
            var resolver = new SignedRequestPublicKeySetResolver(new SignedRequestPublicKeySet { Environment = "live", Version = "empty", GeneratedUtc = DateTimeOffset.UtcNow });
            var service = new SignedRequestValidatorService(resolver, new TestSignedRequestPublicKeyRefreshService());

            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.ValidateServiceHttpV1Async(new SignedRequestValidationContext { Profile = SignedRequestCanonicalProfile.RuntimeInstanceHttpV1 }));
        }

        private static System.Collections.Generic.Dictionary<string, string> CreateServiceHeaders(string privateKeyMaterial, string keyId)
        {
            var headerBuilder = new SignedRequestHeaderBuilder();
            return headerBuilder.BuildHeaders(new SignedRequestHeaderBuildContext
            {
                Profile = SignedRequestCanonicalProfile.ServiceHttpV1,
                AppKey = "api",
                RequestId = "59B328E9E6D249999BA864CE153B5F5E",
                DateUtc = DateTimeOffset.UtcNow,
                Version = "1",
                SigningKeyId = keyId,
                PrivateKeyMaterial = privateKeyMaterial,
                Method = "POST",
                PathAndQuery = "/api/internal/work",
                Body = new byte[] { 1, 2, 3 }
            });
        }

        private static SignedRequestPublicKeySet CreateKeySet(string keyId, string publicKeyMaterial)
        {
            return new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = keyId,
                GeneratedUtc = DateTimeOffset.UtcNow,
                Keys =
                {
                    new SignedRequestPublicKeyEntry
                    {
                        AppKey = "api",
                        KeyId = keyId,
                        Algorithm = SignedRequestSignatureAlgorithms.RsaPssSha256,
                        KeyMaterialFormat = SignedRequestKeyMaterialFormats.RsaXml,
                        PublicKeyMaterial = publicKeyMaterial,
                        Status = SignedRequestValidationKeyStatuses.Active
                    }
                }
            };
        }

        private class TestSignedRequestPublicKeyRefreshService : ISignedRequestPublicKeyRefreshService
        {
            public int RefreshCount { get; private set; }

            public Task<SignedRequestPublicKeyRefreshResult> RefreshAsync(CancellationToken cancellationToken)
            {
                RefreshCount++;
                return Task.FromResult(SignedRequestPublicKeyRefreshResult.Success("test"));
            }
        }

        private class UpdatingRefreshService : ISignedRequestPublicKeyRefreshService
        {
            private readonly SignedRequestPublicKeySetResolver _resolver;
            private readonly SignedRequestPublicKeySet _keySet;

            public int RefreshCount { get; private set; }

            public UpdatingRefreshService(SignedRequestPublicKeySetResolver resolver, SignedRequestPublicKeySet keySet)
            {
                _resolver = resolver;
                _keySet = keySet;
            }

            public Task<SignedRequestPublicKeyRefreshResult> RefreshAsync(CancellationToken cancellationToken)
            {
                RefreshCount++;
                _resolver.Update(_keySet);
                return Task.FromResult(SignedRequestPublicKeyRefreshResult.Success(_keySet.Version));
            }
        }
    }
}
