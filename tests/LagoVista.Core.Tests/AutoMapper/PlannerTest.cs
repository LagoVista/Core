using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using Moq;
using NUnit.Framework;

namespace LagoVista.Core.Tests.AutoMapper
{

    [TestFixture]
    public class PlannerTest
    {

        [Test]
        public void BuildIt()
        {
            var builder = new Mock<IAtomicPlanBuilder>(); 

            var result = MappingPlans.For<BigParentDTO, BigParent>(builder.Object)
                  .IncludeChild(p => p.Child1, s => s.Child1, child =>
                  {
                      child.IncludeChild(p => p.GrandChild1, s => s.GrandChild1);
                      child.IncludeEntityHeaderValue(p => p.GrandChild2, s => s.GrandChild2);
                  })
                  .IncludeEntityHeaderValue(p => p.Child2, s => s.Child2)
                  .IncludeList(p => p.Child3, s => s.Child3)
                  .Build();
        }
    }
}
