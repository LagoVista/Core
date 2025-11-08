// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4f6c86aa8d4af4f50e820e1ce51a1eeee44e4a307bdb93061b4880d8989dbd1e
// IndexVersion: 2
// --- END CODE INDEX META ---
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
