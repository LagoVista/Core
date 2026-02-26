using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.AutoMapper.TestModels;
using LagoVista.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        private static EntityHeader Org() { return new EntityHeader() { Id = "org-123" }; }
        private static EntityHeader User() { return new EntityHeader() { Id = "user-456" }; }

        private readonly IEncryptionKeyProvider _keyProvider = new EncryptionKeyProvider(new FakeSecureStorage());

        private IEncryptedMapper _encryptedMapper;
        private ILagoVistaAutoMapper _mapper;
        private IAtomicPlanBuilder _atomicBuilder;
        private IMapValueConverterRegistry _registry;

        [SetUp]
        public void TestInitialize()
        {
            _registry = ConvertersRegistration.DefaultConverterRegistery;
            _atomicBuilder = new ReflectionAtomicPlanBuilder(_registry);

            // EncryptedMapper uses its own converter registry for string <-> domain conversions.
            // Use the same default registry to keep behavior consistent.
            _encryptedMapper = new EncryptedMapper(_keyProvider, _registry, new Encryptor());

            _mapper = new LagoVistaAutoMapper(_encryptedMapper, _atomicBuilder, _registry);
        }

        [Test]
        public async Task AppModels()
        {
            try
            {
                MappingVerifier.Verify<CoreEntity, DbModelBase>(true);
                MappingVerifier.Verify<DbModelBase, CoreEntity>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }
        [Test]
        public void TestEHMapping()
        {
            try
            { 
                MappingVerifier.Verify<EntityHeaderPrimary, EntityHeaderDTO>(true);
                MappingVerifier.Verify<EntityHeaderDTO, EntityHeaderPrimary>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }


        [Test]
        public async Task TestModels()
        {
            try
            {
                MappingVerifier.Verify<SimpleManual, SimpleManualDto>(true);
                MappingVerifier.Verify<SimpleManualDto, SimpleManual>(true);

                MappingVerifier.Verify<ChildIdMappingSource, ChildIdMappingTarget>(true);
                MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
                MappingVerifier.Verify<Account, AccountDto>(true);
                MappingVerifier.Verify<PlainEntityHeaderSource, PlainEntityHeaderDestination>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task MapstoChildIdTest()
        {
            var source = new ChildIdMappingSource();
            source.GrandChild = new GrandChild() { Id = "grandchild-789" }; 
            var tgt = await _mapper.CreateAsync<ChildIdMappingSource, ChildIdMappingTarget>(source, Org(), User(), null, CancellationToken.None);
            Assert.That(tgt.GrandChildId, Is.EqualTo("grandchild-789"));
        }

        [Test]
        public async Task TestCoreMapping()
        {
            try
            {
                MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
        }

        [Test]
        public async Task TestGraphMapping()
        {
            try
            {
                MappingVerifier.Verify<BigParent, BigParentDTO>(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Mapping verification threw an exception: {ex.Message.Replace("\n", Environment.NewLine)}");
            }
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
        public async Task EH_To_Name_and_Id()
        {
            var eh = new EntityHeaderPrimary() { TheProperty = EntityHeader.Create("eh-123", "My Entity Header") };     
            var dto = await _mapper.CreateAsync<EntityHeaderPrimary, EntityHeaderDTO>(eh, Org(), User(), null, CancellationToken.None);
            Assert.That(dto.Id, Is.EqualTo("eh-123"));
            Assert.That(dto.Text, Is.EqualTo("My Entity Header"));
    }

        [Test]
        public async Task Name_and_Id_ToEH()
        {
            var dto = new EntityHeaderDTO() { Id = "MYID", Text = "The TextFor The Property" };
            var eh   = await _mapper.CreateAsync<EntityHeaderDTO, EntityHeaderPrimary>(dto, Org(), User(), null, CancellationToken.None);
            Assert.That(eh.TheProperty.Id, Is.EqualTo("MYID"));
            Assert.That(eh.TheProperty.Text, Is.EqualTo("The TextFor The Property"));
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
            var dto = new AccountDto() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), EncryptedBalance = null };

            var domainIn = new Account() { Balance = 77.01 };

            await _mapper.MapAsync<Account, AccountDto>(domainIn, dto, Org(), User(), null, CancellationToken.None);

            Assert.That(dto.EncryptedBalance, Is.Not.Null.And.Not.Empty);
            Assert.That(dto.EncryptedBalance, Is.Not.EqualTo("77.01"));

            var domainOut = await _mapper.CreateAsync<AccountDto, Account>(dto, Org(), User(), null, CancellationToken.None);

            Assert.That(domainOut.Balance, Is.EqualTo(domainIn.Balance).Within(0.0000001));
        }
    }
}