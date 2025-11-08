// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c0e528afdd462bb0c97706e954d50e96c8ddea409871e4521bfb85724ccb309f
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.Drawing
{
    public class Color
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public String HexString
        {
            get
            {
                if (A < 0xFF)
                {
                    return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
                }
                else
                {
                    return String.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
                }
            }
        }

        public static Color CreateColor(byte a, byte r, byte g, byte b)
        {
            return new Drawing.Color()
            {
                A = a,
                R = r,
                B = b,
                G = g,
            };
        }

        public static Color CreateColor(byte r, byte g, byte b)
        {
            return Color.CreateColor(0xFF, r, g, b);
        }

        public static Color CreateFromHexString(String color)
        {
            try
            {
                if (color.Length == 9)
                {
                    return new Color()
                    {
                        A = Byte.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
                        R = Byte.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
                        G = Byte.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
                        B = Byte.Parse(color.Substring(7, 2), System.Globalization.NumberStyles.HexNumber),
                    };
                }
                else if (color.Length == 7)
                {
                    return new Color()
                    {
                        A = 0xFF,
                        R = Byte.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
                        G = Byte.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
                        B = Byte.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
                    };
                }
                else if (color.Length == 4)
                {
                    return new Color()
                    {
                        A = 0xFF,
                        R = Byte.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber),
                        G = Byte.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber),
                        B = Byte.Parse(color.Substring(3, 1), System.Globalization.NumberStyles.HexNumber),
                    };
                }
                throw new Exception($"Could not parse {color} has hex string of #AARRGGBB, $RRGGBB or $RGB");
            }
            catch (Exception)
            {
                throw new Exception($"Could not parse {color} has hex string of #AARRGGBB, $RRGGBB or $RGB");

            }
        }
    }
}
