using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Models
{

    public class EntityHeaderFinderTestModel : LagoVista.Core.Models.EntityBase
    {
        public NestedEntityHeaderFinderTestModel Child { get; set; }
        public List<LagoVista.Core.Models.EntityHeader> Devices { get; set; }
    }

    public class NestedEntityHeaderFinderTestModel
    {
        public LagoVista.Core.Models.EntityHeader Child1 { get; set; }
        public LagoVista.Core.Models.EntityHeader Child2 { get; set; }
    
        public List<NestedEntityHeaderFinderTestModel2> GrandChildren { get; set; } 
    }


    public class NestedEntityHeaderFinderTestModel2
    {
        public LagoVista.Core.Models.EntityHeader Child3 { get; set; }
        public LagoVista.Core.Models.EntityHeader Child4 { get; set; }
    }


    [TestClass]
    public class EntityHeaderFindeTests
    {
        [TestMethod]
        public void SetChildOobjects()
        {
            var model = new EntityHeaderFinderTestModel()
            {
                OwnerOrganization = LagoVista.Core.Models.EntityHeader.Create("6053688597EE41CFBCE56372BD590501", "Organization 1"),    
                CreatedBy = LagoVista.Core.Models.EntityHeader.Create("6053688597EE41CFBCE56372BD590502", "User 1"),
                LastUpdatedBy = LagoVista.Core.Models.EntityHeader.Create("6053688597EE41CFBCE56372BD590502", "User 1"),
                Child = new NestedEntityHeaderFinderTestModel()
                {
                    Child1 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A1", Text = "CHILD1" },
                    Child2 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A2", Text = "CHILD2" },
                    GrandChildren = new List<NestedEntityHeaderFinderTestModel2>()
                    {
                        new NestedEntityHeaderFinderTestModel2()
                        {
                            Child3 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A3", Text="GRANDCHILD11" },
                            Child4 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A4", Text="GRANDCHILD12" },
                        },
                        new NestedEntityHeaderFinderTestModel2()
                        {
                            Child3 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A5", Text="GRANDCHILD21" },
                            Child4 = new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A6", Text="GRANDCHILD22" }, 
                        }
                    }
                },
                Devices = new List<LagoVista.Core.Models.EntityHeader>()
                {
                    new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A7", Text="DEV1" },
                    new LagoVista.Core.Models.EntityHeader() { Id = "6053688597EE41CFBCE56372BD5905A8", Text="DEV2" },
                }
            };

            var ehNodes =  model.FindEntityHeaderNodes();

            Console.WriteLine($"Found {ehNodes.Count} Entity Headers");

            var idx = 0;
            foreach (var eh in ehNodes)
            {
                Console.WriteLine($"Found Entity Header with Id: {eh.NormalizedPath} - Id[{eh.Id}] Key[{eh.Key}] Type:[{eh.EntityType}] - Name[{eh.Text}] IsPublic:[{eh.IsPublic}] - Ownr[{eh.OwnerOrgId}]");
                idx++;

                model.UpdateEntityHeaders(eh, key: $"KEY-{idx}", ownerOrgId:"TheOwner", isPublic: idx % 2 == 0, entityType: "NA1");
            }

            var ehNodes2 = model.FindEntityHeaderNodes();

            foreach (var eh in ehNodes2)
            {
                Console.WriteLine($"Found Entity Header with Id: {eh.NormalizedPath} - Id[{eh.Id}] Key[{eh.Key}] Type:[{eh.EntityType}] - Name[{eh.Text}] - IsPublic:[{eh.IsPublic}] -  Ownr[{eh.OwnerOrgId}]");
            }
        }
    }
}
