using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.Models;
using LagoVista.Core.Services.Crypto;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Services.Crypto
{
    [TestFixture]
    public class ModernKeyIdBuilderTests
    {
        private sealed class FakeResolver : IKeyIdTargetResolver
        {
            public int CallCount { get; private set; }
            public string LastTargetPath { get; private set; }
            public Guid LastFk { get; private set; }
            public Guid ReturnGuid { get; set; }

            public Task<Guid> ResolveIdAsync(string targetPath, Guid fkValue, EntityHeader org, EntityHeader user, CancellationToken ct = default)
            {
                CallCount++;
                LastTargetPath = targetPath;
                LastFk = fkValue;
                return Task.FromResult(ReturnGuid);
            }
        }

        private sealed class InvoiceDto
        {
            public Guid CustomerId { get; set; }
        
            public String StrPropertyId { get; set; }

            public NormalizedId32 NormalizedId32 { get; set; }
        }

        private sealed class InvoiceLineItemDto
        {
            public Guid InvoiceId { get; set; }
            public InvoiceDto Invoice { get; set; }
        }

        [Test]
        public async Task Should_Build_KeyId_From_Direct_Guid_Property()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceDto { CustomerId = Guid.Parse("6f9619ff-8b86-d011-b42d-00c04fc964ff") };
            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "CustomerId"
            };

            var org = EntityHeader.Create(Guid.Parse("9a7b3301-2c03-0c9a-d311-894fe04f2504").ToId(), "Org");
            var user = EntityHeader.Create(Guid.Parse("2f1c4e5a-6b7c-8d9e-a0b1-c2d3e4f50617").ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(resolver.CallCount, Is.EqualTo(0));
            Assert.That(keyId, Is.EqualTo("customer-6f9619ff8b86d011b42d00c04fc964ff:v2"));
        }

        [Test]
        public async Task Should_Build_KeyId_From_Direct_Normalized32_Property()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceDto { NormalizedId32 = NormalizedId32.Factory() };
            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "NormalizedId32"
            };

            var org = EntityHeader.Create(Guid.Parse("9a7b3301-2c03-0c9a-d311-894fe04f2504").ToId(), "Org");
            var user = EntityHeader.Create(Guid.Parse("2f1c4e5a-6b7c-8d9e-a0b1-c2d3e4f50617").ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(resolver.CallCount, Is.EqualTo(0));
            Assert.That(keyId, Is.EqualTo($"customer-{dto.NormalizedId32.Value.ToLowerInvariant()}:v2"));
        }

        [Test]
        public async Task Should_Build_KeyId_From_Direct_String_Property()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceDto { StrPropertyId = Guid.NewGuid().ToId() };
            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "StrPropertyId"
            };

            var org = EntityHeader.Create(Guid.Parse("9a7b3301-2c03-0c9a-d311-894fe04f2504").ToId(), "Org");
            var user = EntityHeader.Create(Guid.Parse("2f1c4e5a-6b7c-8d9e-a0b1-c2d3e4f50617").ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(resolver.CallCount, Is.EqualTo(0));
            Assert.That(keyId, Is.EqualTo($"customer-{dto.StrPropertyId.ToLowerInvariant()}:v2"));
        }

        [Test]
        public async Task Should_Throw_If_Invalid_String_Idy()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceDto { StrPropertyId = Guid.NewGuid().ToId() + "INV" };
            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "StrPropertyId"
            };

            var org = EntityHeader.Create(Guid.Parse("9a7b3301-2c03-0c9a-d311-894fe04f2504").ToId(), "Org");
            var user = EntityHeader.Create(Guid.Parse("2f1c4e5a-6b7c-8d9e-a0b1-c2d3e4f50617").ToId(), "User");

            Assert.That(async () => await builder.BuildKeyGuiIdAsync(dto, attr, org, user), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task Should_Build_KeyId_From_Navigation_Path_When_Present()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceLineItemDto
            {
                InvoiceId = Guid.NewGuid(),
                Invoice = new InvoiceDto { CustomerId = Guid.Parse("6f9619ff-8b86-d011-b42d-00c04fc964ff") }
            };

            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "Invoice.CustomerId",
                FallbackFkProperty = "InvoiceId"
            };

            var org = EntityHeader.Create(Guid.NewGuid().ToId(), "Org");
            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(resolver.CallCount, Is.EqualTo(0));
            Assert.That(keyId, Is.EqualTo("customer-6f9619ff8b86d011b42d00c04fc964ff:v2"));
        }

        [Test]
        public async Task Should_Fallback_To_Resolver_When_Navigation_Missing()
        {
            var resolver = new FakeResolver
            {
                ReturnGuid = Guid.Parse("6f9619ff-8b86-d011-b42d-00c04fc964ff")
            };

            var builder = new ModernKeyIdBuilder(resolver);

            var invoiceId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            var dto = new InvoiceLineItemDto
            {
                InvoiceId = invoiceId,
                Invoice = null
            };

            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "Invoice.CustomerId",
                FallbackFkProperty = "InvoiceId"
                // FallbackTargetPath omitted; builder will use IdPath
            };

            var org = EntityHeader.Create(Guid.NewGuid().ToId(), "Org");
            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(resolver.CallCount, Is.EqualTo(1));
            Assert.That(resolver.LastTargetPath, Is.EqualTo("Invoice.CustomerId"));
            Assert.That(resolver.LastFk, Is.EqualTo(invoiceId));
            Assert.That(keyId, Is.EqualTo("customer-6f9619ff8b86d011b42d00c04fc964ff:v2"));
        }

        [Test]
        public void Should_Throw_When_Navigation_Missing_And_No_Fallback_Configured()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceLineItemDto
            {
                InvoiceId = Guid.NewGuid(),
                Invoice = null
            };

            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "Invoice.CustomerId"
            };

            var org = EntityHeader.Create(Guid.NewGuid().ToId(), "Org");
            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "User");

            Assert.That(async () => await builder.BuildKeyGuiIdAsync(dto, attr, org, user),
                Throws.Exception
                    .TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Unable to resolve IdPath"));
        }

        [Test]
        public void Should_Throw_When_Fk_Is_Empty()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceLineItemDto
            {
                InvoiceId = Guid.Empty,
                Invoice = null
            };

            var attr = new ModernKeyIdAttribute("customer-{id}:v2")
            {
                IdPath = "Invoice.CustomerId",
                FallbackFkProperty = "InvoiceId"
            };

            var org = EntityHeader.Create(Guid.NewGuid().ToId(), "Org");
            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "User");

            Assert.That(async () => await builder.BuildKeyGuiIdAsync(dto, attr, org, user),
                Throws.Exception
                    .TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Guid.Empty"));
        }

        [Test]
        public async Task Should_Substitute_OrgId_Token_When_Present()
        {
            var resolver = new FakeResolver();
            var builder = new ModernKeyIdBuilder(resolver);

            var dto = new InvoiceDto { CustomerId = Guid.Parse("6f9619ff-8b86-d011-b42d-00c04fc964ff") };
            var attr = new ModernKeyIdAttribute("org-{orgId}-customer-{id}:v2")
            {
                IdPath = "CustomerId"
            };

            var orgGuid = Guid.Parse("9a7b3301-2c03-0c9a-d311-894fe04f2504").ToId();
            var org = EntityHeader.Create(orgGuid, "Org");
            var user = EntityHeader.Create(Guid.NewGuid().ToId(), "User");

            var keyId = await builder.BuildKeyGuiIdAsync(dto, attr, org, user);

            Assert.That(keyId, Is.EqualTo("org-9a7b33012c030c9ad311894fe04f2504-customer-6f9619ff8b86d011b42d00c04fc964ff:v2"));
        }
    }
}
