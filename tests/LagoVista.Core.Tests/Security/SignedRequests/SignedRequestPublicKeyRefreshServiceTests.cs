using LagoVista.Core.Security;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public class SignedRequestPublicKeyRefreshServiceTests
    {
        [Test]
        public async Task RefreshAsync_ShouldFetchAndUpdateStore()
        {
            var initialSet = new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = "2026-05-26.001",
                GeneratedUtc = DateTimeOffset.UtcNow
            };

            var updatedSet = new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = "2026-05-26.002",
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
                    }
                }
            };

            var store = new SignedRequestPublicKeySetResolver(initialSet);
            var service = new SignedRequestPublicKeyRefreshService(new StaticPublicKeySetClient(updatedSet), store);

            var result = await service.RefreshAsync(CancellationToken.None);

            Assert.That(result.Successful, Is.True, result.ErrorMessage);
            Assert.That(result.Version, Is.EqualTo("2026-05-26.002"));
            Assert.That(store.Current.Version, Is.EqualTo("2026-05-26.002"));

            var key = store.Resolve("api", "api-live-202605", SignedRequestSignatureAlgorithms.RsaPssSha256);
            Assert.That(key, Is.Not.Null);
            Assert.That(key.PublicKeyMaterial, Is.EqualTo("public-key-api"));
        }

        [Test]
        public async Task RefreshAsync_ShouldReturnFailureAndKeepExistingStore_WhenClientFails()
        {
            var initialSet = new SignedRequestPublicKeySet
            {
                Environment = "live",
                Version = "2026-05-26.001",
                GeneratedUtc = DateTimeOffset.UtcNow
            };

            var store = new SignedRequestPublicKeySetResolver(initialSet);
            var service = new SignedRequestPublicKeyRefreshService(new FailingPublicKeySetClient(), store);

            var result = await service.RefreshAsync(CancellationToken.None);

            Assert.That(result.Successful, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("public_key_refresh_failed"));
            Assert.That(store.Current.Version, Is.EqualTo("2026-05-26.001"));
        }

        private class StaticPublicKeySetClient : ISignedRequestPublicKeySetClient
        {
            private readonly SignedRequestPublicKeySet _keySet;

            public StaticPublicKeySetClient(SignedRequestPublicKeySet keySet)
            {
                _keySet = keySet;
            }

            public Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_keySet);
            }
        }

        private class FailingPublicKeySetClient : ISignedRequestPublicKeySetClient
        {
            public Task<SignedRequestPublicKeySet> FetchAsync(CancellationToken cancellationToken)
            {
                throw new InvalidOperationException("boom");
            }
        }
    }
}
