// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 661794e6377cf871d60d99b2a8166a060e53ddccbc51bf2ae191db658eacff7b
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using PdfSharpCore.Fonts;

namespace LagoVista.PDFServices
{
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Roboto";

        public byte[] GetFont(string faceName)
        {
            Console.WriteLine("GETTING FONT -> " + faceName);

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

            Console.WriteLine("Resolve Type Face -> " + familyName);

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

            if (familyName.Equals("Segoe UI", StringComparison.CurrentCultureIgnoreCase))
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
