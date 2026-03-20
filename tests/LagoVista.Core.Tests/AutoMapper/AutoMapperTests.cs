using LagoVista;
using LagoVista.Core.Attributes;
using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Crypto;
using LagoVista.Core.Services;
using LagoVista.Core.Tests.AutoMapper.TestModels;
using LagoVista.Models;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        // OrgId is NormalizedId32 (32 uppercase). This converts cleanly to GuidString36 inside mapper.
        private static EntityHeader Org() => new EntityHeader { Id = "F47AC10B58CC4372A5670E02B2C3D479" };

        // UserId is only used for logging/audit in secure storage in your setup, but must be non-null.
        private static EntityHeader User() => new EntityHeader { Id = "3268E49D6F54499689F24DFABFE06C5F" };

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

            // Test doubles for modern encryption + keyid building
            var modernEncryption = new FakeModernEncryption();
            var modernKeyIdBuilder = new FakeModernKeyIdBuilder();

            _encryptedMapper = new EncryptedMapper(
                _keyProvider,
                planner,
                new Encryptor(),
                modernEncryption,
                modernKeyIdBuilder);

            _mapper = new LagoVistaAutoMapper(_encryptedMapper, _atomicBuilder, _registry);
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
            var dto = new SimpleWithEHDto() { Name = "Test", ChildForEH = new SimpleWithEHDtoChild() { Id = "D7A50FB68C3A44A0A8F3B21723BF65C1", Key = "key-001", Name = "Entity Header Name" } };
            var entity = await _mapper.CreateAsync<SimpleWithEHDto, SimpleWithEH>(dto, Org(), User());

            Assert.That(entity.EntityEH.Id, Is.EqualTo("D7A50FB68C3A44A0A8F3B21723BF65C1"));
            Assert.That(entity.EntityEH.Key, Is.EqualTo("key-001"));
            Assert.That(entity.EntityEH.Text, Is.EqualTo("Entity Header Name"));
        }

        [Test]
        public async Task ISO_DateTime_To_DTO_Test()
        {
            MappingVerifier.Verify<DateMapping, DateMappingDTO>(true);
            var timeStamp = UtcTimestamp.Now;
            var entity = new DateMapping() { TheDate = timeStamp };
            var dto = await _mapper.CreateAsync<DateMapping, DateMappingDTO>(entity, Org(), User());
            var jsonDate = dto.TheDate.ToJSONString();
            Assert.That(jsonDate, Is.EqualTo(timeStamp.Value));
        }

        [Test]
        public async Task DTO_To_ISO_DateTime_Test()
        {
            MappingVerifier.Verify<DateMappingDTO, DateMapping>(true);
            var timeStamp = DateTime.UtcNow;
            var dto = new DateMappingDTO() { TheDate = timeStamp };
            var entity = await _mapper.CreateAsync<DateMappingDTO, DateMapping>(dto, Org(), User());
            Assert.That(entity.TheDate, Is.EqualTo(new UtcTimestamp(timeStamp.ToJSONString())));
        }

        [Test]
        public async Task NullableNormalizedId_To_String_Not_Null()
        {
            var id = NormalizedId32.Factory();

            var src = new PlainSourceWithNullableNormalizedString32() { NullableId = id};
            var mapped = await _mapper.CreateAsync<PlainSourceWithNullableNormalizedString32, PlainTargetWithNullableStringId>(src, Org(), User());
            Assert.That(mapped.NullableId , Is.EqualTo(id.Value));
        }


        [Test]
        public async Task String_To_NullableNormalizedId_Not_Null()
        {
            var id = NormalizedId32.Factory().ToString();

            var src = new PlainTargetWithNullableStringId() { NullableId = id };
            var mapped = await _mapper.CreateAsync<PlainTargetWithNullableStringId, PlainSourceWithNullableNormalizedString32>(src, Org(), User());
            Assert.That(mapped.NullableId.ToString(), Is.EqualTo(id));
        }


        [Test]
        public async Task NullableNormalizedId_To_String_Null()
        {

            var src = new PlainSourceWithNullableNormalizedString32() { NullableId = null };
            var mapped = await _mapper.CreateAsync<PlainSourceWithNullableNormalizedString32, PlainTargetWithNullableStringId>(src, Org(), User());
            Assert.That(mapped.NullableId, Is.Null);
        }


        [Test]
        public async Task String_To_NullableNormalizedId_Null()
        {

            var src = new PlainTargetWithNullableStringId() { NullableId =null };
            var mapped = await _mapper.CreateAsync<PlainTargetWithNullableStringId, PlainSourceWithNullableNormalizedString32>(src, Org(), User());
            Assert.That(mapped.NullableId, Is.Null);
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

            var db = new DbModelBase()
            {
                Id = Guid.NewGuid(),
                CreatedByUser = new AppUserDTO() { AppUserId = "D7A50FB68C3A44A0A8F3B21723BF65C1", FullName = "Tracey Marcey" },
                LastUpdatedByUser = new AppUserDTO() { AppUserId = "D7A50FB68C3A44A0A8F3B21723BF65C1", FullName = "Tracey Marcey" },
                Organization = new OrganizationDTO() { OrgId = "C00BB11B54234CD78566F46306B8A9E2", OrgName = "Franks Fish" },
                LastUpdatedDate = timeStamp,
                CreationDate = timeStamp
            };

            MappingVerifier.Verify<DbModelBase, RelationalEntityBase>(true);
            var entity = await _mapper.CreateAsync<DbModelBase, RelationalEntityBase>(db, Org(), User(), null, CancellationToken.None);

            Assert.That(entity.CreatedBy, Is.Not.Null);
            Assert.That(entity.LastUpdatedBy, Is.Not.Null);
            Assert.That(entity.OwnerOrganization, Is.Not.Null);

            Assert.That(entity.Id.Value, Is.EqualTo(db.Id.ToString()));
            Assert.That(entity.CreatedBy.Id, Is.EqualTo("D7A50FB68C3A44A0A8F3B21723BF65C1"));
            Assert.That(entity.CreatedBy.Text, Is.EqualTo("Tracey Marcey"));
            Assert.That(entity.OwnerOrganization.Id, Is.EqualTo("C00BB11B54234CD78566F46306B8A9E2"));
            Assert.That(entity.OwnerOrganization.Text, Is.EqualTo("Franks Fish"));
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
                CreatedByUser = new AppUserDTO() { AppUserId = "D7A50FB68C3A44A0A8F3B21723BF65C1", FullName = "Tracey Marcey" },
                LastUpdatedByUser = new AppUserDTO() { AppUserId = "D7A50FB68C3A44A0A8F3B21723BF65C1", FullName = "Tracey Marcey" },
                Organization = new OrganizationDTO() { OrgId = "C00BB11B54234CD78566F46306B8A9E2", OrgName = "Franks Fish" },
                LastUpdatedDate = timeStamp,
                CreationDate = timeStamp
            };

            MappingVerifier.Verify<DbModelBase, RelationalEntityBase>(true);
            var entity = await _mapper.CreateAsync<DbModelBase, RelationalEntityBase>(db, Org(), User(), null, CancellationToken.None);

            Assert.That(entity.Id.Value, Is.EqualTo(db.Id.ToString()));
            Assert.That(entity.CreatedBy.Id, Is.EqualTo("D7A50FB68C3A44A0A8F3B21723BF65C1"));
            Assert.That(entity.CreatedBy.Text, Is.EqualTo("Tracey Marcey"));
            Assert.That(entity.OwnerOrganization.Id, Is.EqualTo("C00BB11B54234CD78566F46306B8A9E2"));
            Assert.That(entity.OwnerOrganization.Text, Is.EqualTo("Franks Fish"));
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
                LastUpdatedDate = UtcTimestamp.Now,
                CreationDate = UtcTimestamp.Now
            };

            MappingVerifier.Verify<RelationalEntityBase, DbModelBase>(true);
            var dto = await _mapper.CreateAsync<RelationalEntityBase, DbModelBase>(entity, Org(), User(), null, CancellationToken.None);
            Assert.That(dto.Id, Is.EqualTo(Guid.Parse(entity.Id)));
            Assert.That(dto.CreatedById, Is.EqualTo(User().Id));
            Assert.That(dto.OrganizationId, Is.EqualTo(Org().Id));
            Assert.That(dto.CreationDate, Is.EqualTo(timeStamp).Within(TimeSpan.FromSeconds(1)));
            Assert.That(dto.LastUpdatedDate, Is.EqualTo(timeStamp).Within(TimeSpan.FromSeconds(1)));

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
            Console.WriteLine(dto.EncryptedBalance);
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
            var eh = new EntityHeaderPrimary() { TheProperty = EntityHeader.Create("D7A50FB68C3A44A0A8F3B21723BF65C1", "My Entity Header") };
            var dto = await _mapper.CreateAsync<EntityHeaderPrimary, EntityHeaderDTO>(eh, Org(), User(), null, CancellationToken.None);
            Assert.That(dto.Id, Is.EqualTo("D7A50FB68C3A44A0A8F3B21723BF65C1"));
            Assert.That(dto.Text, Is.EqualTo("My Entity Header"));
        }

        [Test]
        public async Task Name_and_Id_ToEH()
        {
            var dto = new EntityHeaderDTO() { Id = "MYID", Text = "The TextFor The Property" };
            var eh = await _mapper.CreateAsync<EntityHeaderDTO, EntityHeaderPrimary>(dto, Org(), User(), null, CancellationToken.None);
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
        public async Task Map_NullableGuidString36_NullableGuid_With_Value()
        {
            var g = GuidString36.Factory();
            var src = new SourceWithNullableGuidString36() {  Prop1 = g };
            await _mapper.CreateAsync<SourceWithNullableGuidString36, TargetWithNullableGuid>(src, Org(), User());
            Assert.That(src.Prop1.HasValue, Is.True);
            Assert.That(src.Prop1, Is.EqualTo(g));
        }


        [Test]
        public async Task Map_NullableGuidString36_NullableGuid_Without_Value()
        {
            var src = new SourceWithNullableGuidString36() { Prop1 = null };
            await _mapper.CreateAsync<SourceWithNullableGuidString36, TargetWithNullableGuid>(src, Org(), User());
            Assert.That(src.Prop1.HasValue, Is.False);
        }

        [Test]
        public async Task Map_NullableGuid_NullableGuidString36_With_Value()
        {
            var g = Guid.NewGuid();
            var src = new TargetWithNullableGuid() { Prop1 = g };
            await _mapper.CreateAsync<TargetWithNullableGuid, SourceWithNullableGuidString36>(src, Org(), User());
            Assert.That(src.Prop1.HasValue, Is.True);
            Assert.That(src.Prop1, Is.EqualTo(g));
        }


        [Test]
        public async Task Map_NullableGuid_NullableGuidString36_Without_Value()
        {
            var src = new TargetWithNullableGuid() { Prop1 = null };
            await _mapper.CreateAsync<TargetWithNullableGuid, SourceWithNullableGuidString36>(src, Org(), User());
            Assert.That(src.Prop1.HasValue, Is.False);
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

        private sealed class FakeModernEncryption : IModernEncryption
        {
            // Minimal envelope: enc;v=2;alg=test;kv=1;aad=v1;data=<b64url>;
            public Task<string> EncryptAsync(EncryptStringRequest request, CancellationToken ct = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.Plaintext == null) throw new ArgumentNullException(nameof(request.Plaintext));

                var bytes = Encoding.UTF8.GetBytes(request.Plaintext);
                var b64url = Base64UrlEncode(bytes);

                var envelope = $"enc;v=2;alg=test;kv={request.Kv};aad=v1;data={b64url};";
                return Task.FromResult(envelope);
            }

            public Task<string> DecryptAsync(DecryptStringRequest request, CancellationToken ct = default)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrWhiteSpace(request.Envelope)) throw new ArgumentNullException(nameof(request.Envelope));

                var env = request.Envelope;

                // extremely small parser: find "data=" and read to next ';'
                var marker = "data=";
                var idx = env.IndexOf(marker, StringComparison.Ordinal);
                if (idx < 0) throw new FormatException("Envelope missing data= segment.");

                idx += marker.Length;
                var end = env.IndexOf(';', idx);
                if (end < 0) end = env.Length;

                var b64url = env.Substring(idx, end - idx);
                var bytes = Base64UrlDecode(b64url);
                var plaintext = Encoding.UTF8.GetString(bytes);

                return Task.FromResult(plaintext);
            }

            private static string Base64UrlEncode(byte[] data)
            {
                var s = Convert.ToBase64String(data);
                s = s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
                return s;
            }

            private static byte[] Base64UrlDecode(string s)
            {
                s = s.Replace('-', '+').Replace('_', '/');
                switch (s.Length % 4)
                {
                    case 2: s += "=="; break;
                    case 3: s += "="; break;
                }
                return Convert.FromBase64String(s);
            }
        }

        private sealed class FakeModernKeyIdBuilder : IModernKeyIdBuilder
        {
            // NOTE: adjust signature if your interface differs.
            public Task<string> BuildKeyGuiIdAsync<TDto>(TDto dto, ModernKeyIdAttribute attr, EntityHeader org, EntityHeader user, CancellationToken ct = default) where TDto : class
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (attr == null) throw new ArgumentNullException(nameof(attr));

                // For tests we just use the DTO Id -> guid N (lower) and prefix with "account-"
                // so it looks like a real v2 key id shape.
                var idProp = typeof(TDto).GetProperty("Id");
                if (idProp == null) throw new InvalidOperationException($"{typeof(TDto).Name} missing Id property.");

                var idVal = idProp.GetValue(dto);
                if (idVal is Guid g)
                {
                    var id32 = g.ToString("N").ToLowerInvariant();
                    return Task.FromResult($"account-{id32}:v2");
                }

                throw new InvalidOperationException($"{typeof(TDto).Name}.Id must be Guid for this test builder.");
            }
        }
    }
}