using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Services;
using LagoVista.Core.Validation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class EncryptionKeyProviderTests
    {
        private Mock<ISecureStorage> _secureStorage;
        private EncryptionKeyProvider _sut;
        private EntityHeader _org;
        private EntityHeader _user;

        [SetUp]
        public void Setup()
        {
            _secureStorage = new Mock<ISecureStorage>(MockBehavior.Strict);
            _sut = new EncryptionKeyProvider(_secureStorage.Object);
            _org = EntityHeader.Create("org-1", "Org 1");
            _user = EntityHeader.Create("user-1", "User 1");
        }

        [Test]
        public void Constructor_When_SecureStorage_Is_Null_Should_Throw()
        {
            Assert.That(() => new EncryptionKeyProvider(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("secureStorage"));
        }

        [Test]
        public void GetKeyAsync_When_SecretId_Is_Null_Should_Throw()
        {
            Assert.That(async () => await _sut.GetKeyAsync(null, _org, _user, false), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("secretId"));
        }

        [Test]
        public void GetKeyAsync_When_SecretId_Is_Empty_Should_Throw()
        {
            Assert.That(async () => await _sut.GetKeyAsync(string.Empty, _org, _user, false), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("secretId"));
        }

        [Test]
        public void GetKeyAsync_When_SecretId_Is_Whitespace_Should_Throw()
        {
            Assert.That(async () => await _sut.GetKeyAsync("   ", _org, _user, false), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("secretId"));
        }

        [Test]
        public void GetKeyAsync_When_Org_Is_Null_Should_Throw()
        {
            Assert.That(async () => await _sut.GetKeyAsync("secret-1", null, _user, false), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("org"));
        }

        [Test]
        public void GetKeyAsync_When_User_Is_Null_Should_Throw()
        {
            Assert.That(async () => await _sut.GetKeyAsync("secret-1", _org, null, false), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("user"));
        }

        [Test]
        public async Task GetKeyAsync_When_Secret_Exists_Should_Return_Key()
        {
            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.Create("key-123"));

            var result = await _sut.GetKeyAsync("secret-1", _org, _user, false);

            Assert.That(result, Is.EqualTo("key-123"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Secret_Exists_Should_Cache_Successful_Result()
        {
            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.Create("key-123"));

            var result1 = await _sut.GetKeyAsync("secret-1", _org, _user, false);
            var result2 = await _sut.GetKeyAsync("secret-1", _org, _user, false);

            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo("key-123"));
                Assert.That(result2, Is.EqualTo("key-123"));
            });

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Initial_Read_Fails_And_CreateIfMissing_Is_False_Should_Throw()
        {
            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.GetKeyAsync("secret-1", _org, _user, false));

            Assert.That(ex!.Message, Does.Contain("Could not read encryption key for: secret-1"));
            Assert.That(ex.Message, Does.Contain("missing"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Initial_Read_Fails_And_CreateIfMissing_Is_True_Should_Create_And_Reread()
        {
            _secureStorage
                .SetupSequence(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.Create("created-key"));

            _secureStorage
                .Setup(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()))
                .ReturnsAsync(InvokeResult<string>.Create("secret-i1"));

            var result = await _sut.GetKeyAsync("secret-1", _org, _user, true);

            Assert.That(result, Is.EqualTo("created-key"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Exactly(2));
            _secureStorage.Verify(x => x.AddSecretAsync(_org, "secret-1", It.Is<string>(s => !string.IsNullOrWhiteSpace(s) && s.Length == 32)), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Create_Succeeds_Should_Cache_Result_For_Subsequent_Calls()
        {
            _secureStorage
                .SetupSequence(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.Create("created-key"));

            _secureStorage
                .Setup(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()))
                .ReturnsAsync(InvokeResult<string>.Create("secret-i1"));

            var result1 = await _sut.GetKeyAsync("secret-1", _org, _user, true);
            var result2 = await _sut.GetKeyAsync("secret-1", _org, _user, true);

            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo("created-key"));
                Assert.That(result2, Is.EqualTo("created-key"));
            });

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Exactly(2));
            _secureStorage.Verify(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Read_Fails_Should_Not_Cache_Failure()
        {
            _secureStorage
                .SetupSequence(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.Create("key-123"));

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.GetKeyAsync("secret-1", _org, _user, false));

            var result = await _sut.GetKeyAsync("secret-1", _org, _user, false);

            Assert.That(result, Is.EqualTo("key-123"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Exactly(2));
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_First_Call_Fails_With_CreateIfMissing_False_Should_Allow_Second_Call_With_CreateIfMissing_True()
        {
            _secureStorage
                .SetupSequence(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.Create("created-key"));

            _secureStorage
                .Setup(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()))
                .ReturnsAsync(InvokeResult<string>.Create("secret-1"));

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.GetKeyAsync("secret-1", _org, _user, false));

            var result = await _sut.GetKeyAsync("secret-1", _org, _user, true);

            Assert.That(result, Is.EqualTo("created-key"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Exactly(3));
            _secureStorage.Verify(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public void GetKeyAsync_When_CancellationToken_Is_Already_Canceled_Should_Throw_OperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await _sut.GetKeyAsync("secret-1", _org, _user, false, cts.Token), Throws.InstanceOf<OperationCanceledException>());

            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Called_Concurrently_For_Same_Secret_Should_Share_One_InFlight_Fetch()
        {
            var gate = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var callCount = 0;

            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-1", _user))
                .Returns(async () =>
                {
                    Interlocked.Increment(ref callCount);
                    var key = await gate.Task.ConfigureAwait(false);
                    return InvokeResult<string>.Create(key);
                });

            var tasks = Enumerable.Range(0, 10)
                .Select(_ => _sut.GetKeyAsync("secret-1", _org, _user, false))
                .ToList();

            await Task.Delay(50);
            gate.SetResult("shared-key");

            var results = await Task.WhenAll(tasks);

            Assert.That(results, Has.All.EqualTo("shared-key"));
            Assert.That(callCount, Is.EqualTo(1));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Called_For_Different_Secrets_Should_Fetch_Each_Separately()
        {
            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.Create("key-1"));

            _secureStorage
                .Setup(x => x.GetSecretAsync(_org, "secret-2", _user))
                .ReturnsAsync(InvokeResult<string>.Create("key-2"));

            var result1 = await _sut.GetKeyAsync("secret-1", _org, _user, false);
            var result2 = await _sut.GetKeyAsync("secret-2", _org, _user, false);

            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo("key-1"));
                Assert.That(result2, Is.EqualTo("key-2"));
            });

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Once);
            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-2", _user), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetKeyAsync_When_Reread_After_Create_Fails_Should_Throw_And_Not_Cache_Failure()
        {
            _secureStorage
                .SetupSequence(x => x.GetSecretAsync(_org, "secret-1", _user))
                .ReturnsAsync(InvokeResult<string>.FromError("missing"))
                .ReturnsAsync(InvokeResult<string>.FromError("still missing"))
                .ReturnsAsync(InvokeResult<string>.Create("key-123"));

            _secureStorage
                .Setup(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()))
                .ReturnsAsync(InvokeResult<string>.Create("secret-1"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _sut.GetKeyAsync("secret-1", _org, _user, true));

            Assert.That(ex!.Message, Does.Contain("Could not create encryption key for: secret-1"));
            Assert.That(ex.Message, Does.Contain("still missing"));

            var result = await _sut.GetKeyAsync("secret-1", _org, _user, false);

            Assert.That(result, Is.EqualTo("key-123"));

            _secureStorage.Verify(x => x.GetSecretAsync(_org, "secret-1", _user), Times.Exactly(3));
            _secureStorage.Verify(x => x.AddSecretAsync(_org, "secret-1", It.IsAny<string>()), Times.Once);
            _secureStorage.VerifyNoOtherCalls();
        }
    }
}
