using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.CloneableTests
{
    [TestClass]
    public class CloneTests
    {
        [TestMethod]
        public async Task Simple_Clone()
        {
            var original = new ParentModel();
            original.OwnerOrganization = EntityHeader.Create(Guid.NewGuid().ToId(), "owner org");
            original.LastUpdatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "owner org");
            original.CreatedBy = original.LastUpdatedBy;
            original.CreationDate = DateTime.UtcNow.AddDays(-5).ToJSONString();
            original.LastUpdatedDate = original.CreationDate;
            original.IsPublic = false;
            original.Name = "orig name";
            original.Key = "key";

            original.Child2 = new ChildModel2()
            {
                OwnerOrganization = original.OwnerOrganization,
                LastUpdatedBy = original.LastUpdatedBy,
                CreatedBy = original.CreatedBy,
                Name = "Some Name",
                Key = "somekey",
                IsPublic = true,
                CreationDate = original.CreationDate,
                Id = Guid.NewGuid().ToId(),
                LastUpdatedDate = original.LastUpdatedDate,
                GrandChild = new ChildModel2()
                {
                    OwnerOrganization = original.OwnerOrganization,
                    LastUpdatedBy = original.LastUpdatedBy,
                    CreatedBy = original.CreatedBy,
                    Name = "Some Name",
                    Key = "somekey",
                    IsPublic = true,
                    CreationDate = original.CreationDate,
                    Id = Guid.NewGuid().ToId(),
                    LastUpdatedDate = original.LastUpdatedDate,
                }
            };

            var originalGrandChildId = original.Child2.GrandChild.Id;

            var user2 = EntityHeader.Create(Guid.NewGuid().ToId(), "new user");
            var org2 = EntityHeader.Create(Guid.NewGuid().ToId(), "other user");

            original.Child1 = new ChildModel1()
            {
                Prop1 = "p1",
                Prop2 = "p2",
                Prop3 = "p3",
            };

            original.Children1 = new List<ChildModel1>();
            original.Children1.Add(new ChildModel1() { Prop1 = "a", Prop2 = "b", Prop3 = "c" });
            original.Children1.Add(new ChildModel1() { Prop1 = "d", Prop2 = "e", Prop3 = "f" });

            var clone = await original.CloneAsync(user2, org2, "DiffObject", "DIffKey");

            Assert.AreEqual("p1", clone.Child1.Prop1);
            Assert.AreEqual("p2", clone.Child1.Prop2);
            Assert.AreEqual("p3", clone.Child1.Prop3);

            Assert.AreEqual(org2.Id, clone.Child2.OwnerOrganization.Id);
            Assert.AreEqual(user2.Id, clone.Child2.CreatedBy.Id);
            Assert.AreEqual(user2.Id, clone.Child2.LastUpdatedBy.Id);

            Assert.AreEqual(2, clone.Children1.Count);
            Assert.IsNotNull(clone.Child2.GrandChild);

            Assert.AreEqual(org2.Id, clone.Child2.GrandChild.OwnerOrganization.Id);
            Assert.AreEqual(user2.Id, clone.Child2.GrandChild.CreatedBy.Id);
            Assert.AreEqual(user2.Id, clone.Child2.GrandChild.LastUpdatedBy.Id);

            Assert.AreNotEqual(original.Child2.Id, clone.Child2.LastUpdatedBy.Id);
            Assert.AreNotEqual(originalGrandChildId, clone.Child2.GrandChild.Id);
        }
    }
}
