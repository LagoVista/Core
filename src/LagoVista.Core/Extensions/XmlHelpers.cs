using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LagoVista.Core
{
    public static class XmlHelpers
    {

        public static string GetNodeValue(this XDocument doc, String nodeName)
        {
            var node = doc.Root.Descendants().Where(des => des.Name.LocalName == nodeName).FirstOrDefault();
            if (node == null)
                return null;         

            return node.Value;
        }
        public static string GetNodeValue(this XElement ele, String nodeName)
        {
            var node = ele.Descendants().Where(des => des.Name.LocalName == nodeName).FirstOrDefault();
            if (node == null)
                return null;

            return node.Value;
        }

        public static string GetAttributeValue(this XElement ele, String nodeName, String attributeName = "val")
        {
            var node = ele.Descendants().Where(des => des.Name.LocalName == nodeName).FirstOrDefault();
            if (node == null)
                return null;

            var attr = node.Attributes().Where(att => att.Name == attributeName).FirstOrDefault();
            if (attr == null)
                return null;

            return attr.Value;
        }
    }
}
