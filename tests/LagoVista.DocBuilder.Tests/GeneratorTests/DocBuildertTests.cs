using LagoVista.DocBuilder.Tests.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.DocBuilder.Tests.GeneratorTests
{
    public class DocBuildertTests
    {
        [Fact]
        public void GeneratesRootNode()
        {
            var tag = Generator.Instance.Generate(typeof(Model1));
            Console.Write(tag.ToHtmlString());
        }
    }
}
