using LagoVista.Core.Attributes;
using LagoVista.Core.Tests.Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.Extensions
{

    [TestClass]
    public class EnumToDisplayTests
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


        [TestMethod]
        public void ResolveName()
        {
            Assert.AreEqual("Five","enum5".GetEnumLabel<SampleEnum>());
        }


        [TestMethod]
        public void ResolveStringToEnum()
        {
            SampleEnum enumValue;
            Assert.IsTrue("enum5".TryGetFromValue<SampleEnum>(out enumValue));
            Assert.AreEqual(SampleEnum.Enum5, enumValue);
        }

        [TestMethod]
        public void Should_Fail_To_Resolve_Null_To_Enum()
        {
            SampleEnum enumValue;
            Assert.IsFalse(((string)null).TryGetFromValue<SampleEnum>(out enumValue));
        }
    }
}
