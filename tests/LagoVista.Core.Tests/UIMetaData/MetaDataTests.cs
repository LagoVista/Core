using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagoVista.Core.Tests.UIMetaData
{
    [TestClass]
    public class MetaDataTests
    {
        [TestMethod]
        public void ShouldAddAssemblies()
        {
            MetaDataHelper.Instance.RegisterAssembly(this.GetType().Assembly);
        }
    }
}
