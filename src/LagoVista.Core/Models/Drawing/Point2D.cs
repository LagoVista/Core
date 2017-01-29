using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Point2D<T>
    {
        public Point2D()
        {

        }

        public Point2D(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; set; }
        public T Y { get; set; }
    }
}
