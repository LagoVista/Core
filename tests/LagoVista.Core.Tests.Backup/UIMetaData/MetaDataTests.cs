using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.Tests.UIMetaData
{
    public class MetaDataTests
    {
        [Fact]
        public void ShouldAddAssemblies()
        {
            MetaDataHelper.Instance.RegisterAssembly(this.GetType().Assembly);
        }
    }
}
