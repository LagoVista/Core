using LagoVista.Core.AsyncCoupler.Utils.Tests;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Security;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Security.SignedRequests
{
    [TestFixture]
    public sealed class SignedRequestPublicKeySetHttpClientTests
    {
        private sealed class TestConfig : IConfigServerClientSettings
        {
            public string PublicKeySetUrl => "https://config.example";
            public string AppKey => "web";
            public string Environment => "dev";
            public string AuthorizationToken => "receiver-only-credential";
        }

        private sealed class Allowed : ISignedRequestTrustedCallerProvider
        {
            public IReadOnlyCollection<SignedRequestTrustedCaller> GetTrustedCallers() =>
                new List<SignedRequestTrustedCaller>
                {
                    new SignedRequestTrustedCaller { AppKey = "api", Environment = "live" },
                    new SignedRequestTrustedCaller { AppKey = "web", Environment = "live" }
                };
        }

        private sealed class DiagnosticCollector : ISignedRequestKeyDiagnostics
        {
            public readonly List<string> Events = new List<string>();
            public void Write(string stage, string details) => Events.Add(stage + " " + details);
        }

        private sealed class RecordingHandler : HttpMessageHandler
        {
            public readonly List<string> Paths = new List<string>();
            public readonly List<string> RequestEnvironments = new List<string>();
            public readonly List<string> RequestCredentials = new List<string>();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Paths.Add(request.RequestUri.AbsolutePath);
                RequestEnvironments.Add(request.Headers.GetValues("X-Nuviot-Environment").Single());
                RequestCredentials.Add(request.Headers.GetValues("X-Config-Auth").Single());

                var isRemote = request.RequestUri.AbsolutePath.Contains("trusted-public-keys");
                var keys = new SignedRequestPublicKeySet
                {
                    Environment = isRemote ? "live" : "dev",
                    Keys = new List<SignedRequestPublicKeyEntry>()
                };
                if (isRemote)
                {
                    keys.Keys.Add(new SignedRequestPublicKeyEntry { AppKey = "api", KeyId = "api-live", PublicKeyMaterial = "api-public" });
                    keys.Keys.Add(new SignedRequestPublicKeyEntry { AppKey = "web", KeyId = "web-live", PublicKeyMaterial = "web-public" });
                    keys.Keys.Add(new SignedRequestPublicKeyEntry { AppKey = "mcp", KeyId = "mcp-live", PublicKeyMaterial = "mcp-public" });
                }
                else
                    keys.Keys.Add(new SignedRequestPublicKeyEntry { AppKey = "web", KeyId = "web-dev", PublicKeyMaterial = "local-public" });

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(keys))
                });
            }
        }

        [Test]
        public async Task TrustedPeerRetrieval_ReportsPublicKeyBoundariesWithoutCredentials()
        {
            var handler = new RecordingHandler();
            using var http = new HttpClient(handler);
            var diagnostics = new DiagnosticCollector();
            var client = new SignedRequestPublicKeySetHttpClient(
                http, new TestConfig(), new TestLogger(), new Allowed(), diagnostics);

            var result = await client.FetchAsync(CancellationToken.None);

            Assert.That(result.Keys.Count, Is.EqualTo(3));
            Assert.That(diagnostics.Events.Any(item => item.Contains("remote-key-set") &&
                item.Contains("web/web-live")), Is.True);
            Assert.That(diagnostics.Events.Any(item => item.Contains("remote-key-filter") &&
                item.Contains("accepted=2") && item.Contains("rejected=1")), Is.True);
            Assert.That(diagnostics.Events.Any(item => item.Contains("effective-key-set") &&
                item.Contains("web/web-live/live")), Is.True);
            Assert.That(diagnostics.Events.All(item => !item.Contains("receiver-only-credential") &&
                !item.Contains("web-public")), Is.True);
        }

        [Test]
        public async Task TrustedPeerRetrieval_UsesOnlyReceiverCredential_AndFiltersByCallerAndEnvironment()
        {
            var handler = new RecordingHandler();
            using var http = new HttpClient(handler);
            var client = new SignedRequestPublicKeySetHttpClient(http, new TestConfig(), new TestLogger(), new Allowed());
            var set = await client.FetchAsync(CancellationToken.None);

            Assert.That(handler.Paths, Is.EqualTo(new[]
            {
                "/api/config/web/dev/service-signing/public-keys",
                "/api/config/web/dev/service-signing/trusted-public-keys/live"
            }));
            Assert.That(handler.RequestEnvironments, Is.All.EqualTo("dev"));
            Assert.That(handler.RequestCredentials, Is.All.EqualTo("receiver-only-credential"));
            Assert.That(set.Keys.Count, Is.EqualTo(3));
            Assert.That(set.Keys.Any(k => k.AppKey == "mcp"), Is.False);
            Assert.That(set.Keys.Any(k => k.KeyId == "web-dev" && k.SourceEnvironment == "dev"), Is.True);
            Assert.That(set.Keys.Any(k => k.KeyId == "api-live" && k.SourceEnvironment == "live"), Is.True);
            Assert.That(set.Keys.Any(k => k.KeyId == "web-live" && k.SourceEnvironment == "live"), Is.True);
        }
    }
}
