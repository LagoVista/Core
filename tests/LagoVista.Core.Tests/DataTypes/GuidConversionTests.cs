using LagoVista.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.DataTypes
{
    [TestFixture]
    public class GuidConversionTests
    {
        [Test]
        public void Should_CreateFrom_Guid_String()
        {
           var entity = new EntityWithGuidString36AsId();
           entity.StoredId = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_Throw_WhenAssign()
        {
            var entity = new EntityWithNormalizedId32Asid();
            entity.StoredId = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_Assign_AsNormal()
        {
            var entity = new EntityWithNormalizedId32Asid();
            entity.Id = NormalizedId32.Factory();
        }
    }

    [AllowLegacyGuidDocumentId]
    public class EntityWithGuidString36AsId : EntityBase
    {
        
    }

    public class EntityWithNormalizedId32Asid : EntityBase
    {

    }
}
