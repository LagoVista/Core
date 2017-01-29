using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class NamedColors : IColor
    {
        public byte A { get; set; }

        public byte B { get; set; }

        public byte G { get; set; }

        public byte R { get; set; }

        public static readonly NamedColors Gray = new NamedColors() { A = 0xFF, R = 0xA0, G = 0xA0, B = 0xA0 };
        public static readonly NamedColors LightGray = new NamedColors() { A = 0xFF, R = 0xE0, G = 0xE0, B = 0xE0 };
        public static readonly NamedColors DarkGray = new NamedColors() { A = 0xFF, R = 0x60, G = 0x60, B = 0x60 };
        public static readonly NamedColors Black = new NamedColors() { A = 0xFF, R = 0x00, G = 0x00, B = 0x00 };
        public static readonly NamedColors White = new NamedColors() { A = 0xFF, R = 0xFF, G = 0xFF, B = 0xFF };
        public static readonly NamedColors Red = new NamedColors() { A = 0xFF, R = 0xFF, G = 0x00, B = 0x00 };
        public static readonly NamedColors Cyan = new NamedColors() { A = 0xFF, R = 0x00, G = 0xFF, B = 0xFF };
        public static readonly NamedColors Orange = new NamedColors() { A = 0xFF, R = 0xF9, G = 0x73, B = 0x06 };
        public static readonly NamedColors Brown = new NamedColors() { A = 0xFF, R = 0x65, G = 0x37, B = 0x06 };
        public static readonly NamedColors Yellow = new NamedColors() { A = 0xFF, R = 0xFF, G = 0xFF, B = 0x14 };
        public static readonly NamedColors Aqua = new NamedColors() { A = 0xFF, R = 0x13, G = 0xEA, B = 0xC9 };
        public static readonly NamedColors Teal = new NamedColors() { A = 0xFF, R = 0x02, G = 0x93, B = 0x86 };
        public static readonly NamedColors Green = new NamedColors() { A = 0xFF, R = 0x00, G = 0xFF, B = 0x00 };
        public static readonly NamedColors Blue = new NamedColors() { A = 0xFF, R = 0x00, G = 0x00, B = 0xFF };
        public static readonly NamedColors NavyBlue = new NamedColors() { A = 0xFF, R = 0x00, G = 0x00, B = 0x80 };

        public String HexString
        {
            get
            {
                if(A < 0xFF)
                {
                    return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
                }
                else
                {
                    return String.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
                }
            }
        }

    }

    public interface IColorFactory
    {
        IColor CreateColor(byte a, byte r, byte g, byte b);
        IColor CreateColor(byte r, byte g, byte b);
        IColor CreateColor(NamedColors color);
        IColor CreateFromHexString(String color);
        IColor Create(Int32 color);

    }
}
