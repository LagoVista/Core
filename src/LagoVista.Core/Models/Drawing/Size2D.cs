// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e1f77431225ab9856eca646a9113ce4f60d2eba4a8289c8c253f43ad13f72826
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.Drawing
{
    public class Size2D : ModelBase
    {
        public Size2D(double width, double length) 
        { 
            _width = width;
            _height = length;
        }
        
        private double _width;
        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }


        private double _height;
        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public override string ToString()
        {
            return $"{Width}, {Height}";
        }

        public Size2D Clone()
        {
            return new Size2D(Width, Height);
        }
    }
}
