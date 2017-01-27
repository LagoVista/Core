using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Point3D<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }
    }
}
