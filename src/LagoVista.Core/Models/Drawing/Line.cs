using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Line<T>
    {
        public Point2D<T> Start { get; set; }
        public Point2D<T> End { get; set; }
    }
}
