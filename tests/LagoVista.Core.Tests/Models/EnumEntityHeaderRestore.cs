// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 63671efc131e92d396469220fdd7ff979d984f4136e2b823ba3c4173833e936a
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Models
{
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

        public enum EnumWithOutLabel
        {
            EnumWOLabel1,
            EnumWOLabel2,
            EnumWOLabel3,
        }

       
        [TestMethod]
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

        [TestMethod]
        public void EHCreateWithoutLabel()
        {
            var eh = EntityHeader<EnumWithOutLabel>.Create(EnumWithOutLabel.EnumWOLabel1);

            Assert.AreEqual("EnumWOLabel1", eh.Id);
            Assert.AreEqual(EnumWithOutLabel.EnumWOLabel1.ToString(), eh.Text);
            Assert.AreEqual(EnumWithOutLabel.EnumWOLabel1, eh.Value);
        }
    }
}
