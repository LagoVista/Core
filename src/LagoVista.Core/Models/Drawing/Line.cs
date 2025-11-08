// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4c0b283f6d0530e185b76894ac0d846f862b329df0a56a50e8771c31341eedb6
// IndexVersion: 2
// --- END CODE INDEX META ---
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
