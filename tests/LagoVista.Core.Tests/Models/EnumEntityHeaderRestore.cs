using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Models
{
    [TestClass]
    public class EnumEntityHeaderRestore
    {
        public enum SampleEnum
        {
            [EnumLabel("enum1", ValidationResources.Names.EnumOne, typeof(ValidationResources))]
            Enum1,
            [EnumLabel("enum2", ValidationResources.Names.EnumTwo, typeof(ValidationResources))]
            Enum2,
            [EnumLabel("enum3", ValidationResources.Names.EnumThree, typeof(ValidationResources))]
            Enum3,
            [EnumLabel("enum4", ValidationResources.Names.EnumFour, typeof(ValidationResources))]
            Enum4,
            [EnumLabel("enum5", ValidationResources.Names.EnumFive, typeof(ValidationResources))]
            Enum5,
        }

       
        [TestMethod()]
        public void ShowRestoreValueOnEnumEntityHeaderType()
        {
            var eh = new EntityHeader<SampleEnum>();
            eh.Id = "enum4";
            eh.Text = ValidationResources.Names.EnumFour;

            var json = JsonConvert.SerializeObject(eh);

            var restoredEh = JsonConvert.DeserializeObject<EntityHeader<SampleEnum>>(json);

            Assert.AreEqual(restoredEh.Value,SampleEnum.Enum4);
        }

        [TestMethod]
        public void EHCreate()
        {
            var eh = EntityHeader<SampleEnum>.Create(SampleEnum.Enum1);

            Assert.AreEqual("enum1", eh.Id);
            Assert.AreEqual(ValidationResources.EnumOne, eh.Text);
            Assert.AreEqual(SampleEnum.Enum1, eh.Value);
        }
    }
}
