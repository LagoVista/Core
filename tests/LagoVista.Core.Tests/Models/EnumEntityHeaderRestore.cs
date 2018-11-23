using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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

       
        [Fact()]
        public void ShowRestoreValueOnEnumEntityHeaderType()
        {
            var eh = new EntityHeader<SampleEnum>();
            eh.Id = "enum4";
            eh.Text = ValidationResources.Names.EnumFour;

            var json = JsonConvert.SerializeObject(eh);

            var restoredEh = JsonConvert.DeserializeObject<EntityHeader<SampleEnum>>(json);

            Assert.Equal(restoredEh.Value,SampleEnum.Enum4);
        }

        [Fact()]
        public void EHCreate()
        {
            var eh = EntityHeader<SampleEnum>.Create(SampleEnum.Enum1);

            Assert.Equal("enum1", eh.Id);
            Assert.Equal(ValidationResources.EnumOne, eh.Text);
            Assert.Equal(SampleEnum.Enum1, eh.Value);
        }

        [Fact()]
        public void EHCreateWithoutLabel()
        {
            var eh = EntityHeader<EnumWithOutLabel>.Create(EnumWithOutLabel.EnumWOLabel1);

            Assert.Equal("EnumWOLabel1", eh.Id);
            Assert.Equal(EnumWithOutLabel.EnumWOLabel1.ToString(), eh.Text);
            Assert.Equal(EnumWithOutLabel.EnumWOLabel1, eh.Value);
        }
    }
}
