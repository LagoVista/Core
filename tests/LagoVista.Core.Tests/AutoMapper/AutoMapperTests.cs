using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Services;
using LagoVista.Core.Tests.AutoMapper.TestModels;
using LagoVista.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AdaptiveCard.MSTeams;

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
            var planner = new EncryptedMapperPlanner(_registry);    
            // EncryptedMapper uses its own converter registry for string <-> domain conversions.
            // Use the same default registry to keep behavior consistent.
            _encryptedMapper = new EncryptedMapper(_keyProvider, planner, new Encryptor());

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
        public async Task MappingForChildDTO_ToParentDTOProp()
        {
            try
            {
                MappingVerifier.Verify<SimpleWithEH, SimpleWithEHDto>(true);
                MappingVerifier.Verify<SimpleWithEHDto, SimpleWithEH>(true);
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
                MappingVerifier.Verify<DateMapping, DateMappingDTO>(true);
                MappingVerifier.Verify<DateMappingDTO, DateMapping>(true);

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
            var id = GuidString36.Factory();

            source.GrandChild = new GrandChild() { Id = id }; 
            var tgt = await _mapper.CreateAsync<ChildIdMappingSource, ChildIdMappingTarget>(source, Org(), User(), null, CancellationToken.None);
            Assert.That(tgt.GrandChildId, Is.EqualTo(id));
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
        public async Task TestCoreMapping_Reverse()
        {
            var dto = new SimpleWithEHDto() { Name = "Test", ChildForEH = new SimpleWithEHDtoChild() { Id = "eh-001", Key = "key-001", Name = "Entity Header Name" } };
            var entity = await _mapper.CreateAsync<SimpleWithEHDto, SimpleWithEH>(dto, Org(), User());

            Assert.That(entity.EntityEH.Id, Is.EqualTo("eh-001"));
            Assert.That(entity.EntityEH.Key, Is.EqualTo("key-001"));
            Assert.That(entity.EntityEH.Text, Is.EqualTo("Entity Header Name"));
        }

        [Test]
        public async Task ISO_DateTime_To_DTO_Test()
        {
            MappingVerifier.Verify<DateMapping, DateMappingDTO>(true);
            var timeStamp = DateTime.UtcNow.ToJSONString();    
            var entity = new DateMapping() { TheDate = timeStamp };
            var dto = await _mapper.CreateAsync<DateMapping, DateMappingDTO>(entity, Org(), User());
            var jsonDate = dto.TheDate.ToJSONString();
            Assert.That(jsonDate, Is.EqualTo(timeStamp));
        }

        [Test]
        public async Task DTO_To_ISO_DateTime_Test()
        {
            MappingVerifier.Verify<DateMappingDTO, DateMapping>(true);
            var timeStamp = DateTime.UtcNow;
            var dto = new DateMappingDTO() { TheDate = timeStamp};
            var entity = await _mapper.CreateAsync<DateMappingDTO, DateMapping>(dto, Org(), User());
            Assert.That(entity.TheDate, Is.EqualTo(new UtcTimestamp(timeStamp.ToJSONString())));
        }

        [Test]
        public async Task ISO_DateOnly_To_DTO_Test()
        {
            MappingVerifier.Verify<DateMapping, DateMappingDTO>(true);
            var dateOnly = DateOnly.Parse("2024-06-01");
            var entity = new DateMapping() { TheDateOnly = dateOnly.ToString("yyyy/MM/dd") };
            var dto = await _mapper.CreateAsync<DateMapping, DateMappingDTO>(entity, Org(), User());
            Assert.That(dto.TheDateOnly, Is.EqualTo(dateOnly));
        }

        [Test]
        public async Task DTO_To_ISO_DateOnly_Test()
        {
            MappingVerifier.Verify<DateMappingDTO, DateMapping>(true);
            var dateOnly = DateOnly.Parse("2024-06-01");
            var dto = new DateMappingDTO() { TheDateOnly = dateOnly };
            var entity = await _mapper.CreateAsync<DateMappingDTO, DateMapping>(dto, Org(), User());
            Assert.That(entity.TheDateOnly, Is.EqualTo(new CalendarDate(dateOnly.ToString("yyyy-MM-dd"))));
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
        public async Task DbModel_RelationBase_Maps_EH_Props_With_Child_Map()
        {
            var timeStamp = DateTime.UtcNow;

            var db = new DbModelBase() { Id = Guid.NewGuid(), 
                CreatedByUser = new AppUserDTO() { AppUserId = "user-123", FullName = "Tracey Marcey" },
                LastUpdatedByUser = new AppUserDTO() { AppUserId = "user-123", FullName = "Tracey Marcey" },
                Organization = new OrganizationDTO() { OrgId = "org-456", OrgName = "Frnks Fish" },
                LastUpdateDate = timeStamp,
                CreationDate = timeStamp
            };

            MappingVerifier.Verify<DbModelBase, RelationalEntityBase>(true);
            var entity = await _mapper.CreateAsync<DbModelBase, RelationalEntityBase>(db, Org(), User(), pln=>
            {   
                pln.IncludeChild(ch => ch.CreatedBy, s => s.CreatedByUser);
                pln.IncludeChild(ch => ch.LastUpdatedBy, s => s.LastUpdatedByUser);
                pln.IncludeChild(ch => ch.OwnerOrganization, s => s.Organization);
            },
            null, CancellationToken.None );


            Assert.That(entity.Id.Value, Is.EqualTo(db.Id.ToString()));
            Assert.That(entity.CreatedBy.Id, Is.EqualTo("user-123"));
            Assert.That(entity.CreatedBy.Text, Is.EqualTo("Tracey Marcey"));
            Assert.That(entity.OwnerOrganization.Id, Is.EqualTo("org-456"));
            Assert.That(entity.OwnerOrganization.Text, Is.EqualTo("Frnks Fish"));
            Assert.That(entity.CreationDate.Value, Is.EqualTo(timeStamp.ToJSONString()));
            Assert.That(entity.LastUpdatedDate.Value, Is.EqualTo(timeStamp.ToJSONString()));
        }

        [Test]
        public async Task DbModel_RelationBase_Maps_EH_Props_Without_Child_Map()
        {
            var timeStamp = DateTime.UtcNow;

            var db = new DbModelBase()
            {
                Id = Guid.NewGuid(),
                CreatedByUser = new AppUserDTO() { AppUserId = "user-123", FullName = "Tracey Marcey" },
                LastUpdatedByUser = new AppUserDTO() { AppUserId = "user-123", FullName = "Tracey Marcey" },
                Organization = new OrganizationDTO() { OrgId = "org-456", OrgName = "Frnks Fish" },
                LastUpdateDate = timeStamp,
                CreationDate = timeStamp
            };

            MappingVerifier.Verify<DbModelBase, RelationalEntityBase>(true);
            var entity = await _mapper.CreateAsync<DbModelBase, RelationalEntityBase>(db, Org(), User(), null, CancellationToken.None);

            Assert.That(entity.Id.Value, Is.EqualTo(db.Id.ToString()));
            Assert.That(entity.CreatedBy.Id, Is.EqualTo("user-123"));
            Assert.That(entity.CreatedBy.Text, Is.EqualTo("Tracey Marcey"));
            Assert.That(entity.OwnerOrganization.Id, Is.EqualTo("org-456"));
            Assert.That(entity.OwnerOrganization.Text, Is.EqualTo("Frnks Fish"));
            Assert.That(entity.CreationDate.Value, Is.EqualTo(timeStamp.ToJSONString()));
            Assert.That(entity.LastUpdatedDate.Value, Is.EqualTo(timeStamp.ToJSONString()));
        }

        [Test]
        public async Task DbModel_RelationBase_Maps_EH_Props_With_Child_Map_But_Null_EH()
        {
            var timeStamp = DateTime.UtcNow;
            var entity = new RelationalEntityBase()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = User(),
                LastUpdatedBy = User(),
                OwnerOrganization = Org(),
                LastUpdatedDate = timeStamp.ToJSONString(),
                CreationDate = timeStamp.ToJSONString()
            };

            MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
            var dto = await _mapper.CreateAsync<RelationalEntityBase, DbModelBase>(entity, Org(), User(), null, CancellationToken.None);
            Assert.That(dto.Id, Is.EqualTo(Guid.Parse(entity.Id)));
            Assert.That(dto.CreatedById, Is.EqualTo("user-456"));
            Assert.That(dto.OrganizationId, Is.EqualTo("org-123"));
            Assert.That(dto.CreationDate, Is.EqualTo(timeStamp).Within(TimeSpan.FromSeconds(1)));
            Assert.That(dto.LastUpdateDate, Is.EqualTo(timeStamp).Within(TimeSpan.FromSeconds(1)));

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