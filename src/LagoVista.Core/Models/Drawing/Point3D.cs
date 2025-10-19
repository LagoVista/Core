// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d0ac2be503600217572d128aed71bd2753dc055cfa116b6a2ddbadb26c36a670
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Point3D<T>
    {
        public Point3D()
        {

        }

        public Point3D(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        public bool IsOrigin()
        {
            if (typeof(T) == typeof(Int32))
            {
                var p1 = this as Point3D<Int32>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(Int64))
            {
                var p1 = this as Point3D<Int64>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(UInt32))
            {
                var p1 = this as Point3D<UInt32>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(UInt64))
            {
                var p1 = this as Point3D<UInt64>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(Int16))
            {
                var p1 = this as Point3D<Int16>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(UInt16))
            {
                var p1 = this as Point3D<UInt16>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(double))
            {
                var p1 = this as Point3D<double>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            if (typeof(T) == typeof(decimal))
            {
                var p1 = this as Point3D<decimal>;
                return p1.X == 0 && p1.Y == 0 && p1.Z == 0;
            }

            return false;
        }

        public static Point3D<T> operator +(Point3D<T> a, Point2D<T> b)
        {
            if (typeof(T) == typeof(Int32))
            {
                var ia = a as Point3D<Int32>;
                var ib = b as Point2D<Int32>;

                return new Point3D<Int32>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Int64))
            {
                var ia = a as Point3D<Int64>;
                var ib = b as Point2D<Int64>;

                return new Point3D<Int64>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt32))
            {
                var ia = a as Point3D<UInt32>;
                var ib = b as Point2D<UInt32>;

                return new Point3D<UInt32>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z
                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt64))
            {
                var ia = a as Point3D<UInt64>;
                var ib = b as Point2D<UInt64>;

                return new Point3D<UInt64>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Int16))
            {
                var ia = a as Point3D<Int16>;
                var ib = b as Point2D<Int16>;

                return new Point3D<Int16>()
                {
                    X = (Int16)(ia.X + ib.X),
                    Y = (Int16)(ia.Y + ib.Y),
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt16))
            {
                var ia = a as Point3D<UInt16>;
                var ib = b as Point2D<UInt16>;

                return new Point3D<UInt16>()
                {
                    X = (UInt16)(ia.X + ib.X),
                    Y = (UInt16)(ia.Y + ib.Y),
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(double))
            {
                var ia = a as Point3D<double>;
                var ib = b as Point2D<double>;

                return new Point3D<double>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Single))
            {
                var ia = a as Point3D<Single>;
                var ib = b as Point2D<Single>;

                return new Point3D<Single>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = a as Point3D<decimal>;
                var ib = b as Point2D<decimal>;

                return new Point3D<decimal>()
                {
                    X = ia.X + ib.X,
                    Y = ia.Y + ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            throw new InvalidOperationException($"Can not add types {typeof(T).Name}");
        }

        public static Point3D<T> operator -(Point3D<T> a, Point2D<T> b)
        {
            if (typeof(T) == typeof(Int32))
            {
                var ia = a as Point3D<Int32>;
                var ib = b as Point2D<Int32>;

                return new Point3D<Int32>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Int64))
            {
                var ia = a as Point3D<Int64>;
                var ib = b as Point2D<Int64>;

                return new Point3D<Int64>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt32))
            {
                var ia = a as Point3D<UInt32>;
                var ib = b as Point2D<UInt32>;

                return new Point3D<UInt32>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z
                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt64))
            {
                var ia = a as Point3D<UInt64>;
                var ib = b as Point2D<UInt64>;

                return new Point3D<UInt64>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Int16))
            {
                var ia = a as Point3D<Int16>;
                var ib = b as Point2D<Int16>;

                return new Point3D<Int16>()
                {
                    X = (Int16)(ia.X - ib.X),
                    Y = (Int16)(ia.Y - ib.Y),
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(UInt16))
            {
                var ia = a as Point3D<UInt16>;
                var ib = b as Point2D<UInt16>;

                return new Point3D<UInt16>()
                {
                    X = (UInt16)(ia.X - ib.X),
                    Y = (UInt16)(ia.Y - ib.Y),
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(double))
            {
                var ia = a as Point3D<double>;
                var ib = b as Point2D<double>;

                return new Point3D<double>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(float))
            {
                var ia = a as Point3D<float>;
                var ib = b as Point2D<float>;

                return new Point3D<float>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(Single))
            {
                var ia = a as Point3D<float>;
                var ib = b as Point2D<float>;

                return new Point3D<float>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = a as Point3D<decimal>;
                var ib = b as Point2D<decimal>;

                return new Point3D<decimal>()
                {
                    X = ia.X - ib.X,
                    Y = ia.Y - ib.Y,
                    Z = ia.Z

                } as Point3D<T>;
            }

            throw new InvalidOperationException($"Can not subtract types {typeof(T).Name}");
        }

        public Point2D<T> ToPoint2D()
        {
            return new Point2D<T>()
            {
                X = X,
                Y = Y,
            };
        }

        public Point3D<T> Clone()
        {
            return new Point3D<T>(this.X, this.Y, this.Z);
        }

        public Point3D<T> Round(int positions)
        {
            if (typeof(T) == typeof(double))
            {
                var ia = this as Point3D<double>;
                ia.X = Math.Round(ia.X, positions);
                ia.Y = Math.Round(ia.Y, positions);
                ia.Z = Math.Round(ia.Z, positions);
                return ia as Point3D<T>;
            }

            if (typeof(T) == typeof(decimal))
            {
                var ia = this as Point3D<decimal>;
                ia.X = Math.Round(ia.X, positions);
                ia.Y = Math.Round(ia.Y, positions);
                ia.Z = Math.Round(ia.Z, positions);
                return ia as Point3D<T>;
            }

            throw new Exception("Round not supported on non double/decimal type.");
        }
    }
}
