using LagoVista.Core.Configuration;
using LagoVista.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationExtensionsTests
    {
        [SetUp]
        public void SetUp() 
        {
            ConfigurationDiagnostics.Reset();
        }


        [Test]
        public void Set_Should_Throw_When_Configuration_Is_Null()
        {
            IConfiguration configuration = null;

            Assert.That(() => configuration.Set<ConnectionSettings>("Plaid", (section, connection) => { }),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Set_Should_Throw_When_SectionName_Is_Null()
        {
            var configuration = BuildConfiguration();

            Assert.That(() => configuration.Set<ConnectionSettings>(null, (section, connection) => { }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Set_Should_Throw_When_Configure_Is_Null()
        {
            var configuration = BuildConfiguration();

            Assert.That(() => configuration.Set<ConnectionSettings>("Plaid", null),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Set_Should_Throw_When_Section_Does_Not_Exist()
        {
            var configuration = BuildConfiguration();

            Assert.That(() => configuration.Set<ConnectionSettings>("MissingSection", (section, connection) => { }),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Missing configuration section 'MissingSection'."));
        }

        [Test]
        public void Set_Should_Create_Connection_And_Configure_From_Section()
        {
            var configuration = BuildConfiguration(
                ("Plaid:Uri", "https://plaid.example.com"),
                ("Plaid:ClientId", "client-123"),
                ("Plaid:Secret", "secret-xyz"));

            var result = configuration.Set<ConnectionSettings>("Plaid", (section, connection) =>
            {
                connection.Uri = section.Require("Uri");
                connection.AccountId = section.Require("ClientId");
                connection.AccessKey = section.Require("Secret");
            });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Uri, Is.EqualTo("https://plaid.example.com"));
            Assert.That(result.AccountId, Is.EqualTo("client-123"));
            Assert.That(result.AccessKey, Is.EqualTo("secret-xyz"));
        }

        [Test]
        public void Require_Should_Throw_When_Section_Is_Null()
        {
            IConfigurationSection section = null;

            Assert.That(() => section.Require("Uri"),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Require_Should_Throw_When_Key_Is_Null()
        {
            var configuration = BuildConfiguration(("Plaid:Uri", "https://plaid.example.com"));
            var section = configuration.GetRequiredSection("Plaid");

            Assert.That(() => section.Require(null),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Require_Should_Throw_When_Value_Is_Missing()
        {
            var configuration = BuildConfiguration(("Plaid:Uri", "https://plaid.example.com"));
            var section = configuration.GetRequiredSection("Plaid");
            section.Require("Secret");
            var missingEntries = ConfigurationDiagnostics.GetEntries();
            Assert.That(missingEntries.Where(ent => !ent.ValuePresent).Count() > 0);
            Assert.That(missingEntries.Where(ent => ent.Path == "Plaid:Secret" && !ent.ValuePresent).Count(), Is.EqualTo(1));
        }

        [Test]
        public void Require_Should_Throw_When_Value_Is_Whitespace()
        {
            var configuration = BuildConfiguration(("Plaid:Secret", "   "));
            var section = configuration.GetRequiredSection("Plaid");

            section.Require("Secret");

            var missingEntries = ConfigurationDiagnostics.GetEntries();
            Assert.That(missingEntries.Where(ent => !ent.ValuePresent).Count() > 0);
            Assert.That(missingEntries.Where(ent => ent.Path == "Plaid:Secret" && !ent.ValuePresent).Count(), Is.EqualTo(1));
        }

        [Test]
        public void Require_Should_Build_Two_Level_Path_Whitespace()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["top:child:value"] = "hello world"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();

            var parent = configuration.GetRequiredSection("top");
            var child = parent.GetRequiredSection("child");
            var childValue = child.Require("value");

            var entries = ConfigurationDiagnostics.GetEntries();

            Assert.That(entries.Count, Is.EqualTo(1));
            Assert.That(entries[0].Path, Is.EqualTo("top:child:value"));
            Assert.That(childValue, Is.EqualTo("hello world"));
        }

        [Test]
        public void Require_Should_Return_Value_When_Present()
        {
            var configuration = BuildConfiguration(("Plaid:Secret", "secret-xyz"));
            var section = configuration.GetRequiredSection("Plaid");

            var result = section.Require("Secret");

            Assert.That(result, Is.EqualTo("secret-xyz"));
        }

        [Test]
        public void CreateDBStorageSettings_Should_Return_Expected_Values()
        {
            var configuration = BuildConfiguration(
                ("DocDb:Endpoint", "https://cosmos.example.com"),
                ("DocDb:AccessKey", "access-key"),
                ("DocDb:DbName", "billing"));

            var result = configuration.CreateDBStorageSettings("DocDb");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Uri, Is.EqualTo("https://cosmos.example.com"));
            Assert.That(result.AccessKey, Is.EqualTo("access-key"));
            Assert.That(result.ResourceName, Is.EqualTo("billing"));
        }

        [Test]
        public void CreateDBStorageSettings_Should_Throw_When_Endpoint_Is_Missing()
        {
            var configuration = BuildConfiguration(
                ("DocDb:AccessKey", "access-key"),
                ("DocDb:DbName", "billing"));

            configuration.CreateDBStorageSettings("DocDb");

            var missingEntries = ConfigurationDiagnostics.GetEntries();
            Assert.That(missingEntries.Where(ent => !ent.ValuePresent).Count() > 0);
            Assert.That(missingEntries.Where(ent => ent.Path == "DocDb:Endpoint" && !ent.ValuePresent).Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateTableStorageSettings_Should_Return_Expected_Values()
        {
            var configuration = BuildConfiguration(
                ("TableStorage:Name", "storage-account"),
                ("TableStorage:AccessKey", "table-key"));

            var result = configuration.CreateTableStorageSettings("TableStorage");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccountId, Is.EqualTo("storage-account"));
            Assert.That(result.AccessKey, Is.EqualTo("table-key"));
        }

        [Test]
        public void CreateTableStorageSettings_Should_Throw_When_Name_Is_Missing()
        {
            var configuration = BuildConfiguration(("TableStorage:AccessKey", "table-key"));
            var result = configuration.CreateTableStorageSettings("TableStorage");

            var missingEntries = ConfigurationDiagnostics.GetEntries();
            Assert.That(missingEntries.Where(ent => !ent.ValuePresent).Count() > 0);
            Assert.That(missingEntries.Where( ent => ent.Path == "TableStorage:Name" && !ent.ValuePresent).Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateDefaultDBStorageSettings_Should_Use_DefaultDocDBStorage_And_Currently_Call_TableStorage_Settings()
        {
            var configuration = BuildConfiguration(
                ("AnySection:Anything", "value"),
                ("DefaultDocDBStorage:Endpoint", "docdb-account"),
                ("DefaultDocDBStorage:DbName", "somedb"),
                ("DefaultDocDBStorage:AccessKey", "docdb-key"));

            var result = configuration.CreateDefaultDBStorageSettings();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Uri, Is.EqualTo("docdb-account"));
            Assert.That(result.ResourceName, Is.EqualTo("somedb"));
            Assert.That(result.AccessKey, Is.EqualTo("docdb-key"));
        }


        [Test]
        public void CreateDefaultTableStorageSettings_Should_Use_DefaultTableStorage()
        {
            var configuration = BuildConfiguration(
                ("DefaultTableStorage:Name", "default-table-account"),
                ("DefaultTableStorage:AccessKey", "default-table-key"));

            var result = configuration.CreateDefaultTableStorageSettings();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccountId, Is.EqualTo("default-table-account"));
            Assert.That(result.AccessKey, Is.EqualTo("default-table-key"));
        }

        private static IConfigurationRoot BuildConfiguration(params (string Key, string Value)[] values)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
                data[item.Key] = item.Value;

            return new ConfigurationBuilder()
                .AddInMemoryCollection(data)
                .Build();
        }

       
    }
}
