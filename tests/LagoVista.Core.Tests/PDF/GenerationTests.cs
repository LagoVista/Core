using LagoVista.PDFServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NLipsum.Core;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.PDF
{
    [TestClass]
    public class GenerationTests
    {
        public static string GenerateText(int wordCount, int numberLines = 1)
        {
            var lipsum = new LipsumGenerator();
            var text = new StringBuilder();
            for (var idx = 0; idx < numberLines; ++idx)
            {
                text.AppendLine($"{String.Join(" ", lipsum.GenerateWords(wordCount))}");
            }

            return text.ToString();
        }


        [TestMethod]
        public async Task TopMargin()
        {
            var generator = new PDFServices.PDFGenerator();
            generator.Margin = new Margin(50, 100, 50, 100);
            generator.StartDocument();
            generator.AddLogoToHeader(100,100);
            generator.AddClickableLink("Top margin should be twice as the left margin", "http://www.bing.com", GenerateText(120));
            generator.AddClickableLink("Has a very, very long label, want to see how this renders", "http://www.bing.com", GenerateText(120));

            using (var stream = new System.IO.MemoryStream())
            {
                generator.Write(stream);

                var writer = new StreamWriter(stream);
                System.IO.File.WriteAllBytes("X:\\links.pdf", (stream as MemoryStream).GetBuffer());
            }
        }

        [TestMethod]
        public async Task HyperlinkRendering()
        {
            var generator = new PDFServices.PDFGenerator();
            generator.StartDocument();
            generator.Margin = new Margin(50);

            generator.AddClickableLink("This is a Link", "http://www.bing.com", GenerateText(120));
            generator.AddClickableLink("Has a very, very long label, want to see how this renders", "http://www.bing.com", GenerateText(120));
                
            using (var stream = new System.IO.MemoryStream())
            {
                generator.Write(stream);

                var writer = new StreamWriter(stream);
                System.IO.File.WriteAllBytes("X:\\links.pdf", (stream as MemoryStream).GetBuffer());
            }
        }

        [TestMethod]
        public async Task ParagraphRendering()
        {
            var generator = new PDFServices.PDFGenerator();
            generator.Margin = new Margin(50);
            generator.StartDocument();
            generator.Footer = "Document Footer";

            generator.AddParagraph(GenerateText(20, 1), "Notes ");
            generator.AddParagraph(GenerateText(50, 1), "Notes 1");
            generator.AddParagraph(GenerateText(100, 1), "Notes 2");
            generator.AddParagraph(GenerateText(100, 2), "Notes 3");
            generator.AddParagraph(GenerateText(100, 3), "Notes 4");
            generator.AddParagraph(GenerateText(100, 1), "Notes 4.5");
            generator.AddParagraph(GenerateText(150, 4), "Notes 5");

            using (var stream = new System.IO.MemoryStream())
            {
                generator.Write(stream);

                var writer = new StreamWriter(stream);
                System.IO.File.WriteAllBytes("X:\\P1.pdf", (stream as MemoryStream).GetBuffer());
            }
        }
    }
}
