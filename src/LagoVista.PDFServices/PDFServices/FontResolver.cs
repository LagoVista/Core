using Didstopia.PDFSharp.Fonts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LagoVista.PDFServices
{
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Roboto";

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {

                var assembly = typeof(FontResolver).GetTypeInfo().Assembly;
                var fullName = $"{assembly.FullName}.Fonts.{faceName}";
                var resources = assembly.GetManifestResourceNames();
                var resourceName = resources.First(x => x == faceName);
                using (var rs = assembly.GetManifestResourceStream(resourceName))
                {
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Roboto", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Roboto-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Roboto-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Roboto-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Roboto-Regular.ttf");
                }
            }
            return null;
        }
    }

}
