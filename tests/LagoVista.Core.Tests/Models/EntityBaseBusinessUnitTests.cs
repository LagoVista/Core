using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace LagoVista.Core.Tests.Models
{
    [TestClass]
    public class EntityBaseBusinessUnitTests
    {
        [TestMethod]
        public void New_Entities_Default_To_Organization_Scope()
        {
            IEntityBase entity = new EntityBase();
            Assert.IsNull(entity.BusinessUnit);

            var json = JsonConvert.SerializeObject(entity);
            Assert.IsFalse(json.Contains("\"BusinessUnit\""), "Unset business unit should not add a field to stored JSON.");

            var restored = JsonConvert.DeserializeObject<EntityBase>(json);
            Assert.IsNull(restored.BusinessUnit);
        }

        [TestMethod]
        public void Business_Unit_Scope_Roundtrips_Through_JSON()
        {
            IEntityBase entity = new EntityBase()
            {
                OwnerOrganization = EntityHeader.Create("C8AD4589F26842E7A1AEFBAEFC979C9B", "Example Organization"),
                BusinessUnit = EntityHeader.Create("B8AD4589F26842E7A1AEFBAEFC979C9A", "Example Business Unit")
            };

            var json = JsonConvert.SerializeObject(entity);
            var restored = JsonConvert.DeserializeObject<EntityBase>(json);

            Assert.IsNotNull(restored.BusinessUnit);
            Assert.AreEqual(entity.BusinessUnit.Id, restored.BusinessUnit.Id);
            Assert.AreEqual(entity.BusinessUnit.Text, restored.BusinessUnit.Text);
            Assert.AreEqual(entity.OwnerOrganization.Id, restored.OwnerOrganization.Id);
        }

        [TestMethod]
        public void Business_Unit_Is_Scope_Metadata_And_Does_Not_Change_Content_Hash()
        {
            var entity = new EntityBase { Name = "Sample", Key = "sample" };

            LagoVista.EntityHasher.SetHash(entity);
            var orgHash = entity.Sha256Hex;

            entity.BusinessUnit = EntityHeader.Create("B8AD4589F26842E7A1AEFBAEFC979C9A", "Business Unit");
            LagoVista.EntityHasher.SetHash(entity);

            Assert.AreEqual(orgHash, entity.Sha256Hex);
        }
    }
}
