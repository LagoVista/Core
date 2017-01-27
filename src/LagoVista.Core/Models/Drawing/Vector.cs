using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Vector<T>
    {
        public Point3D<T> Start { get; set; }
        public Point3D<T> End { get; set; }
    }
}
