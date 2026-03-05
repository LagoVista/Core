using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.Crypto.Modern;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Crypto.Tests.Modern
{


    [TestFixture]
    public sealed class SecureStorageKeyMaterialStoreTests
    {
        private static EntityHeader Org() => new EntityHeader { Id = "ORG" };
        private static EntityHeader User() => new EntityHeader { Id = "USER" };

        [Test]
        public async Task GetOrCreateKey32Async_WhenSecretExists_ReturnsKey()
        {
            var storage = new FakeSecureStorage();
            var sut = new SecureStorageKeyMaterialStore(storage);

            var key = new byte[32];
            RandomNumberGenerator.Fill(key);

            var secretId = "customer:kv:1";
            storage.Seed(secretId, Convert.ToBase64String(key));

            var result = await sut.GetOrCreateKey32Async(Org(), User(), "customer", 1);

            Assert.That(result, Is.EqualTo(key));
        }

        [Test]
        public async Task GetOrCreateKey32Async_WhenSecretMissing_CreatesKey()
        {
            var storage = new FakeSecureStorage();
            var sut = new SecureStorageKeyMaterialStore(storage);

            var result = await sut.GetOrCreateKey32Async(Org(), User(), "customer", 1);

            Assert.That(result.Length, Is.EqualTo(32));
            Assert.That(storage.AddCalled, Is.True);
        }

        [Test]
        public void GetOrCreateKey32Async_WhenCreatedButStillMissing_Throws()
        {
            var storage = new FakeSecureStorage { FailSecondGet = true };
            var sut = new SecureStorageKeyMaterialStore(storage);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.GetOrCreateKey32Async(Org(), User(), "customer", 1));

            Assert.That(ex.Message, Does.Contain("Could not create key material"));
        }

        [Test]
        public void GetOrCreateKey32Async_WhenKeyWrongLength_Throws()
        {
            var storage = new FakeSecureStorage();
            var sut = new SecureStorageKeyMaterialStore(storage);

            var secretId = "customer:kv:1";
            storage.Seed(secretId, Convert.ToBase64String(new byte[16])); // wrong length

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.GetOrCreateKey32Async(Org(), User(), "customer", 1));
        }

    }
}