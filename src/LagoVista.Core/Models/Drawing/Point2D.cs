﻿using System;
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

        public override string ToString()
        {
            return $"X={X}; Y={Y};";
        }

        // Note there is almost certinly a "smarter way" of doing this...not sure what it is though...

        public static Point2D<T> operator +(Point2D<T> a, Point2D<T> b)
        {
            if (typeof(T) == typeof(Int32))
            {
                var ia = a as Point2D<Int32>;
                var ib = a as Point2D<Int32>;

                return new Point2D<Int32>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(Int64))
            {
                var ia = a as Point2D<Int64>;
                var ib = a as Point2D<Int64>;

                return new Point2D<Int64>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt32))
            {
                var ia = a as Point2D<UInt32>;
                var ib = a as Point2D<UInt32>;

                return new Point2D<UInt32>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt64))
            {
                var ia = a as Point2D<UInt64>;
                var ib = a as Point2D<UInt64>;

                return new Point2D<UInt64>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(Int16))
            {
                var ia = a as Point2D<Int16>;
                var ib = a as Point2D<Int16>;

                return new Point2D<Int16>()
                {
                    X = (Int16)(ia.X + ib.X),
                    Y = (Int16)(ia.Y + ib.Y)

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt16))
            {
                var ia = a as Point2D<UInt16>;
                var ib = a as Point2D<UInt16>;

                return new Point2D<UInt16>()
                {
                    X = (UInt16)(ia.X + ib.X),
                    Y = (UInt16)(ia.Y + ib.Y)

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(double))
            {
                var ia = a as Point2D<double>;
                var ib = a as Point2D<double>;

                return new Point2D<double>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = a as Point2D<decimal>;
                var ib = a as Point2D<decimal>;

                return new Point2D<decimal>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y

                } as Point2D<T>;
            }

            throw new InvalidOperationException($"Can not add types {typeof(T).Name}");
        }

        public static Point2D<T> operator -(Point2D<T> a, Point2D<T> b)
        {
            if (typeof(T) == typeof(Int32))
            {
                var ia = a as Point2D<Int32>;
                var ib = a as Point2D<Int32>;

                return new Point2D<Int32>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(Int64))
            {
                var ia = a as Point2D<Int64>;
                var ib = a as Point2D<Int64>;

                return new Point2D<Int64>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt32))
            {
                var ia = a as Point2D<UInt32>;
                var ib = a as Point2D<UInt32>;

                return new Point2D<UInt32>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt64))
            {
                var ia = a as Point2D<UInt64>;
                var ib = a as Point2D<UInt64>;

                return new Point2D<UInt64>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(Int16))
            {
                var ia = a as Point2D<Int16>;
                var ib = a as Point2D<Int16>;

                return new Point2D<Int16>()
                {
                    X = (Int16)(ia.X - ib.X),
                    Y = (Int16)(ia.Y - ib.Y)

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(UInt16))
            {
                var ia = a as Point2D<UInt16>;
                var ib = a as Point2D<UInt16>;

                return new Point2D<UInt16>()
                {
                    X = (UInt16)(ia.X - ib.X),
                    Y = (UInt16)(ia.Y - ib.Y)

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(double))
            {
                var ia = a as Point2D<double>;
                var ib = a as Point2D<double>;

                return new Point2D<double>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = a as Point2D<decimal>;
                var ib = a as Point2D<decimal>;

                return new Point2D<decimal>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y

                } as Point2D<T>;
            }

            throw new InvalidOperationException($"Can not add types {typeof(T).Name}");
        }
        
        public Point2D<T> AddToX(T val)
        {
            var pt = new Point2D<T>() { X = val, Y = this.Y };
            var sum = pt + this;
            this.X = sum.X;
            this.Y = sum.Y;
            return this;
        }

        public Point2D<T> AddToY(T val)
        {
            var pt = new Point2D<T>() { Y = val, X = this.X};
            var sum = pt + this;
            this.X = sum.X;
            this.Y = sum.Y;
            return this;
        }

        public Point2D<T> SubtractFromX(T val)
        {
            var pt = new Point2D<T>() { X = val, Y = this.Y };
            var sum = pt - this;
            this.X = sum.X;
            this.Y = sum.Y;
            return this;
        }

        public Point2D<T> SubtractFromY(T val)
        {
            var pt = new Point2D<T>() { Y = val, X = this.X };
            var sum = pt - this;
            this.X = sum.X;
            this.Y = sum.Y;
            return this;
        }

        public Point2D<T> Round(int positions)
        {
            if (typeof(T) == typeof(double))
            {
                var ia = this as Point2D<double>;
                ia.X = Math.Round(ia.X, positions);
                ia.Y = Math.Round(ia.Y, positions);
                return ia as Point2D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = this as Point2D<decimal>;
                ia.X = Math.Round(ia.X, positions);
                ia.Y = Math.Round(ia.Y, positions);
                return ia as Point2D<T>;
            }

            throw new Exception("Round not supported on non double/decimal type.");
        }
    }
}
