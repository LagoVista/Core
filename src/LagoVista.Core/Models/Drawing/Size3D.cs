﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.Drawing
{
    public class Size3D : ModelBase
    {
        public Size3D(double wwidth, double length, double height)
        {
            _width = wwidth; 
            _height = height;
            _depth = length;
        }


        /// <summary>
        /// Depth of object
        /// </summary>
        private double _width;
        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        /// <summary>
        /// Dimension in the Z axis
        /// </summary>

        private double _depth;
        public double Depth
        {
            get => _depth;
            set => Set(ref _depth, value);
        }

        /// <summary>
        /// How tall the object is.
        /// </summary>
        private double _height;
        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public override string ToString()
        {
            return $"{Width}, {Depth}, {Height}";
        }
    }
}
