using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Models
{
    public interface IClassA
    {
        String PropertyA { get; set; }
    }

    public interface IClassB
    {
        String PropertyA { get; set; }
    }

    public interface IClassC
    {
        IClassA ClassA { get; }
        IClassB ClassB { get; }
    }

    public class ClassA : IClassA
    {
        public String PropertyA { get; set; }
    }

    public class ClassB : IClassB
    {
        public String PropertyA { get; set; }
    }

    public class ClassC : IClassC
    {
        public ClassC(IClassA classA, IClassB classB)
        {
            ClassA = classA;
            ClassB = classB;
        }

        public IClassA ClassA { get; private set; }
        public IClassB ClassB { get; private set; }
    }
}
