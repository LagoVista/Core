using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class LagoVistaAutoMapperV1Tests
    {
        private static EntityHeader Org() { return new EntityHeader() { Id = "org-123" }; }
        private static EntityHeader User() { return new EntityHeader() { Id = "user-456" }; }

        IEncryptionKeyProvider _keyProvider = new EncryptionKeyProvider(new FakeSecureStorage());
        IEncryptedMapper _encryptedMapper;
        ILagoVistaAutoMapper _mapper;

        MapValueConverterRegistry _registry = new MapValueConverterRegistry(new IMapValueConverter[] {
               new DateTimeIsoStringConverter(),
               new NumericStringConverter(),
               new GuidStringConverter() });

        [SetUp]
        public void TestInitialize()
        {
            _encryptedMapper = new EncryptedMapper(_keyProvider, _registry, new Encryptor());
            _mapper = new LagoVistaAutoMapper(_encryptedMapper, _registry);
        }

        
        [Test]
        public async Task PlainMapping_CaseInsensitive_And_MapFrom_And_Ignore_Works()
        {
            var src = new PlainSource() { name = "Checking", EXTERNALPROVIDERID = "plaid-item-9", ShouldNotCopy = "nope" };

            var tgt = await _mapper.CreateAsync<PlainSource, PlainTarget>(src, Org(), User(), null, CancellationToken.None);

            Assert.That(tgt.Name, Is.EqualTo("Checking"));
            Assert.That(tgt.ExternalProviderId, Is.EqualTo("plaid-item-9"));
            Assert.That(tgt.ShouldNotCopy, Is.Null);
        }

        [Test]
        public async Task Crypto_Decrypt_DtoToDomain_Works()
        {
            var dto = new AccountDto() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), EncryptedBalance = null };

            // Pre-encrypt a value into DTO to simulate DB storage.
            var domainIn = new Account() { Balance = 100.25 };
            await _mapper.MapAsync<Account, AccountDto>(domainIn, dto, Org(), User(), null, CancellationToken.None);

            // Now decrypt DTO -> domain
            var domainOut = await _mapper.CreateAsync<AccountDto, Account>(dto, Org(), User(), null, CancellationToken.None);

            Assert.That(domainOut.Balance, Is.EqualTo(100.25));
        }

        [Test]
        public async Task Crypto_Encrypt_DomainToDto_Works()
        {
            var domain = new Account() { Balance = 77.01 };
            var dto = new AccountDto() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), EncryptedBalance = null };

            await _mapper.MapAsync<Account, AccountDto>(domain, dto, Org(), User(), null, CancellationToken.None);

            Assert.That(dto.EncryptedBalance, Is.Not.Null.And.Not.Empty);
            Assert.That(dto.EncryptedBalance, Is.Not.EqualTo("77.01"));

            var roundTrip = await _mapper.CreateAsync<AccountDto, Account>(dto, Org(), User(), null, CancellationToken.None);
            Assert.That(roundTrip.Balance, Is.EqualTo(77.01));
        }

        [Test]
        public async Task MapAsync_UpdatesExistingTargetInstance()
        {
            var src1 = new PlainSource() { name = "A", EXTERNALPROVIDERID = "X", ShouldNotCopy = "one" };
            var src2 = new PlainSource() { name = "B", EXTERNALPROVIDERID = "Y", ShouldNotCopy = "two" };

            var target = new PlainTarget();

            await _mapper.MapAsync(src1, target, Org(), User(), null, CancellationToken.None);
            Assert.That(target.Name, Is.EqualTo("A"));
            Assert.That(target.ExternalProviderId, Is.EqualTo("X"));

            await _mapper.MapAsync(src2, target, Org(), User(), null, CancellationToken.None);
            Assert.That(target.Name, Is.EqualTo("B"));
            Assert.That(target.ExternalProviderId, Is.EqualTo("Y"));
        }

        [Test]
        public async Task EndToEnd_Double_ToString_ToEncryptedString_ThenBackToDouble_Works()
        {
            var converters = new IMapValueConverter[]
            {
                new NumericStringConverter(),
                new GuidStringConverter(),
                new DateTimeIsoStringConverter()
            };

            var registry = new MapValueConverterRegistry(converters);

            var dto = new AccountDto() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), EncryptedBalance = null };

            var domainIn = new Account() { Balance = 77.01 };

            await _mapper.MapAsync<Account, AccountDto>(domainIn, dto, Org(), User(), null, CancellationToken.None);

            Assert.That(dto.EncryptedBalance, Is.Not.Null.And.Not.Empty);
            Assert.That(dto.EncryptedBalance, Is.Not.EqualTo("77.01"));

            var domainOut = await _mapper.CreateAsync<AccountDto, Account>(dto, Org(), User(), null, CancellationToken.None);

            Assert.That(domainOut.Balance, Is.EqualTo(domainIn.Balance).Within(0.0000001));
        }

        // ----------------------------
        // Test Models + Fakes
        // ----------------------------

        private sealed class PlainSource
        {
            public string name { get; set; }
            public string EXTERNALPROVIDERID { get; set; }
            public string ShouldNotCopy { get; set; }
        }

        private sealed class PlainTarget
        {
            public string Name { get; set; }

            [MapFrom(nameof(PlainSource.EXTERNALPROVIDERID))]
            public string ExternalProviderId { get; set; }

            [MapIgnore]
            public string ShouldNotCopy { get; set; }
        }

        [EncryptionKey("AccountKey-{id}", IdProperty = nameof(AccountDto.Id), CreateIfMissing = true)]
        private sealed class AccountDto
        {
            public Guid Id { get; set; }
            public string EncryptedBalance { get; set; }
        }

        private sealed class Account
        {
            [EncryptedField(nameof(AccountDto.EncryptedBalance), SaltProperty = nameof(AccountDto.Id), SkipIfEmpty = true)]
            public double Balance { get; set; }
        }

        public class FakeSecureStorage : ISecureStorage
        {
            private readonly Dictionary<string, string> _storage = new Dictionary<string, string>();

            public Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string value)
            {
                var id = Guid.NewGuid().ToString();
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> AddSecretAsync(EntityHeader org, string id, string value)
            {
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> AddUserSecretAsync(EntityHeader user, string value)
            {
                var id = Guid.NewGuid().ToString();
                _storage.Add(id, value);
                return Task.FromResult(InvokeResult<string>.Create(id));
            }

            public Task<InvokeResult<string>> GetSecretAsync(EntityHeader org, string id, EntityHeader user)
            {
                if (_storage.TryGetValue(id, out var value))
                {
                    return Task.FromResult(InvokeResult<string>.Create(value));
                }
                else
                {
                    return Task.FromResult(InvokeResult<string>.FromError($"Secret with id {id} not found."));
                }
            }

            public Task<InvokeResult<string>> GetUserSecretAsync(EntityHeader user, string id)
            {
                if (_storage.TryGetValue(id, out var value))
                {
                    return Task.FromResult(InvokeResult<string>.Create(value));
                }
                else
                {
                    return Task.FromResult(InvokeResult<string>.FromError($"Secret with id {id} not found."));
                }
            }

            public Task<InvokeResult> RemoveSecretAsync(EntityHeader org, string id)
            {
                throw new NotImplementedException();
            }

            public Task<InvokeResult> RemoveUserSecretAsync(EntityHeader user, string id)
            {
                throw new NotImplementedException();
            }
        }
    }
}
