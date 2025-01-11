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
    }
}
